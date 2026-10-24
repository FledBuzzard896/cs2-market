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

namespace ISIP123_Solodov_WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для TransactionConfirmation.xaml
    /// </summary>
    public partial class TransactionConfirmation : Window
    {
        private List<MarketListings> _listings; // Список конкретных лотов, которые покупаем
        private decimal _totalBuyerPays;        // Итого с учетом комиссии
        private decimal _totalSellerGets;       // Промежуточный итог (без комиссии)


        public TransactionConfirmation(List<MarketListings> listings)
        {
            InitializeComponent();

            _listings = listings;
            Loaded += PageLoaded;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SSA.IsChecked != true)
            {
                MessageBox.Show("Примите соглашение Stem", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Core.CurrentUser.Balance < _totalBuyerPays)
            {
                MessageBox.Show("У вас недостаточно средств, чтобы купить данные предметы", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                Core.CurrentUser.Balance -= _totalBuyerPays;

                foreach (var listing in _listings)
                {
                    // 1. Находим предмет в инвентаре
                    var itemInInventory = Core.ContextKIP.InventoryItems.FirstOrDefault(x => x.ID == listing.InventoryID);

                    if (itemInInventory == null) continue; 

                    // 2. Передаем предмет покупателю
                    itemInInventory.UserID = Core.CurrentUser.ID;
                    itemInInventory.IsListed = false; // Снимаем с продажи

                    // 3. Начисляем деньги конкретному продавцу (этого лота)
                    var seller = Core.ContextKIP.Users.FirstOrDefault(x => x.ID == listing.SellerID);
                    if (seller != null)
                    {
                        seller.Balance += listing.Price;

                        // Записываем транзакцию для продавца
                        var sellerTransaction = new Transactions
                        {
                            UserID = seller.ID,
                            OperationID = 3, // Продажа
                            Amount = listing.Price,
                            TransactionDate = DateTime.Now,
                        };
                        Core.ContextKIP.Transactions.Add(sellerTransaction);
                    }

                    // 4. Записываем транзакцию для покупателя
                    var buyerTransaction = new Transactions
                    {
                        UserID = Core.CurrentUser.ID,
                        OperationID = 2, // Покупка
                        Amount = listing.Price + listing.Commission, 
                        TransactionDate = DateTime.Now,
                    };
                    Core.ContextKIP.Transactions.Add(buyerTransaction);

                    // 5. Закрываем лот на маркете
                    listing.StatusID = 2; // Продан
                    listing.BuyerID = Core.CurrentUser.ID;
                    listing.SoldAt = DateTime.Now;
                }

                Core.ContextKIP.SaveChanges();

                MessageBox.Show($"Успешно куплено предметов: {_listings.Count}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при покупке: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
            e.Handled = true;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            if (_listings == null || _listings.Count == 0) return;

            // Берем информацию о предмете из первого лота (они все одинаковые)
            var firstItem = _listings.First().InventoryItems.Items;

            foreach (var listing in _listings)
            {
                _totalSellerGets += listing.Price;
                _totalBuyerPays += (listing.Price + listing.Commission);
            }

            ItemName.Text = $"{firstItem.Name} (x{_listings.Count})";
            ItemImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(firstItem.ItemPicture));

            Price.Text = $"{_totalSellerGets:N0} руб.";
            Price_2.Text = $"{_totalSellerGets:N0} руб.";

            // Считаем общую комиссию
            decimal totalCommission = _listings.Sum(x => x.Commission);
            CommissionText.Text = $"{totalCommission:N0} руб.";

            TotalSum.Text = $"{_totalBuyerPays:N0} руб.";

            LoginTextBlock.Text = Core.CurrentUser.Login;
        }
    }
}

