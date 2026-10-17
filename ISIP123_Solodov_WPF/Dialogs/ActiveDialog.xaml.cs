using ISIP123_Solodov_WPF.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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
    /// Логика взаимодействия для ActiveDialog.xaml
    /// </summary>
    public partial class ActiveDialog : Window
    {
        public ActiveDialog()
        {
            InitializeComponent();
            Loaded += PageLoaded;
        }

        private void RemoveSlot_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.DataContext is CS2Market item)
                {
                    var res = MessageBox.Show("Вы точно хотите снять слот с продажи?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        MessageBox.Show($"Вы сняли {item.Inventories.Items.Name} с продажи", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                        Core.ContextKIP.CS2Market.Remove(item);
                        Core.ContextKIP.SaveChanges();

                        PageLoaded(null, null);
                    }
                }
            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e) 
        {
            var lst = Core.ContextKIP.CS2Market.Where(x => x.SellerID == Core.CurrentUser.ID && x.BuyerID == null).ToList();
            MarketActivityLB.ItemsSource = lst;
        }
    }
}
