using ISIP123_Solodov_WPF.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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
    /// Логика взаимодействия для ActiveDialog.xaml
    /// </summary>
    public partial class ActiveDialog : Window
    {
        public ActiveDialog()
        {
            InitializeComponent();
            Loaded += PageLoaded;
        }

        private void RemoveSlot_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is MarketListings item)
            {
                var res = MessageBox.Show("Вы точно хотите снять слот с продажи?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (res == MessageBoxResult.Yes)
                {
                    try
                    {
                        // 1. Меняем статус лота на "Отменён" (3)
                        item.StatusID = 3;

                        // 2. Возвращаем предмет в инвентарь (снимаем флаг продажи)
                        if (item.InventoryItems != null)
                        {
                            item.InventoryItems.IsListed = false;
                        }

                        Core.Context.SaveChanges();

                        // 3. Показываем сообщение после успешного сохранения
                        string itemName = item.InventoryItems?.Items?.Name ?? "Предмет";
                        MessageBox.Show($"Вы сняли {itemName} с продажи", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                        // 4. Обновляем список на экране
                        PageLoaded(null, null);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Произошла ошибка при отмене лота: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            // Получаем активные лоты текущего пользователя.
            var lst = Core.Context.MarketListings
                .Where(x => x.InventoryItems.UserID == Core.CurrentUser.ID && x.StatusID == 1)
                .ToList();

            MarketActivityLB.ItemsSource = lst;
        }
    }
}
