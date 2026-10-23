using ISIP123_Solodov_WPF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для SellDialog.xaml
    /// </summary>
    public partial class SellDialog : Window
    {
        private InventoryItems _thisItem;

        private bool _updating;
        private int _itemCount = 1;
        private int _maxItemCount;

        public SellDialog(InventoryItems item)
        {
            _thisItem = item;

            InitializeComponent();
            Loaded += PageLoaded;
        }

        private void SellButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SellerProfit.Text) || string.IsNullOrEmpty(BuyerPays.Text))
            {
                MessageBox.Show("Задайте стоимость предмету", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SSA.IsChecked == false)
            {
                MessageBox.Show("Примите соглашение подписчика Stem, для торговли на Торговой площадке", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Безопасный парсинг цены (учитывает и точку, и запятую)
            string priceStr = SellerProfit.Text.Replace(',', '.');
            if (!decimal.TryParse(priceStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal price))
            {
                MessageBox.Show("Некорректный формат цены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // 1. Берем нужное количество предметов из инвентаря (только те, что еще не продаются)
                var itemsToSell = Core.Context.InventoryItems
                    .Where(x => x.UserID == Core.CurrentUser.ID && x.ItemID == _thisItem.ItemID && !x.IsListed)
                    .Take(_itemCount)
                    .ToList();

                if (itemsToSell.Count < _itemCount)
                {
                    MessageBox.Show("Недостаточно предметов в инвентаре для продажи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Комиссия считается от цены, которую получит продавец
                decimal commission = (price / 100) * 13;

                // 2. Создаем список новых лотов в памяти
                var newListings = new List<MarketListings>();

                foreach (var item in itemsToSell)
                {
                    item.IsListed = true; // Помечаем, что предмет выставлен

                    newListings.Add(new MarketListings
                    {
                        InventoryID = item.ID,
                        Price = price,
                        Commission = commission,
                        SellerID = Core.CurrentUser.ID,
                        BuyerID = null,
                        SoldAt = null,
                        CreatedAt = DateTime.Now,
                        StatusID = 1 // Активен
                    });
                }

                // 3. Добавляем ВСЕ лоты одной пачкой
                foreach (var listing in newListings)
                {
                    Core.Context.MarketListings.Add(listing);
                }

                Core.Context.SaveChanges();

                MessageBox.Show($"Успешно выставлено предметов: {_itemCount}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выставлении: {ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_itemCount > 1)
            {
                _itemCount--;
                CountTBlock.Text = _itemCount.ToString();

                PlusBtn.IsEnabled = true;
                if (_itemCount == 1)
                {
                    MinusBtn.IsEnabled = false;
                }
            }
        }
        private void PlusBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_itemCount < _maxItemCount)
            {
                _itemCount++;
                CountTBlock.Text = _itemCount.ToString();

                MinusBtn.IsEnabled = true;
                if (_itemCount == _maxItemCount)
                {
                    PlusBtn.IsEnabled = false;
                }
            }
        }


        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var tb = (TextBox)sender;
            string newText = tb.Text.Insert(tb.CaretIndex, e.Text);

            // Разрешаем только цифры, точку/запятую и минус в начале
            e.Handled = !Regex.IsMatch(newText, @"^-?\d*[.,]?\d*$");
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


        private void SellerProfit_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_updating) return;

            if (string.IsNullOrWhiteSpace(SellerProfit.Text))
            {
                _updating = true;
                try { BuyerPays.Text = string.Empty; }
                finally { _updating = false; }
                return;
            }

            string s = SellerProfit.Text.Replace(',', '.');
            if (!decimal.TryParse(s, System.Globalization.NumberStyles.Any,
                                  System.Globalization.CultureInfo.InvariantCulture, out decimal price))
                return;

            _updating = true;
            try
            {
                BuyerPays.Text = (price * 1.13m).ToString("0.##",
                                  System.Globalization.CultureInfo.InvariantCulture);
            }
            finally
            {
                _updating = false;
            }
        }
        private void BuyerPays_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_updating) return;

            if (string.IsNullOrWhiteSpace(BuyerPays.Text))
            {
                _updating = true;
                try { SellerProfit.Text = string.Empty; }
                finally { _updating = false; }
                return;
            }

            string s = BuyerPays.Text.Replace(',', '.');
            if (!decimal.TryParse(s, System.Globalization.NumberStyles.Any,
                                  System.Globalization.CultureInfo.InvariantCulture, out decimal price))
                return;

            _updating = true;
            try
            {
                SellerProfit.Text = (price / 1.13m).ToString("0.##",
                                    System.Globalization.CultureInfo.InvariantCulture);
            }
            finally
            {
                _updating = false;
            }
        }


        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            MainGrid.DataContext = _thisItem;

            AveragePrice.Text = $"Средняя цена: {_thisItem.Items?.AveragePrice} руб.";

            // 1. Ищем АКТИВНЫЙ лот (StatusID == 1) для этого предмета
            var activeListing = Core.Context.MarketListings
                .Where(x => x.StatusID == 1 && x.InventoryItems.Items.ID == _thisItem.Items.ID)
                .OrderBy(x => x.Price) // Берем самый дешевый на рынке
                .FirstOrDefault();

            if (activeListing != null)
            {
                // Ситуация 3: Есть активный лот. Берем его цену.
                LastUserPrice.Text = $"Цена на рынке: {activeListing.Price} руб.";
            }
            else
            {
                // Ищем ПОСЛЕДНЮЮ ПРОДАЖУ (StatusID == 2)
                var lastSoldListing = Core.Context.MarketListings
                    .Where(x => x.StatusID == 2 && x.InventoryItems.Items.ID == _thisItem.Items.ID)
                    .OrderByDescending(x => x.SoldAt)
                    .FirstOrDefault();

                if (lastSoldListing != null)
                {
                    // Ситуация 2: Продавали когда-то, но активных лотов нет. Берем цену последней продажи.
                    LastUserPrice.Text = $"Последняя продажа: {lastSoldListing.Price} руб.";
                }
                else
                {
                    // Ситуация 1: Никогда не продавали и активных нет. Берем среднюю цену.
                    LastUserPrice.Text = $"Средняя цена: {_thisItem.Items?.AveragePrice} руб.";
                }
            }

            // Считаем, сколько ТАКИХ ЖЕ предметов есть в инвентаре (и не выставлено на продажу)
            _maxItemCount = Core.Context.InventoryItems
                .Count(x => x.UserID == Core.CurrentUser.ID && x.ItemID == _thisItem.ItemID && !x.IsListed);

            if (_maxItemCount <= 1)
            {
                _maxItemCount = 1;
                _itemCount = 1;
                MinusBtn.IsEnabled = false;
                MinusBtn.Foreground = Brushes.DarkRed;
                PlusBtn.IsEnabled = false;
                PlusBtn.Foreground = Brushes.DarkRed;
                DescriptionTBlock.Visibility = Visibility.Visible;
            }
            else
            {
                MinusBtn.IsEnabled = false;
                PlusBtn.IsEnabled = true;
            }

            CountTBlock.Text = _itemCount.ToString();
        }
    }
}
