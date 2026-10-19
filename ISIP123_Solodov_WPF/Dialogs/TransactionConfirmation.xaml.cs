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
using System.Windows.Shapes;

namespace ISIP123_Solodov_WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для TransactionConfirmation.xaml
    /// </summary>
    public partial class TransactionConfirmation : Window
    {
        private CS2Market _item;
        private int _count;
        public TransactionConfirmation(CS2Market item, int count)
        {
            InitializeComponent();

            _item = item;
            _count = count;

            Loaded += PageLoaded;
        }

        private void PageLoaded(object sender, RoutedEventArgs e) 
        {
            MainGrid.DataContext = _item;

            // Поиск цены скина (стоимости при последней продаже)
            var list = Core.ContextKIP.CS2Market.Where(x => x.StatusID == 1).ToList();
            int i = list.Count() - 1;
            while (i != 0)
            {
                var str = list[i];

                if (str.Inventories.Items.ID == _item.Inventories.Items.ID)
                {
                    Price.Text = $"{str.Price.ToString()} руб.";
                    Price_2.Text = $"{str.Price.ToString()} руб.";
                    break;
                }

                i--;
            }

            // Исключение: базовая цена скина (на ТП его никто не продавал)
            if (Price.Text == "тест")
            {
                Price.Text = $"{_item.Inventories.Items.AveragePrice.ToString()} руб.";
                Price_2.Text = $"{_item.Inventories.Items.AveragePrice.ToString()} руб.";
            }

            double total = Convert.ToDouble(Price.Text + Convert.ToDouble(_item.Commission));
            TotalSum.Text = $"{total} руб.";
        }
    }
}
