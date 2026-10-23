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
        private Inventories _thisItem;
        private List<Inventories> _userInventory;

        private bool _updating;
        private int _itemCount = 1;
        private int _maxItemCount;

        public SellDialog(Inventories item)
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
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MinusBtn.IsEnabled) 
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
            
        }

        private void PlusBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PlusBtn.IsEnabled) 
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
        }

        // Вспомогательная функция для ввода только чисел
        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var tb = (TextBox)sender;
            string newText = tb.Text.Insert(tb.CaretIndex, e.Text);

            // ^-?\d*[.,]?\d*$
            e.Handled = !Regex.IsMatch(newText, @"^-?\d*[.,]?\d*$");
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


        private void SellerProfit_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_updating) return;

            // Если поле пустое — чистим парное и выходим
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

            // Если поле пустое — чистим парное и выходим
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
                SellerProfit.Text = (price * 0.87m).ToString("0.##",
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

            AveragePrice.Text = $"Средняя цена: {_thisItem.Items.AveragePrice} руб.";

            // Поиск цены скина (стоимости при последней продаже)
            var list = Core.Context.CS2Market.Where(x => x.StatusID == 2).ToList();
            int i = list.Count() - 1;
            while (i >= 0)
            {
                var str = list[i];

                if (str.Inventories.Items.ID == _thisItem.Items.ID)
                {
                    LastUserPrice.Text = $"Начальная цена: {str.Price} руб.";
                    break;
                }

                i--;
            }

            if (string.IsNullOrEmpty(LastUserPrice.Text))
            {
                LastUserPrice.Text = $"Начальная цена: {_thisItem.Items.AveragePrice} руб.";
            }

            // Поиск максимально возможного количества данного типа предмета в инвентаре
            _userInventory = Core.Context.Inventories
                .Where(x => x.UserID == Core.CurrentUser.ID)
                .ToList();

            _maxItemCount = _thisItem.Count;

            if (_maxItemCount == 1)
            {
                MinusBtn.IsEnabled = false;
                PlusBtn.IsEnabled = false;

                DescriptionTBlock.Visibility = Visibility.Visible;
            }

            CountTBlock.Text = _itemCount.ToString();
        }
    }
}
