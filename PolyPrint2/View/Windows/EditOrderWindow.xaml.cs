using PolyPrint2.AppData;
using PolyPrint2.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace PolyPrint2.View.Windows
{
    public partial class EditOrderWindow : Window
    {
        #region Поля

        private Orders currentOrder;
        private bool isEditMode;

        #endregion

        #region Инициализация

        public EditOrderWindow(Orders order)
        {
            InitializeComponent();
            currentOrder = order;
            isEditMode = order != null;

            InitializeEvents();
            LoadData();
        }

        private void InitializeEvents()
        {
            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        #endregion

        #region Загрузка данных

        private void LoadData()
        {
            List<Clients> clients = App.context.Clients.ToList();
            ClientComboBox.ItemsSource = clients;

            StatusComboBox.ItemsSource = new List<string> { "Новый", "В обработке", "Выполнен", "Отменён" };

            if (isEditMode)
            {
                TitleText.Text = "Редактирование заказа";
                ClientComboBox.SelectedItem = currentOrder.Clients;
                OrderDatePicker.SelectedDate = currentOrder.Order_Date;
                StatusComboBox.SelectedItem = currentOrder.Status;
            }
            else
            {
                TitleText.Text = "Добавление заказа";
                OrderDatePicker.SelectedDate = DateTime.Now;
                StatusComboBox.SelectedIndex = 0;
            }
        }

        #endregion

        #region Сохранение

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientComboBox.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите клиента");
                return;
            }

            if (OrderDatePicker.SelectedDate == null)
            {
                NotificationService.ShowWarning("Выберите дату заказа");
                return;
            }

            if (StatusComboBox.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите статус");
                return;
            }

            Clients selectedClient = ClientComboBox.SelectedItem as Clients;
            DateTime orderDate = OrderDatePicker.SelectedDate.Value;
            string status = StatusComboBox.SelectedItem.ToString();

            try
            {
                if (isEditMode)
                {
                    currentOrder.ID_Client = selectedClient.ID_Client;
                    currentOrder.Order_Date = orderDate;
                    currentOrder.Status = status;
                }
                else
                {
                    Orders newOrder = new Orders
                    {
                        ID_Client = selectedClient.ID_Client,
                        Order_Date = orderDate,
                        Status = status,
                        Total = 0
                    };
                    App.context.Orders.Add(newOrder);
                }

                App.context.SaveChanges();

                if (isEditMode)
                {
                    NotificationService.ShowSuccess("Заказ успешно обновлён");
                }
                else
                {
                    NotificationService.ShowSuccess("Заказ успешно добавлен");
                }

                DialogResult = true;
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessage = DbHelper.GetValidationErrorMessage(ex);
                NotificationService.ShowError(errorMessage);
            }
            catch (SqlException ex)
            {
                NotificationService.ShowError("Ошибка БД: " + ex.Message);
            }
            catch (Exception ex)
            {
                NotificationService.ShowError("Ошибка сохранения: " + ex.Message);
            }
        }

        #endregion

        #region Отмена

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion
    }
}
