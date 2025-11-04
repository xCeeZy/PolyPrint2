using PolyPrint2.AppData;
using PolyPrint2.Model;
using PolyPrint2.View.Windows;
using System.Collections.Generic;
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
        }

        #endregion

        #region Загрузка данных

        private void LoadData()
        {
            allClients = App.context.Clients.ToList();
            ClientsGrid.ItemsSource = allClients;
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

            MessageBoxResult result = NotificationService.ShowConfirmation(
                "Вы уверены, что хотите удалить клиента " + selectedClient.Organization_Name + "?",
                "Подтверждение удаления"
            );

            if (result == MessageBoxResult.Yes)
            {
                App.context.Clients.Remove(selectedClient);
                App.context.SaveChanges();
                NotificationService.ShowSuccess("Клиент успешно удалён");
                LoadData();
            }
        }

        #endregion
    }
}
