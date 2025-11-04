using PolyPrint2.AppData;
using PolyPrint2.View.Pages;
using System.Windows;
using System.Windows.Controls;

namespace PolyPrint2.View.Windows
{
    public partial class MainWindow : Window
    {
        #region Инициализация

        public MainWindow()
        {
            InitializeComponent();
            InitializeUI();
            InitializeEvents();
        }

        #endregion

        #region Настройка интерфейса

        private void InitializeUI()
        {
            if (App.CurrentUser == null)
            {
                NotificationService.ShowError("Ошибка авторизации");
                Application.Current.Shutdown();
                return;
            }

            UserRoleText.Text = App.CurrentUser.Role + ": " + App.CurrentUser.Login;

            ConfigureAccessByRole();

            MainFrame.Navigate(new ClientsPage());
        }

        private void ConfigureAccessByRole()
        {
            bool isAdmin = AuthService.IsAdmin(App.CurrentUser);
            bool isManager = AuthService.IsManager(App.CurrentUser);
            bool isMaster = AuthService.IsMaster(App.CurrentUser);

            ClientsButton.Visibility = isAdmin || isManager ? Visibility.Visible : Visibility.Collapsed;
            EquipmentButton.Visibility = isAdmin || isManager ? Visibility.Visible : Visibility.Collapsed;
            OrdersButton.Visibility = isAdmin || isManager ? Visibility.Visible : Visibility.Collapsed;
            ServiceRequestsButton.Visibility = Visibility.Visible;
            StatisticsButton.Visibility = isAdmin || isManager ? Visibility.Visible : Visibility.Collapsed;
            UsersButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region События

        private void InitializeEvents()
        {
            ClientsButton.Click += ClientsButton_Click;
            EquipmentButton.Click += EquipmentButton_Click;
            OrdersButton.Click += OrdersButton_Click;
            ServiceRequestsButton.Click += ServiceRequestsButton_Click;
            StatisticsButton.Click += StatisticsButton_Click;
            UsersButton.Click += UsersButton_Click;
            LogoutButton.Click += LogoutButton_Click;
        }

        #endregion

        #region Навигация

        private void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientsPage());
        }

        private void EquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EquipmentPage());
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrdersPage());
        }

        private void ServiceRequestsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ServiceRequestsPage());
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StatisticsPage());
        }

        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UsersPage());
        }

        #endregion

        #region Выход

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = NotificationService.ShowConfirmation("Вы уверены, что хотите выйти?", "Выход");

            if (result == MessageBoxResult.Yes)
            {
                App.CurrentUser = null;
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        #endregion
    }
}
