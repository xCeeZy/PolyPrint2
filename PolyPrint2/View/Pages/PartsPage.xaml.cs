using PolyPrint2.AppData;
using PolyPrint2.Model;
using PolyPrint2.View.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PolyPrint2.View.Pages
{
    public partial class PartsPage : Page
    {
        #region Поля

        private List<Parts> allParts;

        #endregion

        #region Инициализация

        public PartsPage()
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
            PartsGrid.MouseDoubleClick += PartsGrid_MouseDoubleClick;
        }

        #endregion

        #region Загрузка данных

        private void LoadData()
        {
            try
            {
                allParts = App.context.Parts.ToList();
                PartsGrid.ItemsSource = allParts;
            }
            catch (Exception ex)
            {
                NotificationService.ShowError("Ошибка загрузки данных: " + ex.Message);
                allParts = new List<Parts>();
            }
        }

        #endregion

        #region Поиск

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                PartsGrid.ItemsSource = allParts;
                return;
            }

            List<Parts> filtered = allParts.Where(p =>
                (p.Name != null && p.Name.ToLower().Contains(searchText))
            ).ToList();

            PartsGrid.ItemsSource = filtered;
        }

        #endregion

        #region Добавление

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            EditPartWindow window = new EditPartWindow(null);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        #endregion

        #region Редактирование

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (PartsGrid.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите запчасть для редактирования");
                return;
            }

            Parts selectedPart = PartsGrid.SelectedItem as Parts;
            EditPartWindow window = new EditPartWindow(selectedPart);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        #endregion

        #region Удаление

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (PartsGrid.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите запчасть для удаления");
                return;
            }

            Parts selectedPart = PartsGrid.SelectedItem as Parts;

            MessageBoxResult result = NotificationService.ShowConfirmation(
                "Вы уверены, что хотите удалить запчасть " + selectedPart.Name + "?",
                "Подтверждение удаления"
            );

            if (result == MessageBoxResult.Yes)
            {
                App.context.Parts.Remove(selectedPart);
                App.context.SaveChanges();
                NotificationService.ShowSuccess("Запчасть успешно удалена");
                LoadData();
            }
        }

        #endregion

        #region Двойной клик

        private void PartsGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditButton_Click(sender, e);
        }

        #endregion
    }
}
