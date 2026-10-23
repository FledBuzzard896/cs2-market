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
    public partial class IForgotPasswordDialog : Window
    {
        public IForgotPasswordDialog()
        {
            InitializeComponent();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void changePasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверка на пустой логин
            if (string.IsNullOrWhiteSpace(LoginBox.Text))
            {
                MessageBox.Show("Заполните поле Логин.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Ищем пользователя
            var usr = Core.Context.Users.FirstOrDefault(x => x.Login == LoginBox.Text);

            if (usr == null)
            {
                MessageBox.Show("Пользователь с таким логином не найден.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. Проверка на пустые пароли
            if (string.IsNullOrWhiteSpace(firstPassBox.Password) || string.IsNullOrWhiteSpace(secondPassBox.Password))
            {
                MessageBox.Show("Заполните поле Пароль.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 4. Проверка на совпадение паролей
            if (firstPassBox.Password != secondPassBox.Password)
            {
                MessageBox.Show("Пароли не совпадают.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 5. Сохраняем изменения
            try
            {
                usr.Password = firstPassBox.Password;
                Core.Context.SaveChanges();

                Core.CurrentUser = usr;

                MessageBox.Show("Пароль изменён.", "Проходите", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
