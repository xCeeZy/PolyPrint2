using PolyPrint2.AppData;
using PolyPrint2.Model;
using PolyPrint2.View.Windows;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PolyPrint2.View.Pages
{
    public partial class OrdersPage : Page
    {
        #region Поля

        private List<OrderGridItem> allOrders;

        #endregion

        #region Инициализация

        public OrdersPage()
        {
            InitializeComponent();
            InitializeEvents();
            LoadData();
        }

        private void InitializeEvents()
        {
            SearchBox.TextChanged += SearchBox_TextChanged;
            FilterAllButton.Click += FilterAllButton_Click;
            FilterNewButton.Click += FilterNewButton_Click;
            FilterProcessingButton.Click += FilterProcessingButton_Click;
            FilterCompletedButton.Click += FilterCompletedButton_Click;
            FilterCancelledButton.Click += FilterCancelledButton_Click;
            AddButton.Click += AddButton_Click;
            EditButton.Click += EditButton_Click;
            ViewItemsButton.Click += ViewItemsButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            ExportButton.Click += ExportButton_Click;
            OrdersGrid.MouseDoubleClick += OrdersGrid_MouseDoubleClick;
        }

        #endregion

        #region Загрузка данных

        private void LoadData()
        {
            try
            {
                List<Orders> orders = App.context.Orders.ToList();
                allOrders = orders.Select(o => new OrderGridItem
                {
                    ID_Order = o.ID_Order,
                    ClientName = o.Clients != null ? o.Clients.Organization_Name : "",
                    Order_Date = o.Order_Date,
                    Status = o.Status,
                    ItemsCount = o.Order_Items.Count,
                    Total = o.Total
                }).ToList();

                OrdersGrid.ItemsSource = allOrders;
            }
            catch (Exception ex)
            {
                NotificationService.ShowError("Ошибка загрузки данных: " + ex.Message);
                allOrders = new List<OrderGridItem>();
            }
        }

        #endregion

        #region Поиск

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                OrdersGrid.ItemsSource = allOrders;
                return;
            }

            List<OrderGridItem> filtered = allOrders.Where(o =>
                o.ID_Order.ToString().Contains(searchText) ||
                (o.ClientName != null && o.ClientName.ToLower().Contains(searchText)) ||
                (o.Status != null && o.Status.ToLower().Contains(searchText))
            ).ToList();

            OrdersGrid.ItemsSource = filtered;
        }

        #endregion

        #region Фильтры

        private void FilterAllButton_Click(object sender, RoutedEventArgs e)
        {
            OrdersGrid.ItemsSource = allOrders;
            SearchBox.Text = "";
        }

        private void FilterNewButton_Click(object sender, RoutedEventArgs e)
        {
            List<OrderGridItem> filtered = allOrders.Where(o =>
                o.Status != null && o.Status.ToLower() == "новый").ToList();
            OrdersGrid.ItemsSource = filtered;
            SearchBox.Text = "";
        }

        private void FilterProcessingButton_Click(object sender, RoutedEventArgs e)
        {
            List<OrderGridItem> filtered = allOrders.Where(o =>
                o.Status != null && o.Status.ToLower() == "в обработке").ToList();
            OrdersGrid.ItemsSource = filtered;
            SearchBox.Text = "";
        }

        private void FilterCompletedButton_Click(object sender, RoutedEventArgs e)
        {
            List<OrderGridItem> filtered = allOrders.Where(o =>
                o.Status != null && o.Status.ToLower() == "выполнен").ToList();
            OrdersGrid.ItemsSource = filtered;
            SearchBox.Text = "";
        }

        private void FilterCancelledButton_Click(object sender, RoutedEventArgs e)
        {
            List<OrderGridItem> filtered = allOrders.Where(o =>
                o.Status != null && o.Status.ToLower() == "отменён").ToList();
            OrdersGrid.ItemsSource = filtered;
            SearchBox.Text = "";
        }

        #endregion

        #region Добавление

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            EditOrderWindow window = new EditOrderWindow(null);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        #endregion

        #region Редактирование

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите заказ для редактирования");
                return;
            }

            OrderGridItem selectedItem = OrdersGrid.SelectedItem as OrderGridItem;
            Orders order = App.context.Orders.Find(selectedItem.ID_Order);

            if (order == null)
            {
                NotificationService.ShowError("Заказ не найден");
                return;
            }

            EditOrderWindow window = new EditOrderWindow(order);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        #endregion

        #region Просмотр позиций

        private void ViewItemsButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите заказ для просмотра позиций");
                return;
            }

            OrderGridItem selectedItem = OrdersGrid.SelectedItem as OrderGridItem;
            Orders order = App.context.Orders.Find(selectedItem.ID_Order);

            if (order == null)
            {
                NotificationService.ShowError("Заказ не найден");
                return;
            }

            OrderItemsWindow window = new OrderItemsWindow(order);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        #endregion

        #region Удаление

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите заказ для удаления");
                return;
            }

            OrderGridItem selectedItem = OrdersGrid.SelectedItem as OrderGridItem;
            Orders order = App.context.Orders.Find(selectedItem.ID_Order);

            if (order == null)
            {
                NotificationService.ShowError("Заказ не найден");
                return;
            }

            List<Order_Items> relatedItems = App.context.Order_Items.Where(oi => oi.ID_Order == order.ID_Order).ToList();
            string confirmMessage = "Вы уверены, что хотите удалить заказ #" + order.ID_Order + "?";

            if (relatedItems.Count > 0)
            {
                confirmMessage += string.Format("\n\nВНИМАНИЕ: Будут также удалены позиции заказа: {0} шт.", relatedItems.Count);
            }

            MessageBoxResult result = NotificationService.ShowConfirmation(confirmMessage, "Подтверждение удаления");

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (Order_Items item in relatedItems)
                    {
                        App.context.Order_Items.Remove(item);
                    }

                    App.context.Orders.Remove(order);
                    App.context.SaveChanges();
                    NotificationService.ShowSuccess("Заказ успешно удалён");
                    LoadData();
                }
                catch (SqlException ex)
                {
                    NotificationService.ShowError("Ошибка удаления из БД: " + ex.Message);
                }
                catch (Exception ex)
                {
                    NotificationService.ShowError("Ошибка удаления: " + ex.Message);
                }
            }
        }

        #endregion

        #region Экспорт

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            List<OrderGridItem> dataToExport = OrdersGrid.ItemsSource as List<OrderGridItem>;
            if (dataToExport != null)
            {
                ExportService.ExportToCSV(dataToExport, "Заказы.csv");
            }
        }

        #endregion

        #region Двойной клик

        private void OrdersGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditButton_Click(sender, e);
        }

        #endregion
    }
}
