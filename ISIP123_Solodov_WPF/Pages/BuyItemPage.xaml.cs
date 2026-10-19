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
        private CS2Market _item;
        private int _itemCount = 1;
        public BuyItemPage(CS2Market item)
        {
            _item = item;
            InitializeComponent();

            Loaded += PageLoaded;
        }

        private void PageLoaded(object sender, RoutedEventArgs e) 
        {
            if (_item != null) 
            {
                ItemGrid.DataContext = _item;
                CountOfSkin.Text = _itemCount.ToString();

                foreach (var str in Core.ContextKIP.Iznos.ToList())
                {
                    if (str.MinFlot <= _item.Inventories.Flot && _item.Inventories.Flot < str.MaxFlot)
                    {
                        Sostoianie.Text = $"СОСТОЯНИЕ: {str.Name}"; ;
                    }
                }

                // Поиск цены скина (стоимости при последней продаже)
                var list = Core.ContextKIP.CS2Market.Where(x => x.StatusID == 2).ToList();
                int i = list.Count() - 1;
                while (i != 0)
                {
                    var str = list[i];

                    if (str.Inventories.Items.ID == _item.Inventories.Items.ID)
                    {
                        Price.Text = $"Начальная цена: {str.Price.ToString()} руб.";
                        break;
                    }

                    i--;
                }

                // Исключение: базовая цена скина (на ТП его никто не продавал)
                if (Price.Text == "тест")
                {
                    Price.Text = $"Начальная цена: {_item.Inventories.Items.AveragePrice.ToString()} руб.";
                }
            }
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            if (_itemCount > 1)
            {
                _itemCount--;
                CountOfSkin.Text = _itemCount.ToString();
            }
            else 
            {
                Minus.IsEnabled = false;
            }
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            if (_item.Count > 1 && (_itemCount + 1) <= _item.Count)
            {
                _itemCount++;
                CountOfSkin.Text = _itemCount.ToString();
            }
            else
            {
                Plus.IsEnabled = false;
            }
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
