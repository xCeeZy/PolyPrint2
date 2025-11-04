using PolyPrint2.AppData;
using PolyPrint2.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace PolyPrint2.View.Pages
{
    public partial class OrdersPage : Page
    {
        #region Инициализация

        public OrdersPage()
        {
            InitializeComponent();
            LoadData();
        }

        #endregion

        #region Загрузка данных

        private void LoadData()
        {
            List<Orders> orders = App.context.Orders.ToList();
            List<OrderGridItem> gridItems = orders.Select(o => new OrderGridItem
            {
                ID_Order = o.ID_Order,
                ClientName = o.Clients != null ? o.Clients.Organization_Name : "",
                Order_Date = o.Order_Date,
                Status = o.Status,
                Total = o.Total
            }).ToList();

            OrdersGrid.ItemsSource = gridItems;
        }

        #endregion
    }
}
