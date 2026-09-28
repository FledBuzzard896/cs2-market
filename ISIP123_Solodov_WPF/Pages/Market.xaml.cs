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

            var market = Core.ContextKIP.CS2Market.ToList();
            ItemsLB.ItemsSource = market;
        }

        private void sellItemBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void shopBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void communityBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void informationBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void supportBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;

                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = UIElement.MouseWheelEvent,
                    Source = sender
                };

                // 1. Приводим к FrameworkElement (у него есть .Parent)
                // 2. Используем safe-cast к UIElement для вызова RaiseEvent
                var parent = ((FrameworkElement)sender).Parent as UIElement;

                parent?.RaiseEvent(eventArg);
            }
        }
    }
}
