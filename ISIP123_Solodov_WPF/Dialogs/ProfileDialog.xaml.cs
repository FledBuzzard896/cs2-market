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
            Core.CurrentUser.Balance = 1000000;
            Core.Context.SaveChanges();

            PageLoaded(null, null);
        }

        private void PageLoaded(object sender, RoutedEventArgs e) 
        {
            LoginTBox.Text = Core.CurrentUser.Login;
            NicknameTBox.Text = Core.CurrentUser.Nickname;
            BalanceTBox.Text = Core.CurrentUser.Balance.ToString();
        }
    }
}
