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
    /// Логика взаимодействия для BuyItemPage.xaml
    /// </summary>
    public partial class BuyItemPage : Page
    {
        private MarketListings _item;
        private int _itemCount = 1;
        private int _maxItemCount = 1;

        public BuyItemPage(MarketListings item)
        {
            _item = item;
            InitializeComponent();

            Loaded += PageLoaded;
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            if (_itemCount > 1)
            {
                _itemCount--;
                CountOfSkin.Text = _itemCount.ToString();
                Plus.IsEnabled = true;
                if (_itemCount == 1) Minus.IsEnabled = false;
            }
        }
        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            if (_itemCount < _maxItemCount)
            {
                _itemCount++;
                CountOfSkin.Text = _itemCount.ToString();
                Minus.IsEnabled = true;
                if (_itemCount == _maxItemCount) Plus.IsEnabled = false;
            }
        }


        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser != null)
            {
                // Берем N самых дешевых лотов (неважно, кто продавец)
                var listingsToBuy = Core.Context.MarketListings
                    .Where(x => x.StatusID == 1
                             && x.InventoryItems.ItemID == _item.InventoryItems.ItemID)
                    .OrderBy(x => x.Price) // Сортируем от дешевых к дорогим
                    .Take(_itemCount)      // Берем столько, сколько выбрал юзер
                    .ToList();

                // Открываем окно подтверждения и передаем туда СПИСОК
                TransactionConfirmation dialog = new TransactionConfirmation(listingsToBuy);
                if (dialog.ShowDialog() == true)
                {
                    // После успешной покупки возвращаемся на маркет
                    if (NavigationService.CanGoBack) NavigationService.GoBack();
                }
                return;
            }

            MessageBox.Show("Войдите в аккаунт, чтобы совершать покупки на Торговой площадке Stem", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
        }


        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }


        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            if (_item != null)
            {
                ItemGrid.DataContext = _item;
                CountOfSkin.Text = _itemCount.ToString();

                // 1. Определение степени износа (если есть FloatValue)
                if (_item.InventoryItems.FloatValue != null)
                {
                    foreach (var str in Core.Context.WearCategories.ToList())
                    {
                        if (str.MinFloat <= _item.InventoryItems.FloatValue && _item.InventoryItems.FloatValue < str.MaxFloat)
                        {
                            Sostoianie.Text = $"Состояние: {str.Name}";
                            break;
                        }
                    }
                }
                else
                {
                    Sostoianie.Text = "Состояние: Отсутствует";
                }

                // 2. Поиск цены (быстрый LINQ вместо цикла)
                var activeListing = Core.Context.MarketListings
                    .Where(x => x.StatusID == 1 && x.InventoryItems.ItemID == _item.InventoryItems.ItemID)
                    .OrderBy(x => x.Price)
                    .FirstOrDefault();

                if (activeListing != null)
                {
                    Price.Text = $"Цена на рынке: от {activeListing.Price:N0} руб.";
                }
                else
                {
                    var lastSold = Core.Context.MarketListings
                        .Where(x => x.StatusID == 2 && x.InventoryItems.ItemID == _item.InventoryItems.ItemID)
                        .OrderByDescending(x => x.SoldAt)
                        .FirstOrDefault();

                    Price.Text = lastSold != null
                        ? $"Последняя продажа: {lastSold.Price:N0} руб."
                        : $"Средняя цена: {_item.InventoryItems.Items.AveragePrice:N0} руб.";
                }

                // 3. Считаем, сколько ВСЕГО таких предметов сейчас продается на рынке (у всех продавцов)
                _maxItemCount = Core.Context.MarketListings.Count(x =>
                    x.StatusID == 1 &&
                    x.InventoryItems.ItemID == _item.InventoryItems.ItemID);

                if (_maxItemCount <= 1)
                {
                    _maxItemCount = 1;
                    Minus.IsEnabled = false;
                    Plus.IsEnabled = false;
                }
                else
                {
                    Minus.IsEnabled = false; // Изначально стоит 1
                    Plus.IsEnabled = true;
                }
            }
        }
    }
}
