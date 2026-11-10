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
        public ProfileDialog()
        {
            InitializeComponent();
            Loaded += PageLoaded;
        }

        private void BanPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            var AllClearestUsers = Core.Context.Users.Where(u => u.Login != "gaben@valvesoftware.com" && u.IsBanned == false).ToList();

        }

        private void UnBanPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            var AllBannedUsers = Core.Context.Users.Where(u => u.Login != "gaben@valvesoftware.com" && u.IsBanned == true).ToList();

        }

        private void GiveItemToPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            var AllUsers = Core.Context.Users.ToList();

        }

        private void CheckUsersBtn_Click(object sender, RoutedEventArgs e)
        {
            var AllUsers = Core.Context.Users.Where(u => u.Login != "gaben@valvesoftware.com").ToList();


        }

        private void CapitalizationBtn_Click(object sender, RoutedEventArgs e)
        {

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
