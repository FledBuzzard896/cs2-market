using ISIP123_Solodov_WPF.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ISIP123_Solodov_WPF.Dialogs
{
    public partial class GabenDialog : Window
    {
        private List<Users> _users = null;
        private GabenDialogMode _mode;
        
        public string ButtonText { get; private set; }
        public GabenDialogMode CurrentMode => _mode;
        public List<Items> ItemsList { get; private set; }

        public GabenDialog()
        {
            InitializeComponent();
            Loaded += PageLoaded;
        }

        public GabenDialog(List<Users> usersList, GabenDialogMode mode)
        {
            InitializeComponent();
            _users = usersList;
            _mode = mode;
            Loaded += PageLoaded;
        }

        private void ItemComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is Items selectedItem && comboBox.Tag is Users user)
            {
                var result = MessageBox.Show(Application.Current.MainWindow, $"Вы уверены, что хотите выдать предмет:\n\n«{selectedItem.Name}»\n\nпользователю {user.Login}?", "Подтверждение выдачи", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        InventoryItems newStr = new InventoryItems
                        {
                            UserID = user.ID,
                            ItemID = selectedItem.ID,
                            IsListed = false
                        };

                        if (selectedItem.TypeID >= 7 && selectedItem.TypeID <= 13)
                        {
                            newStr.FloatValue = null;
                        }
                        else
                        {
                            Random random = new Random();
                            var randomFloat = random.NextDouble();

                            newStr.FloatValue = Math.Round(randomFloat, 8);
                        }

                        Core.ContextKIP.InventoryItems.Add(newStr);
                        Core.ContextKIP.SaveChanges();

                        string floatMsg = newStr.FloatValue.HasValue ? $"\nИзнос (Float): {newStr.FloatValue.Value}" : "";
                        MessageBox.Show(Application.Current.MainWindow, $"Предмет «{selectedItem.Name}» успешно выдан!{floatMsg}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(Application.Current.MainWindow, $"Ошибка при выдаче: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                comboBox.SelectedIndex = -1;
            }
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Users selectedUser)
            {
                switch (_mode)
                {
                    case GabenDialogMode.Ban:
                        selectedUser.IsBanned = true;
                        Core.ContextKIP.SaveChanges();
                        _users.Remove(selectedUser);
                        MainListBox.Items.Refresh();
                        break;

                    case GabenDialogMode.Unban:
                        selectedUser.IsBanned = false;
                        Core.ContextKIP.SaveChanges();
                        _users.Remove(selectedUser);
                        MainListBox.Items.Refresh();
                        break;

                    case GabenDialogMode.Check:
                        MessageBox.Show(Application.Current.MainWindow, $"Login: {selectedUser.Login}\nNickname: {selectedUser.Nickname}\nBalance: {selectedUser.Balance} rub.\nIsAdmin: {selectedUser.IsAdmin}\nIsBanned: {selectedUser.IsBanned}", "Подробная информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                }
            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            if (_users == null)
            {
                var ruCulture = new CultureInfo("ru-RU");
                MarketStatistic.Visibility = Visibility.Visible;

                TitleTBlock.Text = "СТАТИСТИКА ТОРГОВОЙ ПЛОЩАДКИ:";

                var MarketLst = Core.ContextKIP.MarketListings.ToList();
                var InventoriesLst = Core.ContextKIP.InventoryItems.ToList();

                double GMV = 0;
                double MarketCapOfItems = 0;
                double MarketCapitalization = 0;

                foreach (var item in MarketLst)
                {
                    if (item.StatusID == 2) 
                    { 
                        GMV += Convert.ToDouble(item.Price + item.Commission); 
                        MarketCapitalization += Convert.ToDouble(item.Commission);
                    }
                }

                while (MarketLst.Count > 0)
                {
                    var currentItem = MarketLst[0];
                    var pricesSumOfItem = MarketLst.Where(x => x == currentItem).Sum(x => x.Price);
                    int countOfItemInMarket = MarketLst.Count(x => x == currentItem);
                    int countOfUtemInInventories = InventoriesLst.Count(x => x == currentItem.InventoryItems);

                    MarketCapOfItems += Convert.ToDouble(pricesSumOfItem / countOfItemInMarket);

                    MarketLst.RemoveAll(x => x == currentItem);
                }

                GmvTBlock.Text = $"{GMV.ToString("N2", ruCulture)} ₽";
                MarketCupTBlock.Text = $"{MarketCapOfItems.ToString("N2", ruCulture)} ₽";
                MarketCapitalizationTBlock.Text = $"{MarketCapitalization.ToString("N2", ruCulture)} ₽";
            }
            else
            {
                switch (_mode)
                {
                    case GabenDialogMode.Ban:
                        TitleTBlock.Text = "ЗАБАНИТЬ ИГРОКА:";
                        ButtonText = "Забанить";
                        break;
                    case GabenDialogMode.Unban:
                        TitleTBlock.Text = "РАЗБАНИТЬ ИГРОКА:";
                        ButtonText = "Разбанить";
                        break;
                    case GabenDialogMode.GiveItem:
                        TitleTBlock.Text = "ВЫДАТЬ ПРЕДМЕТ:";
                        ButtonText = "Выдать";
                        
                        ItemsList = Core.ContextKIP.Items.ToList();
                        break;
                    case GabenDialogMode.Check:
                        TitleTBlock.Text = "ПРОВЕРКА ИГРОКА:";
                        ButtonText = "Профиль";
                        break;
                }

                DataContext = this;

                MainListBox.ItemsSource = _users;
            }
        }
    }
}