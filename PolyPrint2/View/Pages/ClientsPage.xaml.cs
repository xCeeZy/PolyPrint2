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
    public partial class ClientsPage : Page
    {
        #region Поля

        private List<Clients> allClients;

        #endregion

        #region Инициализация

        public ClientsPage()
        {
            InitializeComponent();
            InitializeEvents();
            LoadData();
        }

        private void InitializeEvents()
        {
            SearchBox.TextChanged += SearchBox_TextChanged;
            AddButton.Click += AddButton_Click;
            EditButton.Click += EditButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            ExportButton.Click += ExportButton_Click;
            ClientsGrid.MouseDoubleClick += ClientsGrid_MouseDoubleClick;
        }

        #endregion

        #region Загрузка данных

        private void LoadData()
        {
            try
            {
                allClients = App.context.Clients.ToList();
                ClientsGrid.ItemsSource = allClients;
            }
            catch (Exception ex)
            {
                NotificationService.ShowError("Ошибка загрузки данных: " + ex.Message);
                allClients = new List<Clients>();
            }
        }

        #endregion

        #region Поиск

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                ClientsGrid.ItemsSource = allClients;
                return;
            }

            List<Clients> filtered = allClients.Where(c =>
                c.Organization_Name.ToLower().Contains(searchText) ||
                (c.Contact_Name != null && c.Contact_Name.ToLower().Contains(searchText)) ||
                (c.Phone != null && c.Phone.Contains(searchText)) ||
                (c.Email != null && c.Email.ToLower().Contains(searchText))
            ).ToList();

            ClientsGrid.ItemsSource = filtered;
        }

        #endregion

        #region Добавление

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            EditClientWindow window = new EditClientWindow(null);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        #endregion

        #region Редактирование

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите клиента для редактирования");
                return;
            }

            Clients selectedClient = ClientsGrid.SelectedItem as Clients;
            EditClientWindow window = new EditClientWindow(selectedClient);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        #endregion

        #region Удаление

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите клиента для удаления");
                return;
            }

            Clients selectedClient = ClientsGrid.SelectedItem as Clients;

            int equipmentCount = App.context.Equipment.Count(eq => eq.ID_Client == selectedClient.ID_Client);
            int requestsCount = App.context.Service_Requests.Count(r => r.Equipment.ID_Client == selectedClient.ID_Client);

            string confirmMessage = "Вы уверены, что хотите удалить клиента " + selectedClient.Organization_Name + "?";

            if (equipmentCount > 0 || requestsCount > 0)
            {
                confirmMessage += "\n\nВНИМАНИЕ: Будут также удалены связанные данные:";
                if (equipmentCount > 0)
                {
                    confirmMessage += string.Format("\n• Оборудование: {0} шт.", equipmentCount);
                }
                if (requestsCount > 0)
                {
                    confirmMessage += string.Format("\n• Заявки: {0} шт.", requestsCount);
                }
            }

            MessageBoxResult result = NotificationService.ShowConfirmation(confirmMessage, "Подтверждение удаления");

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    List<Equipment> clientEquipment = App.context.Equipment.Where(eq => eq.ID_Client == selectedClient.ID_Client).ToList();

                    foreach (Equipment equipment in clientEquipment)
                    {
                        List<Service_Requests> equipmentRequests = App.context.Service_Requests.Where(r => r.ID_Equipment == equipment.ID_Equipment).ToList();

                        foreach (Service_Requests request in equipmentRequests)
                        {
                            List<Works> requestWorks = App.context.Works.Where(w => w.ID_Request == request.ID_Request).ToList();

                            foreach (Works work in requestWorks)
                            {
                                List<Used_Parts> usedParts = App.context.Used_Parts.Where(up => up.ID_Work == work.ID_Work).ToList();
                                foreach (Used_Parts usedPart in usedParts)
                                {
                                    App.context.Used_Parts.Remove(usedPart);
                                }
                                App.context.Works.Remove(work);
                            }

                            App.context.Service_Requests.Remove(request);
                        }

                        App.context.Equipment.Remove(equipment);
                    }

                    App.context.Clients.Remove(selectedClient);
                    App.context.SaveChanges();
                    NotificationService.ShowSuccess("Клиент успешно удалён");
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
            List<Clients> dataToExport = ClientsGrid.ItemsSource as List<Clients>;
            if (dataToExport != null)
            {
                ExportService.ExportToCSV(dataToExport, "Клиенты.csv");
            }
        }

        #endregion

        #region Двойной клик

        private void ClientsGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditButton_Click(sender, e);
        }

        #endregion
    }
}
