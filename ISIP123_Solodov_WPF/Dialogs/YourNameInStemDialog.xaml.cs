using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для YourNameInStemDialog.xaml
    /// </summary>
    public partial class YourNameInStemDialog : Window
    {
        public YourNameInStemDialog()
        {
            InitializeComponent();
        }

        private void TopUpAcc_Click(object sender, RoutedEventArgs e)
        {

        }

        // Блокирует ввод букв с клавиатуры
        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Регулярное выражение: [^0-9] означает "всё, кроме цифр"
            Regex regex = new Regex("[^0-9]+");

            // Если вводимый символ НЕ цифра, отменяем событие ввода
            e.Handled = regex.IsMatch(e.Text);
        }

        // Блокирует вставку текста с буквами через Ctrl+V
        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                Regex regex = new Regex("[^0-9]+");

                if (regex.IsMatch(text))
                {
                    e.CancelCommand(); // Отменяем вставку, если там есть буквы
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
