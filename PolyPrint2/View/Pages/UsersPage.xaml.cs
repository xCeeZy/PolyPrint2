using PolyPrint2.AppData;
using PolyPrint2.Model;
using PolyPrint2.View.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PolyPrint2.View.Pages
{
    public partial class UsersPage : Page
    {
        #region Поля

        private List<Users> allUsers;

        #endregion

        #region Инициализация

        public UsersPage()
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
            allUsers = App.context.Users.ToList();
            UsersGrid.ItemsSource = allUsers;
        }

        #endregion

        #region Поиск

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                UsersGrid.ItemsSource = allUsers;
                return;
            }

            List<Users> filtered = allUsers.Where(u =>
                u.Login.ToLower().Contains(searchText) ||
                (u.Role != null && u.Role.ToLower().Contains(searchText))
            ).ToList();

            UsersGrid.ItemsSource = filtered;
        }

        #endregion

        #region Добавление

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            EditUserWindow window = new EditUserWindow(null);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        #endregion

        #region Редактирование

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите пользователя для редактирования");
                return;
            }

            Users selectedUser = UsersGrid.SelectedItem as Users;
            EditUserWindow window = new EditUserWindow(selectedUser);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        #endregion

        #region Удаление

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите пользователя для удаления");
                return;
            }

            Users selectedUser = UsersGrid.SelectedItem as Users;

            if (selectedUser.ID_User == App.CurrentUser.ID_User)
            {
                NotificationService.ShowWarning("Невозможно удалить текущего пользователя");
                return;
            }

            MessageBoxResult result = NotificationService.ShowConfirmation(
                "Вы уверены, что хотите удалить пользователя " + selectedUser.Login + "?",
                "Подтверждение удаления"
            );

            if (result == MessageBoxResult.Yes)
            {
                App.context.Users.Remove(selectedUser);
                App.context.SaveChanges();
                NotificationService.ShowSuccess("Пользователь успешно удалён");
                LoadData();
            }
        }

        #endregion
    }
}
