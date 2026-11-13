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

namespace ISIP123_Solodov_WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для ProfileDialog.xaml
    /// </summary>
    public partial class ProfileDialog : Window
    {
        private const string GabenLogin = "gaben@valvesoftware.com";

        public ProfileDialog()
        {
            InitializeComponent();
            Loaded += PageLoaded;
        }

        // Маркер 1 = Бан пользователей
        private void BanPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            var AllClearestUsers = Core.Context.Users.Where(u => u.Login != GabenLogin && u.IsBanned == false).ToList();

            var dialog = new GabenDialog(AllClearestUsers, GabenDialogMode.Ban)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        }

        // Маркер 2 = Разбан пользователей
        private void UnBanPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            var AllBannedUsers = Core.Context.Users.Where(u => u.Login != GabenLogin && u.IsBanned == true).ToList();

            var dialog = new GabenDialog(AllBannedUsers, GabenDialogMode.Unban)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        }

        // Маркер 3 = Подарок пользователям
        private void GiveItemToPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            var AllUsers = Core.Context.Users.ToList();

            var dialog = new GabenDialog(AllUsers, GabenDialogMode.GiveItem)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        }

        // Маркер 4 = Просмотр пользователей
        private void CheckUsersBtn_Click(object sender, RoutedEventArgs e)
        {
            var AllUsers = Core.Context.Users.Where(u => u.Login != GabenLogin).ToList();

            var dialog = new GabenDialog(AllUsers, GabenDialogMode.Check)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        }

        private void CapitalizationBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new GabenDialog()
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = e.Uri.AbsoluteUri, UseShellExecute = true });
            e.Handled = true;
        }

        private void TopUpBalance_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser.IsAdmin == true) 
            {
                DefaultUserGrid.Visibility = Visibility.Collapsed;
                GabenTheGodGrid.Visibility = Visibility.Visible;
            }
            if (Core.CurrentUser.IsBanned == true) 
            {
                IsBannedTBlock.Visibility = Visibility.Visible;
            }
            UpdateUI();
        }

        private void UpdateUI()
        {
            LoginTBox.Text = Core.CurrentUser.Login;
            NicknameTBox.Text = Core.CurrentUser.Nickname;
            BalanceTBox.Text = $"{Core.CurrentUser.Balance} ₽"; 
        }
    }
}
