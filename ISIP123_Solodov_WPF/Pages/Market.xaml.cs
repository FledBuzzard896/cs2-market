using ISIP123_Solodov_WPF.Dialogs;
using ISIP123_Solodov_WPF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private void sellItemBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser is null) 
            {
                MessageBox.Show("Вы должны зарегистрироваться, чтобы зайти в Инвентарь.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            NavigationService.Navigate(new InventoryPage());
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

        private void communityBtn_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://steamcommunity.com/market/"; 

            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true 
            });
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


        // Настройка для глобального скрола по ListBox
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
            // Подгрузка ТП
            var market = Core.Context.CS2Market.Where(x => x.StatusID == 1).ToList();  
            ItemsLB.ItemsSource = market;


            // Подгрузка фильтров
            var types = Core.Context.Types.Select(x => x.Name).ToList();
            types.Insert(0, "");
            TypesCombo.ItemsSource = types;

            var qualities = Core.Context.Qualities.Select(x => x.Name).ToList();
            qualities.Insert(0, "");
            QualitesCombo.ItemsSource = qualities;


            if (Core.CurrentUser is null)
            {
                NicknameTBox.Text = "guest";
                balance.Text = $"Баланс кошелька: 0 ₽";
            }
            else 
            {
                NicknameTBox.Text = Core.CurrentUser.Nickname;
                balance.Text = $"Баланс кошелька: {Core.CurrentUser.Balance.ToString()} ₽";
            }
        }
    }
}
