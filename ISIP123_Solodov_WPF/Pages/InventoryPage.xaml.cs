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

        private List<Inventories> _UserInventory = null;
        private List<Inventories> _InventorySection = null;

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
        private void MarketBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Market());
        }


        // Листрание инвентаря
        private void rightBtn_Click(object sender, RoutedEventArgs e)
        {
            if (rightBtn.IsEnabled) 
            {
                _currentInvetoryPage++;
                count.Text = $"{_currentInvetoryPage} из {_countOfInventoryPages}";

                skipCount = 25 * (_currentInvetoryPage - 1);
                _InventorySection = _UserInventory.Skip(skipCount).Take(25).ToList();
                InventoryItems.ItemsSource = _InventorySection;


                leftBtn.IsEnabled = true;   

                if (_currentInvetoryPage == _countOfInventoryPages) 
                {
                    rightBtn.IsEnabled = false;
                }

                TypesCombo.SelectedItem = null;
                QualitesCombo.SelectedItem = null;
            }
        }
        private void leftBtn_Click(object sender, RoutedEventArgs e)
        {
            if (leftBtn.IsEnabled) 
            {
                _currentInvetoryPage--;
                count.Text = $"{_currentInvetoryPage} из {_countOfInventoryPages}";

                skipCount = 25 * (_currentInvetoryPage - 1);
                _InventorySection = _UserInventory.Skip(skipCount).Take(25).ToList();
                InventoryItems.ItemsSource = _InventorySection;


                rightBtn.IsEnabled = true;

                if (_currentInvetoryPage == 1) 
                {
                    leftBtn.IsEnabled = false;
                }

                TypesCombo.SelectedItem = null;
                QualitesCombo.SelectedItem = null;
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

                // 1. Приводим к FrameworkElement (у него есть .Parent)
                // 2. Используем safe-cast к UIElement для вызова RaiseEvent
                var parent = ((FrameworkElement)sender).Parent as UIElement;

                parent?.RaiseEvent(eventArg);
            }
        }
        // Выбор одного элемента ListBox
        private void InventoryItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InventoryItems.SelectedItem is Inventories selectedItem)
            {
                ItemDetails(selectedItem);
            }
        }
        private void ItemDetails(Inventories item) 
        {
            if (item.Flot is null)
            {
                FlotTblock.Visibility = Visibility.Collapsed;
            }
            else 
            {
                FlotTblock.Visibility = Visibility.Visible;

                double? flot = item.Flot;
                foreach (var str in Core.Context.Iznos.ToList())
                {
                    if (str.MinFlot <= flot && flot < str.MaxFlot)
                    {
                        IznosTblock.Text = $"Состояние: {str.Name}";
                    }
                }
            }

            if (item.Items.QualityID is null)
            {
                IznosTblock.Visibility = Visibility.Collapsed;
            }
            else 
            {
                IznosTblock.Visibility = Visibility.Visible;
            }

            // Поиск цены скина (стоимости при последней продаже)
            StartPrice.Text = "тест";

            var list = Core.Context.CS2Market.Where(x => x.StatusID == 2).ToList();
            int i = list.Count() - 1;
            while (i != 0)
            {
                var str = list[i];

                if (str.Inventories.Items.ID == item.Items.ID)
                {
                    StartPrice.Text = $"Начальная цена: {str.Price.ToString()} руб.";
                    break;
                }

                i--;
            }

            // Исключение: базовая цена скина (на ТП его никто не продавал)
            if (StartPrice.Text == "тест")
            {
                StartPrice.Text = $"Начальная цена: {item.Items.AveragePrice.ToString()} руб.";
            }

            ItemCardGrid.DataContext = item;
        }


        // Фильтрация и Поиск
        private void SearchItemTBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }
        private void TypesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }
        private void QualitesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }
        private void ApplyFilters()
        {
            var changed_dataList = _InventorySection.ToList();

            // Поиск по названию
            string search = SearchItemTBox.Text.Trim().ToLower();

            if (!string.IsNullOrEmpty(search))
            {
                changed_dataList = changed_dataList.Where(x =>
                    x.Items.Name.ToLower().StartsWith(search) ||
                    x.Items.Name.ToLower().Contains(search)
                ).ToList();
            }

            // Фильтрация по качеству предмета
            //if (QualitesCombo.SelectedItem != null)
            //{
            //    string selectedQuality = QualitesCombo.SelectedItem.ToString();

            //    if (!string.IsNullOrEmpty(selectedQuality) && selectedQuality != "Все")
            //    {
            //        changed_dataList = changed_dataList
            //            .Where(x => x.Items.Qualities.Name == selectedQuality)
            //            .ToList();
            //    }
            //}

            // Фильтрация по типу предмета
            if (TypesCombo.SelectedItem != null)
            {
                string selectedType = TypesCombo.SelectedItem.ToString();

                if (!string.IsNullOrEmpty(selectedType) && selectedType != "Все")
                {
                    changed_dataList = changed_dataList
                        .Where(x => x.Items.Types.Name == selectedType)
                        .ToList();
                }
            }

            // Подгружает отфильтрованный список    
            InventoryItems.ItemsSource = changed_dataList;
        }


        // Переход к продаже предмета
        private void SellItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryItems.SelectedItem is Inventories selectedItem)
            {

            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e) 
        {
            NicknameTBlock.Text = Core.CurrentUser.Nickname;
            NicknameTBlock_2.Text = Core.CurrentUser.Nickname;


            // Подгрузка фильтров
            var types = Core.Context.Types.Select(x => x.Name).ToList();
            types.Insert(0, "Все");
            TypesCombo.ItemsSource = types;

            var qualities = Core.Context.Qualities.Select(x => x.Name).ToList();
            qualities.Insert(0, "Все");
            QualitesCombo.ItemsSource = qualities;
            

            // Загрузка инвентаря
            var items = Core.Context.Inventories.Where(x => x.UserID == Core.CurrentUser.ID);
            var itemsCopy = new List<Inventories>();

            foreach (var item in items.ToList())
            {
                itemsCopy.Add(item);
                if (item.Count > 1)
                {
                    for (int i = 1; i < item.Count; i++)
                    {
                        itemsCopy.Add(item);
                    }
                }
            }
            itemsCopy.Reverse();

            InventoryItems.ItemsSource = itemsCopy;
            _UserInventory = itemsCopy;
            _InventorySection = itemsCopy.Take(25).ToList();


            // Проработка кнопок листания инвентаря
            _countOfInventoryPages = itemsCopy.Count / 25 + 1;
            count.Text = $"{_currentInvetoryPage} из {_countOfInventoryPages}";

            if (_countOfInventoryPages > 1) 
            {
                rightBtn.IsEnabled = true;
            }


            if (itemsCopy.Count > 0)
            {
                ItemCardGrid.Visibility = Visibility.Visible;

                ItemDetails(itemsCopy[0]);
            }
        }
    }
}
