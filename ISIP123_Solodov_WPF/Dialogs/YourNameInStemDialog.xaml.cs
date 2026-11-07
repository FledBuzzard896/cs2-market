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
            Regex regex = new Regex("[^0-9]+");
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
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        // Блокирует ввод пробела
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e) 
        {
            if (e.Key == Key.Space) 
            {
                e.Handled = true;
            }
        }


        private void CardNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Отключаем обработчик, чтобы избежать бесконечной рекурсии при изменении текста кодом
            CardNumber.TextChanged -= CardNumber_TextChanged;

            string digits = CardNumber.Text.Replace(" ", "");

            string formatted = "";
            for (int i = 0; i < digits.Length; i++)
            {
                if (i > 0 && i % 4 == 0)
                {
                    formatted += " ";
                }
                formatted += digits[i];
            }

            CardNumber.Text = formatted;
            CardNumber.CaretIndex = CardNumber.Text.Length;

            // Включаем обработчик обратно
            CardNumber.TextChanged += CardNumber_TextChanged;
        }
    }
}
