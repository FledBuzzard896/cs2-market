using ISIP123_Solodov_WPF.Dialogs;
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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void enterBtn_Click(object sender, RoutedEventArgs e)
        {
            var usr = Core.ContextKIP.Users.FirstOrDefault(x => x.Login == loginBox.Text);
            if (usr == null)
            {
                MessageBox.Show("Пользователя с таким Login не существует.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (passwordBox.Password != usr.Password)
            {
                MessageBox.Show("Логин или пароль неправильный", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Core.CurrentUser = usr;
            NavigationService.Navigate(new Market());
        }

        private void regBtn_Click(object sender, RoutedEventArgs e)
        {
            RegistrationDialog dialog = new RegistrationDialog();
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                NavigationService.Navigate(new Market());
            }
        }

        private void entryGuestBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Market());
        }

        private void IForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            IForgotPasswordDialog dialog = new IForgotPasswordDialog();
            bool? result = dialog.ShowDialog();

            if (result == true) 
            {
                NavigationService.Navigate(new Market());
            }
        }
    }
}
