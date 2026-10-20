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
            this.Close();
        }
        private void RegBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NewUserLoginTB.Text))
            {
                MessageBox.Show("Заполните поле Логин.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else 
            {
                if (Core.Context.Users.Where(x => x.Login == NewUserLoginTB.Text.Trim()).Count() != 0)
                {
                    MessageBox.Show("Пользователь с таким Login уже существует.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else 
                {
                    if (string.IsNullOrEmpty(NewUserPasswordF.Password) || string.IsNullOrEmpty(NewUserPasswordS.Password))
                    {
                        MessageBox.Show("Оба поля с паролями должны быть заполнены.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else if (NewUserPasswordF.Password != NewUserPasswordS.Password)
                    {
                        MessageBox.Show("Пароли не совпадают.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else 
                    {
                        if (string.IsNullOrEmpty(NickNameTB.Text)) 
                        {
                            MessageBox.Show("Заполните поле Никнейм.", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        Users newUser = new Users 
                        {
                            Login = NewUserLoginTB.Text.Trim(),
                            Password = NewUserPasswordF.Password,
                            Nickname = NickNameTB.Text.Trim(),
                            Balance = 0,
                        };

                        Core.Context.Users.Add(newUser);
                        Core.Context.SaveChanges();

                        Core.CurrentUser = newUser;
                        this.DialogResult = true;
                    }
                }
            }
        }
    }
}
