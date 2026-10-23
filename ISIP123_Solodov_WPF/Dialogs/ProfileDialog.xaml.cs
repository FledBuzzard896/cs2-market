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
    /// Логика взаимодействия для ProfileDialog.xaml
    /// </summary>
    public partial class ProfileDialog : Window
    {
        public ProfileDialog()
        {
            InitializeComponent();
            Loaded += PageLoaded;
        }

        private void TopUpBalance_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Core.CurrentUser.Balance += Convert.ToDecimal(1000000.0);

                var newTransaction = new Transactions
                {
                    UserID = Core.CurrentUser.ID,
                    OperationID = 1, // Пополнение
                    Amount = 1000000, 
                    TransactionDate = DateTime.Now, 
                };

                Core.Context.Transactions.Add(newTransaction); 
                Core.Context.SaveChanges();

                // Обновляем интерфейс
                UpdateUI();

                MessageBox.Show("Баланс успешно пополнен на 1 000 000 ₽!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при пополнении баланса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SwitchThemeBtn_Click(object sender, RoutedEventArgs e)
        {
            // Меняем тему на противоположную
            Core.ChangeTheme(!Core.IsDarkTheme);

            MessageBox.Show("Тема успешно изменена! Перезапустите приложение или вернитесь назад, чтобы увидеть изменения.",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close(); // Закрываем профиль, чтобы пользователь увидел новый маркет
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
