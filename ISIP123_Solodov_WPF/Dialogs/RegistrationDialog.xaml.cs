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
using System.Windows.Shapes;

namespace ISIP123_Solodov_WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для RegistrationDialog.xaml
    /// </summary>
    public partial class RegistrationDialog : Window
    {
        public RegistrationDialog()
        {
            InitializeComponent();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void RegBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = NewUserLoginTB.Text.Trim();
            string password1 = NewUserPasswordF.Password;
            string password2 = NewUserPasswordS.Password;
            string nickname = NickNameTB.Text.Trim();

            // 1. Проверка логина
            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Заполните поле Логин.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Проверка на существующего пользователя
            if (Core.ContextKIP.Users.Any(x => x.Login == login))
            {
                MessageBox.Show("Пользователь с таким Login уже существует.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. Проверка паролей
            if (string.IsNullOrEmpty(password1) || string.IsNullOrEmpty(password2))
            {
                MessageBox.Show("Оба поля с паролями должны быть заполнены.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password1 != password2)
            {
                MessageBox.Show("Пароли не совпадают.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 4. Проверка никнейма
            if (string.IsNullOrEmpty(nickname))
            {
                MessageBox.Show("Заполните поле Никнейм.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 5. Регистрация
            try
            {
                Users newUser = new Users
                {
                    Login = login,
                    Password = password1,
                    Nickname = nickname,
                    Balance = 0,
                };

                Core.ContextKIP.Users.Add(newUser);
                Core.ContextKIP.SaveChanges();

                Core.CurrentUser = newUser;
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
