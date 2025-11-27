using PolyPrint2.AppData;
using PolyPrint2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PolyPrint2.View.Windows
{
    public partial class OrderItemsWindow : Window
    {
        #region Поля

        private Orders currentOrder;
        private List<OrderItemGridItem> itemsList;

        #endregion

        #region Инициализация

        public OrderItemsWindow(Orders order)
        {
            InitializeComponent();
            currentOrder = order;
            itemsList = new List<OrderItemGridItem>();

            InitializeEvents();
            LoadData();
        }

        private void InitializeEvents()
        {
            AddItemButton.Click += AddItemButton_Click;
            DeleteItemButton.Click += DeleteItemButton_Click;
            CloseButton.Click += CloseButton_Click;
        }

        #endregion

        #region Загрузка данных

        private void LoadData()
        {
            OrderInfoText.Text = string.Format("Заказ #{0} от {1}",
                currentOrder.ID_Order,
                currentOrder.Order_Date.ToString("dd.MM.yyyy"));

            string clientName = currentOrder.Clients != null ? currentOrder.Clients.Organization_Name : "";
            StatusText.Text = string.Format("Клиент: {0} | Статус: {1}",
                clientName,
                currentOrder.Status);

            List<Equipment> equipment = App.context.Equipment.ToList();
            EquipmentComboBox.ItemsSource = equipment;
            if (equipment.Count > 0)
            {
                EquipmentComboBox.SelectedIndex = 0;
            }

            RefreshItemsGrid();
        }

        private void RefreshItemsGrid()
        {
            List<Order_Items> orderItems = App.context.Order_Items.Where(oi => oi.ID_Order == currentOrder.ID_Order).ToList();

            itemsList = orderItems.Select(oi => new OrderItemGridItem
            {
                ID_Item = oi.ID_Item,
                ID_Equipment = oi.ID_Equipment,
                EquipmentName = oi.Equipment != null ? oi.Equipment.Name : "",
                Quantity = oi.Quantity,
                Price = oi.Price,
                TotalPrice = oi.Quantity * oi.Price
            }).ToList();

            ItemsGrid.ItemsSource = null;
            ItemsGrid.ItemsSource = itemsList;

            RecalculateTotal();
        }

        private void RecalculateTotal()
        {
            decimal total = itemsList.Sum(i => i.TotalPrice);
            TotalText.Text = string.Format("Итого: {0:N2} руб.", total);

            currentOrder.Total = total;
            App.context.SaveChanges();
        }

        #endregion

        #region Добавление позиции

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentComboBox.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите оборудование");
                return;
            }

            int quantity = 0;
            if (!int.TryParse(QuantityBox.Text.Trim(), out quantity) || quantity <= 0)
            {
                NotificationService.ShowWarning("Введите корректное количество");
                return;
            }

            Equipment selectedEquipment = EquipmentComboBox.SelectedItem as Equipment;

            MessageBoxResult result = MessageBoxResult.Yes;
            string priceInput = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите цену за единицу:",
                "Цена оборудования",
                "0",
                -1,
                -1);

            decimal price = 0;
            if (!decimal.TryParse(priceInput, out price) || price < 0)
            {
                NotificationService.ShowWarning("Введите корректную цену");
                return;
            }

            try
            {
                Order_Items newItem = new Order_Items
                {
                    ID_Order = currentOrder.ID_Order,
                    ID_Equipment = selectedEquipment.ID_Equipment,
                    Quantity = quantity,
                    Price = price
                };

                App.context.Order_Items.Add(newItem);
                App.context.SaveChanges();

                NotificationService.ShowSuccess("Позиция добавлена");
                RefreshItemsGrid();
                QuantityBox.Text = "1";
            }
            catch (Exception ex)
            {
                NotificationService.ShowError("Ошибка добавления: " + ex.Message);
            }
        }

        #endregion

        #region Удаление позиции

        private void DeleteItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsGrid.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите позицию для удаления");
                return;
            }

            OrderItemGridItem selectedItem = ItemsGrid.SelectedItem as OrderItemGridItem;

            MessageBoxResult result = NotificationService.ShowConfirmation(
                "Удалить позицию?",
                "Подтверждение удаления"
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Order_Items item = App.context.Order_Items.Find(selectedItem.ID_Item);
                    if (item != null)
                    {
                        App.context.Order_Items.Remove(item);
                        App.context.SaveChanges();
                        NotificationService.ShowSuccess("Позиция удалена");
                        RefreshItemsGrid();
                    }
                }
                catch (Exception ex)
                {
                    NotificationService.ShowError("Ошибка удаления: " + ex.Message);
                }
            }
        }

        #endregion

        #region Закрытие

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        #endregion
    }

    #region Вспомогательный класс

    public class OrderItemGridItem
    {
        public int ID_Item { get; set; }
        public int ID_Equipment { get; set; }
        public string EquipmentName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }

    #endregion
}
