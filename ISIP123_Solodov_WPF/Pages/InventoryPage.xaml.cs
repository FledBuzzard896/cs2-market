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
    /// Логика взаимодействия для InventoryPage.xaml
    /// </summary>
    public partial class InventoryPage : Page
    {
        public InventoryPage()
        {
            InitializeComponent();
            Loaded += PageLoaded;
        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.CurrentUser = null;
            NavigationService.Navigate(new AuthPage());
        }

        private void ShowError_Dialog(object sender, RoutedEventArgs e)
        {
            Error dialog = new Error();
            dialog.ShowDialog();
        }

        private void MarketBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Market());
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

        private void PageLoaded(object sender, RoutedEventArgs e) 
        {
            NicknameTBlock.Text = Core.CurrentUser.Nickname;
            NicknameTBlock_2.Text = Core.CurrentUser.Nickname;

            var types = Core.Context.Types.Select(x => x.Name).ToList();
            types.Insert(0, "");
            TypesCombo.ItemsSource = types;

            var qualities = Core.Context.Qualities.Select(x => x.Name).ToList();
            qualities.Insert(0, "");
            QualitesCombo.ItemsSource = qualities;
            
            var items = Core.Context.Inventories.Where(x => x.UserID == Core.CurrentUser.ID).ToList();
            var itemsCopy = new List<Inventories>();

            foreach (var item in items.ToList())
            {
                itemsCopy.Add(item);
                if (item.Count > 1)
                {
                    for (int i = 1; i < item.Count; i++)
                    {
                        itemsCopy.Add(item);
                    }
                }
            }
            
            InventoryItems.ItemsSource = itemsCopy;

            if (itemsCopy.Count > 0)
            {
                ItemCardGrid.Visibility = Visibility.Visible;
                ItemCardGrid.DataContext = itemsCopy[0];

                double? flot = itemsCopy[0].Flot;
                foreach (var str in Core.Context.Iznos.ToList())
                {
                    if (str.MinFlot <= flot && flot < str.MaxFlot)
                    {
                        IznosTB.Text = $"Состояние: {str.Name}";
                    }
                }

                var list = Core.Context.CS2Market.Where(x => x.StatusID == 2).ToList();
                int i = list.Count() - 1;
                while (i != 0)
                {
                    var str = list[i];

                    if (str.Inventories.Items.ID == itemsCopy[0].Items.ID)
                    {
                        StartPrice.Text = $"Начальная цена: {str.Price.ToString()} руб.";
                        break;
                    }

                    i--;
                }
            }
        }
    }
}
