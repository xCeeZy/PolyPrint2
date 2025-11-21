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
    public partial class EquipmentPage : Page
    {
        #region Поля

        private List<EquipmentGridItem> allEquipment;

        #endregion

        #region Инициализация

        public EquipmentPage()
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
            EquipmentGrid.MouseDoubleClick += EquipmentGrid_MouseDoubleClick;
        }

        #endregion

        #region Загрузка данных

        private void LoadData()
        {
            try
            {
                List<Equipment> equipment = App.context.Equipment.ToList();
                allEquipment = equipment.Select(e => new EquipmentGridItem
                {
                    ID_Equipment = e.ID_Equipment,
                    Name = e.Name,
                    Model = e.Model,
                    Serial_Number = e.Serial_Number,
                    ClientName = e.Clients != null ? e.Clients.Organization_Name : "",
                    Condition = e.Condition,
                    ID_Client = e.ID_Client
                }).ToList();

                EquipmentGrid.ItemsSource = allEquipment;
            }
            catch (Exception ex)
            {
                NotificationService.ShowError("Ошибка загрузки данных: " + ex.Message);
                allEquipment = new List<EquipmentGridItem>();
            }
        }

        #endregion

        #region Поиск

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                EquipmentGrid.ItemsSource = allEquipment;
                return;
            }

            List<EquipmentGridItem> filtered = allEquipment.Where(eq =>
                eq.Name.ToLower().Contains(searchText) ||
                (eq.Model != null && eq.Model.ToLower().Contains(searchText)) ||
                (eq.Serial_Number != null && eq.Serial_Number.ToLower().Contains(searchText)) ||
                (eq.ClientName != null && eq.ClientName.ToLower().Contains(searchText))
            ).ToList();

            EquipmentGrid.ItemsSource = filtered;
        }

        #endregion

        #region Добавление

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            EditEquipmentWindow window = new EditEquipmentWindow(null);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        #endregion

        #region Редактирование

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentGrid.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите оборудование для редактирования");
                return;
            }

            EquipmentGridItem selectedItem = EquipmentGrid.SelectedItem as EquipmentGridItem;
            Equipment equipment = App.context.Equipment.Find(selectedItem.ID_Equipment);

            if (equipment == null)
            {
                NotificationService.ShowError("Оборудование не найдено");
                return;
            }

            EditEquipmentWindow window = new EditEquipmentWindow(equipment);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        #endregion

        #region Удаление

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentGrid.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите оборудование для удаления");
                return;
            }

            EquipmentGridItem selectedItem = EquipmentGrid.SelectedItem as EquipmentGridItem;
            Equipment equipment = App.context.Equipment.Find(selectedItem.ID_Equipment);

            if (equipment == null)
            {
                NotificationService.ShowError("Оборудование не найдено");
                return;
            }

            MessageBoxResult result = NotificationService.ShowConfirmation(
                "Вы уверены, что хотите удалить оборудование " + equipment.Name + "?",
                "Подтверждение удаления"
            );

            if (result == MessageBoxResult.Yes)
            {
                App.context.Equipment.Remove(equipment);
                App.context.SaveChanges();
                NotificationService.ShowSuccess("Оборудование успешно удалено");
                LoadData();
            }
        }

        #endregion

        #region Экспорт

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            List<EquipmentGridItem> dataToExport = EquipmentGrid.ItemsSource as List<EquipmentGridItem>;
            if (dataToExport != null)
            {
                ExportService.ExportToCSV(dataToExport, "Оборудование.csv");
            }
        }

        #endregion

        #region Двойной клик

        private void EquipmentGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditButton_Click(sender, e);
        }

        #endregion
    }
}
