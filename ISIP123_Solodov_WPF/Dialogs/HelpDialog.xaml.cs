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
    /// Логика взаимодействия для HelpDalog.xaml
    /// </summary>
    public partial class HelpDialog : Window
    {
        private string text = 
            "Чтобы перейти на страницу профиля:\n" +
            "\tНажмите на ваш Nickname в правом верхнем углу.\n\n" +
            "Чтобы перейти на страницу инвентаря:\n" +
            "\tНажмите на кнопку \"Продать предмет\".\n\n" +
            "Чтобы вернуться на ТП из страницы инвентаря:\n" +
            "\tНажмите на кнопку \"Маркет\" или на кнопку \"Назад\".\n\n" +
            "Чтобы пополнить баланс Stem:\n" +
            "\tПерейдите в профиль.";
        public HelpDialog()
        {
            InitializeComponent();
            MainText.Text = text;
        }
    }
}
