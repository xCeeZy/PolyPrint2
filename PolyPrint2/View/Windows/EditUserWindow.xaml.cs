using PolyPrint2.AppData;
using PolyPrint2.Model;
using System.Collections.Generic;
using System.Windows;

namespace PolyPrint2.View.Windows
{
    public partial class EditUserWindow : Window
    {
        #region Поля

        private Users currentUser;
        private bool isEditMode;

        #endregion

        #region Инициализация

        public EditUserWindow(Users user)
        {
            InitializeComponent();
            currentUser = user;
            isEditMode = user != null;

            InitializeEvents();
            InitializeRoles();
            LoadData();
        }

        private void InitializeEvents()
        {
            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        private void InitializeRoles()
        {
            List<string> roles = new List<string>
            {
                "Администратор",
                "Менеджер",
                "Мастер"
            };
            RoleBox.ItemsSource = roles;
            RoleBox.SelectedIndex = 0;
        }

        #endregion

        #region Загрузка данных

        private void LoadData()
        {
            if (isEditMode)
            {
                TitleText.Text = "Редактирование пользователя";
                LoginBox.Text = currentUser.Login;
                RoleBox.SelectedItem = currentUser.Role;
            }
            else
            {
                TitleText.Text = "Добавление пользователя";
            }
        }

        #endregion

        #region Сохранение

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            string role = RoleBox.SelectedItem as string;

            if (!ValidationService.IsNotEmpty(login))
            {
                NotificationService.ShowWarning("Введите логин");
                return;
            }

            if (!isEditMode && !ValidationService.IsNotEmpty(password))
            {
                NotificationService.ShowWarning("Введите пароль");
                return;
            }

            if (role == null)
            {
                NotificationService.ShowWarning("Выберите роль");
                return;
            }

            if (isEditMode)
            {
                currentUser.Login = login;
                if (ValidationService.IsNotEmpty(password))
                {
                    currentUser.Password = password;
                }
                currentUser.Role = role;
                NotificationService.ShowSuccess("Пользователь успешно обновлён");
            }
            else
            {
                Users newUser = new Users
                {
                    Login = login,
                    Password = password,
                    Role = role
                };
                App.context.Users.Add(newUser);
                NotificationService.ShowSuccess("Пользователь успешно добавлен");
            }

            App.context.SaveChanges();
            DialogResult = true;
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
