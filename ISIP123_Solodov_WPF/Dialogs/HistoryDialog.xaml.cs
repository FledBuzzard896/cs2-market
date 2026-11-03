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
    /// Логика взаимодействия для HistoryDialog.xaml
    /// </summary>
    public partial class HistoryDialog : Window
    {
        public HistoryDialog()
        {
            InitializeComponent();
            Loaded += PageLoaded;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            var marketHistory = Core.Context.MarketListings
                .Where(x => x.BuyerID == Core.CurrentUser.ID || (x.SellerID == Core.CurrentUser.ID && x.BuyerID != null))
                .ToList()
                .Select(x => new
                {
                    OperationSign = x.SellerID == Core.CurrentUser.ID ? "-" : "+    ",
                    ItemName = x.InventoryItems?.Items?.Name ?? "Неизвестный предмет",
                    CreatedAt = x.CreatedAt,
                    SoldAt = x.SoldAt,
                    Price = x.Price
                })
                .OrderByDescending(x => x.SoldAt ?? x.CreatedAt)
                .ToList();

            MarketHistoryLB.ItemsSource = marketHistory;
        }
    }
}
