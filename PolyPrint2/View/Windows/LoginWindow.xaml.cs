using PolyPrint2.AppData;
using PolyPrint2.Model;
using System.Windows;

namespace PolyPrint2.View.Windows
{
    public partial class LoginWindow : Window
    {
        #region Инициализация

        public LoginWindow()
        {
            InitializeComponent();
            LoginButton.Click += LoginButton_Click;
            PasswordBox.KeyDown += PasswordBox_KeyDown;
        }

        #endregion

        #region Обработчики событий

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            PerformLogin();
        }

        private void PasswordBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                PerformLogin();
            }
        }

        #endregion

        #region Авторизация

        private void PerformLogin()
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (!ValidationService.IsNotEmpty(login) || !ValidationService.IsNotEmpty(password))
            {
                NotificationService.ShowWarning("Введите логин и пароль");
                return;
            }

            Users user = AuthService.Authenticate(login, password);

            if (user == null)
            {
                NotificationService.ShowError("Неверный логин или пароль");
                return;
            }

            App.CurrentUser = user;

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        #endregion
    }
}
