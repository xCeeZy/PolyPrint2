using PolyPrint2.AppData;
using PolyPrint2.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PolyPrint2.View.Pages
{
    public partial class ServiceRequestsPage : Page
    {
        #region Поля

        private List<ServiceRequestGridItem> allRequests;

        #endregion

        #region Инициализация

        public ServiceRequestsPage()
        {
            InitializeComponent();
            InitializeEvents();
            LoadData();
        }

        private void InitializeEvents()
        {
            SearchBox.TextChanged += SearchBox_TextChanged;
            AddButton.Click += AddButton_Click;
        }

        #endregion

        #region Загрузка данных

        private void LoadData()
        {
            List<Service_Requests> requests = App.context.Service_Requests.ToList();

            if (AuthService.IsMaster(App.CurrentUser))
            {
                requests = requests.Where(r => r.ID_Master == App.CurrentUser.ID_User).ToList();
            }

            allRequests = requests.Select(r => new ServiceRequestGridItem
            {
                ID_Request = r.ID_Request,
                ClientName = r.Clients != null ? r.Clients.Organization_Name : "",
                EquipmentName = r.Equipment != null ? r.Equipment.Name : "",
                Created_Date = r.Created_Date,
                Problem_Description = r.Problem_Description,
                Status = r.Status,
                MasterName = r.Users != null ? r.Users.Login : ""
            }).ToList();

            RequestsGrid.ItemsSource = allRequests;
        }

        #endregion

        #region Поиск

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                RequestsGrid.ItemsSource = allRequests;
                return;
            }

            List<ServiceRequestGridItem> filtered = allRequests.Where(r =>
                (r.ClientName != null && r.ClientName.ToLower().Contains(searchText)) ||
                (r.EquipmentName != null && r.EquipmentName.ToLower().Contains(searchText)) ||
                (r.Problem_Description != null && r.Problem_Description.ToLower().Contains(searchText)) ||
                (r.Status != null && r.Status.ToLower().Contains(searchText))
            ).ToList();

            RequestsGrid.ItemsSource = filtered;
        }

        #endregion

        #region Добавление

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NotificationService.ShowInfo("Функция добавления заявок в разработке");
        }

        #endregion
    }
}
