using PolyPrint2.AppData;
using PolyPrint2.Model;
using PolyPrint2.View.Windows;
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
            FilterAllButton.Click += FilterAllButton_Click;
            FilterNewButton.Click += FilterNewButton_Click;
            FilterInProgressButton.Click += FilterInProgressButton_Click;
            FilterCompletedButton.Click += FilterCompletedButton_Click;
            AddButton.Click += AddButton_Click;
            EditButton.Click += EditButton_Click;
            ViewWorksButton.Click += ViewWorksButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            ExportButton.Click += ExportButton_Click;
            RequestsGrid.MouseDoubleClick += RequestsGrid_MouseDoubleClick;
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

        #region Фильтры

        private void FilterAllButton_Click(object sender, RoutedEventArgs e)
        {
            RequestsGrid.ItemsSource = allRequests;
            SearchBox.Text = "";
        }

        private void FilterNewButton_Click(object sender, RoutedEventArgs e)
        {
            List<ServiceRequestGridItem> filtered = allRequests.Where(r =>
                r.Status != null && r.Status.ToLower() == "новая").ToList();
            RequestsGrid.ItemsSource = filtered;
            SearchBox.Text = "";
        }

        private void FilterInProgressButton_Click(object sender, RoutedEventArgs e)
        {
            List<ServiceRequestGridItem> filtered = allRequests.Where(r =>
                r.Status != null && r.Status.ToLower() == "в работе").ToList();
            RequestsGrid.ItemsSource = filtered;
            SearchBox.Text = "";
        }

        private void FilterCompletedButton_Click(object sender, RoutedEventArgs e)
        {
            List<ServiceRequestGridItem> filtered = allRequests.Where(r =>
                r.Status != null && (r.Status.ToLower() == "выполнена" || r.Status.ToLower() == "закрыта")).ToList();
            RequestsGrid.ItemsSource = filtered;
            SearchBox.Text = "";
        }

        #endregion

        #region Добавление

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            EditServiceRequestWindow window = new EditServiceRequestWindow(null);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        #endregion

        #region Редактирование

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (RequestsGrid.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите заявку для редактирования");
                return;
            }

            ServiceRequestGridItem selectedItem = RequestsGrid.SelectedItem as ServiceRequestGridItem;
            Service_Requests request = App.context.Service_Requests.Find(selectedItem.ID_Request);

            if (request == null)
            {
                NotificationService.ShowError("Заявка не найдена");
                return;
            }

            EditServiceRequestWindow window = new EditServiceRequestWindow(request);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        #endregion

        #region Просмотр работ

        private void ViewWorksButton_Click(object sender, RoutedEventArgs e)
        {
            if (RequestsGrid.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите заявку");
                return;
            }

            ServiceRequestGridItem selectedItem = RequestsGrid.SelectedItem as ServiceRequestGridItem;
            Service_Requests request = App.context.Service_Requests.Find(selectedItem.ID_Request);

            if (request == null)
            {
                NotificationService.ShowError("Заявка не найдена");
                return;
            }

            WorksWindow window = new WorksWindow(request);
            window.ShowDialog();
        }

        #endregion

        #region Удаление

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (RequestsGrid.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите заявку для удаления");
                return;
            }

            ServiceRequestGridItem selectedItem = RequestsGrid.SelectedItem as ServiceRequestGridItem;
            Service_Requests request = App.context.Service_Requests.Find(selectedItem.ID_Request);

            if (request == null)
            {
                NotificationService.ShowError("Заявка не найдена");
                return;
            }

            MessageBoxResult result = NotificationService.ShowConfirmation(
                "Вы уверены, что хотите удалить заявку #" + request.ID_Request + "?",
                "Подтверждение удаления"
            );

            if (result == MessageBoxResult.Yes)
            {
                App.context.Service_Requests.Remove(request);
                App.context.SaveChanges();
                NotificationService.ShowSuccess("Заявка успешно удалена");
                LoadData();
            }
        }

        #endregion

        #region Экспорт

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            List<ServiceRequestGridItem> dataToExport = RequestsGrid.ItemsSource as List<ServiceRequestGridItem>;
            if (dataToExport != null)
            {
                ExportService.ExportToCSV(dataToExport, "Заявки.csv");
            }
        }

        #endregion

        #region Двойной клик

        private void RequestsGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditButton_Click(sender, e);
        }

        #endregion
    }
}
