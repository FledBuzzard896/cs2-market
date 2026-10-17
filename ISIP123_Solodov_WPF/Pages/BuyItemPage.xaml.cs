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


                foreach (var str in Core.ContextKIP.Iznos.ToList())
                {
                    if (str.MinFlot <= _item.Inventories.Flot && _item.Inventories.Flot < str.MaxFlot)
                    {
                        Sostoianie.Text = $"СОСТОЯНИЕ: {str.Name}"; ;
                    }
                }
            }
        }
    }
}
