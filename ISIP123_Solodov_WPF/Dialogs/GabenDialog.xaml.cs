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
    /// Логика взаимодействия для GabenDialog.xaml
    /// </summary>
    public partial class GabenDialog : Window
    {
        private List<Users> _users = null;
        private GabenDialogMode _mode;  
        public string ButtonText { get; private set; }


        public GabenDialog() 
        {
            InitializeComponent();
            Loaded += PageLoaded;
        }
        public GabenDialog(List<Users> UsersList, GabenDialogMode mode)
        {
            InitializeComponent();

            _users = UsersList;
            _mode = mode;

            Loaded += PageLoaded;
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем пользователя, на чьей строке нажали кнопку
            if (sender is Button button && button.Tag is Users selectedUser)
            {
                switch (_mode)
                {
                    case GabenDialogMode.Ban:
                        selectedUser.IsBanned = true;
                        Core.Context.SaveChanges();
                        // Опционально: удалить из визуального списка, так как он больше не подходит под фильтр
                        _users.Remove(selectedUser);
                        MainListBox.Items.Refresh();
                        break;

                    case GabenDialogMode.Unban:
                        selectedUser.IsBanned = false;
                        Core.Context.SaveChanges();
                        _users.Remove(selectedUser);
                        MainListBox.Items.Refresh();
                        break;

                    case GabenDialogMode.GiveItem:
                        // Логика выдачи предмета (например, вызов другого окна)
                        MessageBox.Show($"Выдан предмет для {selectedUser.Login}");
                        break;

                    case GabenDialogMode.Check:
                        // Логика проверки (например, открытие профиля)
                        MessageBox.Show($"Проверка {selectedUser.Login}");
                        break;
                }
            }
        }

        /// <summary>
        /// Подгрузка страницы.
        /// <para>1 - Вывод пользователей на Бан</para> 
        /// <para>2 - Вывод пользователей на Разбан</para> 
        /// <para>3 - Отправить подарок от Габена</para> 
        /// <para>4 - Вывод всех пользователей</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageLoaded(object sender, RoutedEventArgs e) 
        {
            if (_users == null)
            {
                // Подгрузка капитализации
            }
            else 
            {
                switch (_mode)
                {
                    case GabenDialogMode.Ban:
                        TitleTBlock.Text = "ЗАБАНИТЬ ИГРОКА:";
                        ButtonText = "Забанить";
                        break;
                    case GabenDialogMode.Unban:
                        TitleTBlock.Text = "РАЗЗАБАНИТЬ ИГРОКА:";
                        ButtonText = "Разбанить";
                        break;
                    case GabenDialogMode.GiveItem:
                        TitleTBlock.Text = "ВЫДАТЬ ПРЕДМЕТ:";
                        ButtonText = "Выдать предмет";
                        break;
                    case GabenDialogMode.Check:
                        //ButtonText = "Профиль";
                        break;
                    default: break;
                }

                MainListBox.ItemsSource = _users;
            }
        }
    }
}
