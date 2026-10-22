using ISIP123_Solodov_WPF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// <summary>
    /// Логика взаимодействия для TransactionConfirmation.xaml
    /// </summary>
    public partial class TransactionConfirmation : Window
    {
        private CS2Market _item;
        private int _count;
        private double _profit_without_comission, _consumption_with_comission;

        public TransactionConfirmation(CS2Market item, int count)
        {
            InitializeComponent();

            _item = item;
            _count = count;

            Loaded += PageLoaded;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SSA.IsChecked != true)
            {
                MessageBox.Show("Примите соглашение Stem", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Core.CurrentUser.Balance < Convert.ToDecimal(_consumption_with_comission)) 
            {
                MessageBox.Show("У вас недостаточно средств, чтобы купить данный предмет", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создание записи предмета у покупателя
            Inventories newItem = new Inventories
            {
                UserID = Core.CurrentUser.ID,
                ItemID = _item.Inventories.ItemID,
                Flot = _item.Inventories.Flot,
                Count = _count
            };
            Core.Context.Inventories.Add(newItem);
            Core.Context.SaveChanges();

            // Добавляем кэш продавцу
            _item.Users1.Balance += Convert.ToDecimal(_profit_without_comission);

            // Вычитаем кэш у покупателя
            Core.CurrentUser.Balance -= Convert.ToDecimal(_consumption_with_comission);

            // Удаление записи предмета у продавца
            var oldItem = Core.Context.Inventories.FirstOrDefault(x => x.ID == _item.InventoryID);
            Core.Context.Inventories.Remove(oldItem);

            // Изменяем запись в истории ТП как проведенную сделку
            _item.StatusID = 2;
            _item.BuyerID = Core.CurrentUser.ID;
            _item.DateOfDeal = DateTime.Now;

            // Изменяем инвентарь продавца на инвентарь покупателя в истории ТП
            _item.InventoryID = newItem.ID;

            // Добавляем транзакцию как покупку для покуптеля
            var BuyerTransaction = new TransactionsHistory 
            {
                UserID = Core.CurrentUser.ID,
                OperationID = 2, // покупка
                TransactionSum = Convert.ToDecimal(_consumption_with_comission),
                DateOfTransaction = DateTime.Now,
            };
            Core.Context.TransactionsHistory.Add(BuyerTransaction);

            // Добавляем транзакцию как продажу для продавцы
            var SellerTransaction = new TransactionsHistory 
            {
                UserID = _item.SellerID,
                OperationID = 3, // продажа
                TransactionSum = Convert.ToDecimal(_profit_without_comission),
                DateOfTransaction= DateTime.Now,
            };
            Core.Context.TransactionsHistory.Add(SellerTransaction);

            // Сохраняем изменения
            Core.Context.SaveChanges();

            MessageBox.Show("Предмет успешно куплен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }


        // Открытие ссылки на ресурс
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });

            e.Handled = true;
        }


        private void PageLoaded(object sender, RoutedEventArgs e) 
        {
            double total = 0;

            MainGrid.DataContext = _item;

            // Поиск цены скина (стоимости при последней продаже)
            var list = Core.Context.CS2Market.Where(x => x.StatusID == 2).ToList();
            int i = list.Count() - 1;
            while (i >= 0)
            {
                var str = list[i];

                if (str.Inventories.Items.ID == _item.Inventories.Items.ID)
                {
                    Price.Text = $"{str.Price.ToString()} руб.";
                    Price_2.Text = $"{str.Price.ToString()} руб.";

                    total += Convert.ToDouble(str.Price);

                    break;
                }

                i--;
            }

            // Исключение: базовая цена скина (на ТП его никто не продавал)
            if (Price.Text == "тест")
            {
                Price.Text = $"{_item.Inventories.Items.AveragePrice.ToString()} руб.";
                Price_2.Text = $"{_item.Inventories.Items.AveragePrice.ToString()} руб.";

                total += Convert.ToDouble(_item.Inventories.Items.AveragePrice);
            }

            _profit_without_comission = total;

            total += Convert.ToDouble(_item.Commission);
            TotalSum.Text = $"{total} руб.";

            _consumption_with_comission = total;

            LoginTextBlock.Text = Core.CurrentUser.Login;
        }
    }
}

