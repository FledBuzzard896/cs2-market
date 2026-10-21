using ISIP123_Solodov_WPF.Dialogs;
using ISIP123_Solodov_WPF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для Market.xaml
    /// </summary>
    public partial class Market : Page
    {
        public Market()
        {
            InitializeComponent();
            Loaded += PageLoaded;
        }

        private void listingsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser != null)
            {
                ActiveDialog dialog = new ActiveDialog();
                dialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Чтобы посмотреть ваши лоты на Торговой Площадке, вам нужно зайти в аккаунт", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void historyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser != null)
            {
                HistoryDialog dialog = new HistoryDialog();
                dialog.ShowDialog();
            }
            else 
            {
                MessageBox.Show("Чтобы посмотреть вашу историю на Торговой Площадке, вам нужно зайти в аккаунт", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void sellItemBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser is null) 
            {
                MessageBox.Show("Вы должны зарегистрироваться, чтобы зайти в Инвентарь.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            NavigationService.Navigate(new InventoryPage());
        }


        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.CurrentUser = null;
            NavigationService.Navigate(new AuthPage());
        }


        private void ShowError_Dialog(object sender, RoutedEventArgs e)
        {
            Error dialog = new Error();
            dialog.ShowDialog();
        }
        private void communityBtn_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://steamcommunity.com/market/"; 

            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true 
            });
        }


        // Открытие ссылки на ресурс
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });

            e.Handled = true;
        }

        // Настройка для глобального скрола по ListBox
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

        // Обработка клика по нику 
        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Core.CurrentUser == null) 
            {
                MessageBox.Show("Войдите в аккаунт, чтобы посмотреть свой профиль", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ProfileDialog dialog = new ProfileDialog();
            dialog.ShowDialog();

            balance.Text = $"Баланс кошелька: {Core.CurrentUser.Balance.ToString()} ₽";
        }

        // Выбор элемента для покупки
        private void ItemsLB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ItemsLB.SelectedItem is CS2Market selectedMarketItem)
            {
                // Передаем объект на страницу покупки
                NavigationService.Navigate(new BuyItemPage(selectedMarketItem));
            }
        }


        private void searchTBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            // Берем весь список предметов
            var market = Core.Context.CS2Market.Where(x => x.StatusID == 1).ToList();
            var changed_dataList = market.ToList();

            // Поиск по названию
            string search = searchTBox.Text.Trim().ToLower();

            if (!string.IsNullOrEmpty(search))
            {
                changed_dataList = changed_dataList.Where(x =>
                    x.Inventories.Items.Name.ToLower().StartsWith(search) ||
                    x.Inventories.Items.Name.ToLower().Contains(search)
                ).ToList();
            }

            // Фильтрация по качеству предмета
            if (QualitesCombo.SelectedItem != null) 
            {
                string selectedQuality = QualitesCombo.SelectedItem.ToString();

                // Проверяем, что строка не пустая и не равна "Все"
                if (!string.IsNullOrEmpty(selectedQuality) && selectedQuality != "Все")
                {
                    changed_dataList = changed_dataList
                        .Where(x => x.Inventories.Items.Qualities.Name == selectedQuality)
                        .ToList();
                }
            }

            // Фильтрация по типу предмета
            if (TypesCombo.SelectedItem != null)    
            {
                string selectedType = TypesCombo.SelectedItem.ToString();

                if (!string.IsNullOrEmpty(selectedType) && selectedType != "Все")
                {
                    changed_dataList = changed_dataList
                        .Where(x => x.Inventories.Items.Types.Name == selectedType)
                        .ToList();
                }
            }

            // Сортировка
            if (SortingCombo.SelectedItem is ComboBoxItem sortItem)
            {
                string sortValue = sortItem.Content.ToString();
                switch (sortValue)
                {
                    case "Название (↑)":
                        changed_dataList = changed_dataList.OrderBy(x => x.Inventories.Items.Name).ToList();
                        break;
                    case "Название (↓)":
                        changed_dataList = changed_dataList.OrderByDescending(x => x.Inventories.Items.Name).ToList();
                        break;
                    case "Цена (↑)":
                        changed_dataList = changed_dataList.OrderBy(x => x.Price).ToList();
                        break;
                    case "Цена (↓)":
                        changed_dataList = changed_dataList.OrderByDescending(x => x.Price).ToList();
                        break;
                    default:
                        break;
                }
            }
            
            // Подгружает отфильтрованный список    
            ItemsLB.ItemsSource = changed_dataList;
        }


        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            // Подгрузка ТП
            var market = Core.Context.CS2Market.Where(x => x.StatusID == 1).ToList();
            if (Core.CurrentUser != null)
            {
                market = market.Where(x => x.Users1.ID != Core.CurrentUser.ID).ToList();
            }
            ItemsLB.ItemsSource = market;


            // Подгрузка фильтров
            var types = Core.Context.Types.Select(x => x.Name).ToList();
            types.Insert(0, "Все");
            TypesCombo.ItemsSource = types;

            var qualities = Core.Context.Qualities.Select(x => x.Name).ToList();
            qualities.Insert(0, "Все");
            QualitesCombo.ItemsSource = qualities;


            if (Core.CurrentUser is null)
            {
                NicknameTBox.Text = "guest";
                balance.Text = $"Баланс кошелька: 0 ₽";
            }
            else 
            {
                NicknameTBox.Text = Core.CurrentUser.Nickname;
                balance.Text = $"Баланс кошелька: {Core.CurrentUser.Balance.ToString()} ₽";
            }
        }


    }
}
