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
        private List<Users> _users;
        private bool _marker;

        public GabenDialog(List<Users> UsersList)
        {
            InitializeComponent();
            _users = UsersList;
        }
        public GabenDialog(bool marker) 
        {
            InitializeComponent();
            _marker = marker;
        }


    }
}
