using PolyPrint2.AppData;
using System.Windows;
using System.Windows.Controls;

namespace PolyPrint2.View.Pages
{
    public partial class ProfilePage : Page
    {
        #region Инициализация

        public ProfilePage()
        {
            InitializeComponent();
            InitializeEvents();
            LoadData();
        }

        private void InitializeEvents()
        {
            ChangePasswordButton.Click += ChangePasswordButton_Click;
        }

        #endregion

        #region Загрузка данных

        private void LoadData()
        {
            if (App.CurrentUser != null)
            {
                LoginText.Text = App.CurrentUser.Login;
                RoleText.Text = App.CurrentUser.Role;
            }
        }

        #endregion

        #region Смена пароля

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string currentPassword = CurrentPasswordBox.Password.Trim();
            string newPassword = NewPasswordBox.Password.Trim();
            string confirmPassword = ConfirmPasswordBox.Password.Trim();

            if (!ValidationService.IsNotEmpty(currentPassword))
            {
                NotificationService.ShowWarning("Введите текущий пароль");
                return;
            }

            if (!ValidationService.IsNotEmpty(newPassword))
            {
                NotificationService.ShowWarning("Введите новый пароль");
                return;
            }

            if (newPassword.Length < 4)
            {
                NotificationService.ShowWarning("Пароль должен быть не менее 4 символов");
                return;
            }

            if (newPassword != confirmPassword)
            {
                NotificationService.ShowWarning("Пароли не совпадают");
                return;
            }

            if (App.CurrentUser.Password != currentPassword)
            {
                NotificationService.ShowError("Неверный текущий пароль");
                return;
            }

            App.CurrentUser.Password = newPassword;
            App.context.SaveChanges();

            CurrentPasswordBox.Password = "";
            NewPasswordBox.Password = "";
            ConfirmPasswordBox.Password = "";

            NotificationService.ShowSuccess("Пароль успешно изменён");
        }

        #endregion
    }
}
