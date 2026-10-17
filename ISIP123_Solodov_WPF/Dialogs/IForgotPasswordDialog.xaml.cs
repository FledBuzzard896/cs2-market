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

namespace ISIP123_Solodov_WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для IForgotPasswordDialog.xaml
    /// </summary>
    public partial class IForgotPasswordDialog : Window
    {
        public IForgotPasswordDialog()
        {
            InitializeComponent();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void changePasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LoginBox.Text))
            {
                MessageBox.Show("Заполните поле Логин.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (Core.ContextKIP.Users.FirstOrDefault(x => x.Login == LoginBox.Text) is null) 
            {
                MessageBox.Show("Пользователь с таким логином не найден.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                var usr = Core.ContextKIP.Users.FirstOrDefault(x => x.Login == LoginBox.Text);

                if (string.IsNullOrEmpty(firstPassBox.Password) || string.IsNullOrEmpty(secondPassBox.Password))
                {
                    MessageBox.Show("Заполните поле Пароль.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    if (firstPassBox.Password != secondPassBox.Password)
                    {
                        MessageBox.Show("Пароли не совпадают.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    usr.Password = firstPassBox.Password;
                    Core.ContextKIP.SaveChanges();

                    Core.CurrentUser = usr;

                    MessageBox.Show("Пароль изменён.", "Проходите", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
            }
        }
    }
}
