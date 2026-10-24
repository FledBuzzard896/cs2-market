using ISIP123_Solodov_WPF.Dialogs;
using ISIP123_Solodov_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ISIP123_Solodov_WPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для InventoryPage.xaml
    /// </summary>
    public partial class InventoryPage : Page
    {
        private int _countOfInventoryPages;
        private int _currentInvetoryPage = 1;
        private int skipCount;

        private List<InventoryItems> _UserInventory = null;
        private List<InventoryItems> _InventorySection = null;
        private List<InventoryItems> _FilteredInventory = null; // Для хранения отфильтрованного списка

        public InventoryPage()
        {
            InitializeComponent();
            Loaded += PageLoaded;
        }

        // Кнопка выхода
        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.CurrentUser = null;
            NavigationService.Navigate(new AuthPage());
        }


        // Кнопки навигации
        private void ShowError_Dialog(object sender, RoutedEventArgs e)
        {
            Error dialog = new Error();
            dialog.ShowDialog();
        }


        // Кнопка навигации: Маркет
        private void MarketBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Market());
        }


        // Листание инвентаря
        private void rightBtn_Click(object sender, RoutedEventArgs e)
        {
            if (rightBtn.IsEnabled)
            {
                _currentInvetoryPage++;
                UpdatePage();
            }
        }
        private void leftBtn_Click(object sender, RoutedEventArgs e)
        {
            if (leftBtn.IsEnabled)
            {
                _currentInvetoryPage--;
                UpdatePage();
            }
        }

        // Вычисление элементов текущей страницы
        private void UpdatePage()
        {
            count.Text = $"{_currentInvetoryPage} из {_countOfInventoryPages}";

            skipCount = 25 * (_currentInvetoryPage - 1);
            _InventorySection = _FilteredInventory.Skip(skipCount).Take(25).ToList();
            InventoryItems.ItemsSource = _InventorySection;

            leftBtn.IsEnabled = _currentInvetoryPage > 1;
            rightBtn.IsEnabled = _currentInvetoryPage < _countOfInventoryPages;

            if (_InventorySection.Count > 0)
            {
                ItemCardGrid.Visibility = Visibility.Visible;
                InventoryItems.SelectedIndex = 0;
                ItemDetails(_InventorySection[0]);
            }
            else
            {
                ItemCardGrid.Visibility = Visibility.Collapsed;
            }
        }


        // Прокрутка при наведении на ListBox
        private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = UIElement.MouseWheelEvent,
                    Source = sender
                };
                var parent = ((FrameworkElement)sender).Parent as UIElement;
                parent?.RaiseEvent(eventArg);
            }
        }


        // Выбор одного элемента ListBox
        private void InventoryItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InventoryItems.SelectedItem is InventoryItems selectedItem)
            {
                ItemDetails(selectedItem);
            }
        }
        private void ItemDetails(InventoryItems item)
        {
            // 1. Проверка на наличие флоата
            if (item.FloatValue is null)
            {
                FlotTblock.Visibility = Visibility.Collapsed;
                IznosTblock.Visibility = Visibility.Collapsed; // Если нет флоата, то и износа нет
            }
            else
            {
                FlotTblock.Visibility = Visibility.Visible;
                IznosTblock.Visibility = Visibility.Visible;

                double? floatVal = item.FloatValue;
                // Ищем степень износа 
                foreach (var str in Core.ContextKIP.WearCategories.ToList())
                {
                    if (str.MinFloat <= floatVal && floatVal < str.MaxFloat)
                    {
                        IznosTblock.Text = $"Состояние: {str.Name}";
                        break;
                    }
                }
            }

            // 2. Ищем минимальную стоимость
            // Ищем активные лоты
            var activeListing = Core.ContextKIP.MarketListings
                .Where(x => x.StatusID == 1 && x.InventoryItems.ItemID == item.ItemID)
                .OrderBy(x => x.Price) // Берем самый дешевый
                .FirstOrDefault();

            if (activeListing != null)
            {
                // Ситуация 3: Есть активные лоты, берем минимальную цену на рынке
                StartPrice.Text = $"Цена на рынке: от {activeListing.Price:N0} руб.";
            }
            else
            {
                // Если активных нет, ищем оследнюю продажу
                var lastSold = Core.ContextKIP.MarketListings
                    .Where(x => x.StatusID == 2 && x.InventoryItems.ItemID == item.ItemID)
                    .OrderByDescending(x => x.SoldAt)
                    .FirstOrDefault();

                if (lastSold != null)
                {
                    // Ситуация 2: Продавали когда-то, но сейчас активных нет
                    StartPrice.Text = $"Последняя продажа: {lastSold.Price:N0} руб.";
                }
                else
                {
                    // Ситуация 1: Вообще никогда не продавали
                    StartPrice.Text = $"Средняя цена: {item.Items.AveragePrice:N0} руб.";
                }
            }

            ItemCardGrid.DataContext = item;
        }

        // Фильтрация и Поиск
        private void SearchItemTBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void TypesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void QualitesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void ApplyFilters()
        {
            if (_UserInventory == null) return;

            // Вычисление фильтров по всему списку
            _FilteredInventory = _UserInventory.ToList();

            // Поиск по названию
            string search = SearchItemTBox.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                _FilteredInventory = _FilteredInventory.Where(x =>
                    x.Items.Name.ToLower().Contains(search)
                ).ToList();
            }

            // Фильтрация по типу предмета
            if (TypesCombo.SelectedItem != null)
            {
                string selectedType = TypesCombo.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(selectedType) && selectedType != "Все")
                {
                    _FilteredInventory = _FilteredInventory
                        .Where(x => x.Items.ItemTypes.Name == selectedType)
                        .ToList();
                }
            }

            // Фильтрация по качеству
            if (QualitesCombo.SelectedItem != null)
            {
                string selectedQuality = QualitesCombo.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(selectedQuality) && selectedQuality != "Все")
                {
                    _FilteredInventory = _FilteredInventory
                        .Where(x => x.Items.Qualities.Name == selectedQuality)
                        .ToList();
                }
            }

            // Вычисление количества страниц
            _countOfInventoryPages = (int)Math.Ceiling((double)_FilteredInventory.Count / 25);
            if (_countOfInventoryPages == 0) _countOfInventoryPages = 1;
            if (_currentInvetoryPage > _countOfInventoryPages) _currentInvetoryPage = _countOfInventoryPages;

            UpdatePage();
        }

        // Переход к продаже предмета
        private void SellItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryItems.SelectedItem is InventoryItems selectedItem)
            {
                SellDialog dialog = new SellDialog(selectedItem);
                if (dialog.ShowDialog() == true)
                {
                    PageLoaded(null, null);
                }
            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null) return;

            NicknameTBlock.Text = Core.CurrentUser.Nickname;
            NicknameTBlock_2.Text = Core.CurrentUser.Nickname;

            // Подгрузка фильтров
            if (TypesCombo.ItemsSource == null)
            {
                var types = Core.ContextKIP.ItemTypes.Select(x => x.Name).ToList();
                types.Insert(0, "Все");
                TypesCombo.ItemsSource = types;

                var qualities = Core.ContextKIP.Qualities.Select(x => x.Name).ToList();
                qualities.Insert(0, "Все");
                QualitesCombo.ItemsSource = qualities;
            }

            // Загрузка инвентаря
            _UserInventory = Core.ContextKIP.InventoryItems
                .Where(x => x.UserID == Core.CurrentUser.ID && !x.IsListed)
                .OrderByDescending(x => x.ID)
                .ToList();

            _currentInvetoryPage = 1;
            ApplyFilters();
        }
    }
}
