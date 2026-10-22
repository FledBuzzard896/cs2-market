using ISIP123_Solodov_WPF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
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
    /// Логика взаимодействия для SellDialog.xaml
    /// </summary>
    public partial class SellDialog : Window
    {
        private Inventories _thisItem;

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
        }
    }
}
