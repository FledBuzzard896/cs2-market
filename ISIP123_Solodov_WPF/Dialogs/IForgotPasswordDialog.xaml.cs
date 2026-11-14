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
                MessageBox.Show(Application.Current.MainWindow, "Заполните поле Логин.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Ищем пользователя
            var usr = Core.ContextKIP.Users.FirstOrDefault(x => x.Login == LoginBox.Text);

            if (usr == null)
            {
                MessageBox.Show(Application.Current.MainWindow, "Пользователь с таким логином не найден.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. Проверка на пустые пароли
            if (string.IsNullOrWhiteSpace(firstPassBox.Password) || string.IsNullOrWhiteSpace(secondPassBox.Password))
            {
                MessageBox.Show(Application.Current.MainWindow, "Заполните поле Пароль.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 4. Проверка на короткий пароль
            if (firstPassBox.Password.Length < 6) 
            {
                MessageBox.Show(Application.Current.MainWindow, "Пароль должен быть от 6ти символов.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 5. Проверка на сложность пароля
            if (!firstPassBox.Password.Any(char.IsDigit)) 
            {
                MessageBox.Show(Application.Current.MainWindow, "Пароль должен содержать цифры.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 6. Проверка на совпадение паролей
            if (firstPassBox.Password != secondPassBox.Password)
            {
                MessageBox.Show(Application.Current.MainWindow, "Пароли не совпадают.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 7. Проверка изменения на точно такой же пароль
            string newHash = Core.HashPassword(firstPassBox.Password);
            if (usr.Password == newHash) 
            {
                MessageBox.Show(Application.Current.MainWindow, "Пароль совпадает со старым.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 8. Сохраняем изменения
            try
            {
                usr.Password = newHash;
                Core.ContextKIP.SaveChanges();

                Core.CurrentUser = usr;

                MessageBox.Show(Application.Current.MainWindow, "Пароль изменён.", "Проходите", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(Application.Current.MainWindow, $"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
