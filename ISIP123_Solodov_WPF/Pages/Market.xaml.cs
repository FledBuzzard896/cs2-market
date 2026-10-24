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
    public partial class Market : Page
    {
        public Market()
        {
            InitializeComponent();
            Loaded += PageLoaded;
        }

        // Выход с аккаунта / Просмотр аккаунта
        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.CurrentUser = null;
            NavigationService.Navigate(new AuthPage());
        }
        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                MessageBox.Show("Войдите в аккаунт, чтобы посмотреть свой профиль", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ProfileDialog dialog = new ProfileDialog();
            dialog.ShowDialog();
            balance.Text = $"Баланс кошелька: {Core.CurrentUser.Balance} ₽";
        }


        // Навигационная плашка
        private void ShowError_Dialog(object sender, RoutedEventArgs e)
        {
            Error dialog = new Error();
            dialog.ShowDialog();
        }
        private void communityBtn_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://steamcommunity.com/market/";
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }


        // Активные лоты, история сделок и продава предмета
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


        // ВСПОМОГАЛКА: Открытие ссылки на ресурс
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = e.Uri.AbsoluteUri, UseShellExecute = true });
            e.Handled = true;
        }

        // ВСПОМОГАЛКА: Прокрутка страницы на LitBox
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


        // ВЫБОР ПРЕДМЕТА ДЛЯ ПОКУПКИ
        private void ItemsLB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ItemsLB.SelectedItem != null)
            {
                // Так как мы используем анонимный тип для группировки, берем самый дешевый лот из группы
                dynamic selectedGroup = ItemsLB.SelectedItem;
                MarketListings cheapestListing = selectedGroup.CheapestListing;

                NavigationService.Navigate(new BuyItemPage(cheapestListing));
            }
        }

        private void SearchTBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void TypesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void QualitesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void ApplyFilters()
        {
            // Берем все активные лоты из новой таблицы
            var market = Core.ContextKIP.MarketListings.Where(x => x.StatusID == 1).ToList();

            // Скрываем лоты текущего пользователя (чтобы он не купил сам у себя)
            if (Core.CurrentUser != null)
            {
                market = market.Where(x => x.SellerID != Core.CurrentUser.ID).ToList();
            }

            var changedDataList = market.ToList();

            // 1. Поиск по названию
            string search = SearchTBox.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                changedDataList = changedDataList.Where(x =>
                    x.InventoryItems.Items.Name.ToLower().Contains(search)
                ).ToList();
            }

            // 2. Фильтрация по качеству
            if (QualitesCombo.SelectedItem != null)
            {
                string selectedQuality = QualitesCombo.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(selectedQuality) && selectedQuality != "Все")
                {
                    changedDataList = changedDataList
                        .Where(x => x.InventoryItems.Items.Qualities.Name == selectedQuality)
                        .ToList();
                }
            }

            // 3. Фильтрация по типу
            if (TypesCombo.SelectedItem != null)
            {
                string selectedType = TypesCombo.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(selectedType) && selectedType != "Все")
                {
                    changedDataList = changedDataList
                        .Where(x => x.InventoryItems.Items.ItemTypes.Name == selectedType)
                        .ToList();
                }
            }

            // 4. Группировка
            var groupedList = changedDataList
                .GroupBy(x => x.InventoryItems.ItemID) // Группируем по ID предмета
                .Select(g => new
                {
                    ItemID = g.Key,
                    ItemName = g.First().InventoryItems.Items.Name,
                    ItemPicture = g.First().InventoryItems.Items.ItemPicture,
                    Price = g.Min(x => x.Price), // Показываем самую дешевую цену
                    TotalCount = g.Count(), // Общее количество таких предметов
                    CheapestListing = g.OrderBy(x => x.Price).First() // Запоминаем самый дешевый лот для покупки
                })
                .ToList();

            // 5. Сортировка
            if (SortingCombo.SelectedItem is ComboBoxItem sortItem)
            {
                string sortValue = sortItem.Content.ToString();
                switch (sortValue)
                {
                    case "Название (↑)":
                        groupedList = groupedList.OrderBy(x => x.ItemName).ToList();
                        break;
                    case "Название (↓)":
                        groupedList = groupedList.OrderByDescending(x => x.ItemName).ToList();
                        break;
                    case "Цена (↑)":
                        groupedList = groupedList.OrderBy(x => x.Price).ToList();
                        break;
                    case "Цена (↓)":
                        groupedList = groupedList.OrderByDescending(x => x.Price).ToList();
                        break;
                }
            }

            ItemsLB.ItemsSource = groupedList;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            // Подгрузка фильтров
            var types = Core.ContextKIP.ItemTypes.Select(x => x.Name).ToList(); // Было Types
            types.Insert(0, "Все");
            TypesCombo.ItemsSource = types;

            var qualities = Core.ContextKIP.Qualities.Select(x => x.Name).ToList();
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
                balance.Text = $"Баланс кошелька: {Core.CurrentUser.Balance} ₽";
            }

            // Загружаем предметы
            ApplyFilters();
        }
    }
}
