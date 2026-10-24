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
            if (string.IsNullOrEmpty(loginBox.Text.Trim()) || string.IsNullOrEmpty(passwordBox.Password.Trim()))
            {
                MessageBox.Show("Введите логин и пароль.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Защита от двойного клика
            enterBtn.IsEnabled = false;

            try
            {
                var usr = Core.ContextKIP.Users.FirstOrDefault(x => x.Login == loginBox.Text.Trim());

                if (usr == null || usr.Password != passwordBox.Password.Trim())
                {
                    MessageBox.Show("Неверный логин или пароль", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Core.CurrentUser = usr;
                NavigationService.Navigate(new Market());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Включаем кнопку обратно, если мы остались на этой странице (не перешли на маркет)
                enterBtn.IsEnabled = true;
            }
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
