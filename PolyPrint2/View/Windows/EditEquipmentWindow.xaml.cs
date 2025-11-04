using PolyPrint2.AppData;
using PolyPrint2.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PolyPrint2.View.Windows
{
    public partial class EditEquipmentWindow : Window
    {
        #region Поля

        private Equipment currentEquipment;
        private bool isEditMode;

        #endregion

        #region Инициализация

        public EditEquipmentWindow(Equipment equipment)
        {
            InitializeComponent();
            currentEquipment = equipment;
            isEditMode = equipment != null;

            InitializeEvents();
            InitializeComboBoxes();
            LoadData();
        }

        private void InitializeEvents()
        {
            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        private void InitializeComboBoxes()
        {
            List<Clients> clients = App.context.Clients.ToList();
            ClientBox.ItemsSource = clients;

            List<string> conditions = new List<string>
            {
                "Исправно",
                "Требует обслуживания",
                "Неисправно",
                "На ремонте"
            };
            ConditionBox.ItemsSource = conditions;
            ConditionBox.SelectedIndex = 0;
        }

        #endregion

        #region Загрузка данных

        private void LoadData()
        {
            if (isEditMode)
            {
                TitleText.Text = "Редактирование оборудования";
                NameBox.Text = currentEquipment.Name;
                ModelBox.Text = currentEquipment.Model;
                SerialNumberBox.Text = currentEquipment.Serial_Number;
                ClientBox.SelectedValue = currentEquipment.ID_Client;
                ConditionBox.SelectedItem = currentEquipment.Condition;
            }
            else
            {
                TitleText.Text = "Добавление оборудования";
            }
        }

        #endregion

        #region Сохранение

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text.Trim();
            string model = ModelBox.Text.Trim();
            string serialNumber = SerialNumberBox.Text.Trim();
            int? clientId = ClientBox.SelectedValue as int?;
            string condition = ConditionBox.SelectedItem as string;

            if (!ValidationService.IsNotEmpty(name))
            {
                NotificationService.ShowWarning("Введите название оборудования");
                return;
            }

            if (!ValidationService.IsNotEmpty(model))
            {
                NotificationService.ShowWarning("Введите модель");
                return;
            }

            if (!ValidationService.IsNotEmpty(serialNumber))
            {
                NotificationService.ShowWarning("Введите серийный номер");
                return;
            }

            if (condition == null)
            {
                NotificationService.ShowWarning("Выберите состояние");
                return;
            }

            if (isEditMode)
            {
                currentEquipment.Name = name;
                currentEquipment.Model = model;
                currentEquipment.Serial_Number = serialNumber;
                currentEquipment.ID_Client = clientId;
                currentEquipment.Condition = condition;
                NotificationService.ShowSuccess("Оборудование успешно обновлено");
            }
            else
            {
                Equipment newEquipment = new Equipment
                {
                    Name = name,
                    Model = model,
                    Serial_Number = serialNumber,
                    ID_Client = clientId,
                    Condition = condition
                };
                App.context.Equipment.Add(newEquipment);
                NotificationService.ShowSuccess("Оборудование успешно добавлено");
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
