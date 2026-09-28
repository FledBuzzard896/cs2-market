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
    /// Логика взаимодействия для Market.xaml
    /// </summary>
    public partial class Market : Page
    {
        public Market()
        {
            InitializeComponent();
            Loaded += PageLoaded;
        }

        private void listingsBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void historyBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            //var market = Core.Context.CS2Market.ToList();
            //ItemsLB.ItemsSource = market;
        }
    }
}
