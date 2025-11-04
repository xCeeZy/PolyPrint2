using PolyPrint2.AppData;
using PolyPrint2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PolyPrint2.View.Windows
{
    public partial class EditServiceRequestWindow : Window
    {
        #region Поля

        private Service_Requests currentRequest;
        private bool isEditMode;

        #endregion

        #region Инициализация

        public EditServiceRequestWindow(Service_Requests request)
        {
            InitializeComponent();
            currentRequest = request;
            isEditMode = request != null;

            InitializeEvents();
            InitializeComboBoxes();
            LoadData();
        }

        private void InitializeEvents()
        {
            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
            ClientBox.SelectionChanged += ClientBox_SelectionChanged;
        }

        private void InitializeComboBoxes()
        {
            List<Clients> clients = App.context.Clients.ToList();
            ClientBox.ItemsSource = clients;

            List<string> statuses = new List<string>
            {
                "Новая",
                "В работе",
                "Ожидание запчастей",
                "Выполнена",
                "Закрыта"
            };
            StatusBox.ItemsSource = statuses;
            StatusBox.SelectedIndex = 0;

            List<Users> masters = App.context.Users.Where(u => u.Role == "Мастер").ToList();
            MasterBox.ItemsSource = masters;
        }

        #endregion

        #region События

        private void ClientBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ClientBox.SelectedValue != null)
            {
                int clientId = (int)ClientBox.SelectedValue;
                List<Equipment> equipment = App.context.Equipment.Where(eq => eq.ID_Client == clientId).ToList();
                EquipmentBox.ItemsSource = equipment;
            }
        }

        #endregion

        #region Загрузка данных

        private void LoadData()
        {
            if (isEditMode)
            {
                TitleText.Text = "Редактирование заявки";
                ClientBox.SelectedValue = currentRequest.ID_Client;
                ClientBox_SelectionChanged(null, null);
                EquipmentBox.SelectedValue = currentRequest.ID_Equipment;
                ProblemBox.Text = currentRequest.Problem_Description;
                StatusBox.SelectedItem = currentRequest.Status;
                MasterBox.SelectedValue = currentRequest.ID_Master;
            }
            else
            {
                TitleText.Text = "Создание сервисной заявки";
            }
        }

        #endregion

        #region Сохранение

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientBox.SelectedValue == null)
            {
                NotificationService.ShowWarning("Выберите клиента");
                return;
            }

            string problem = ProblemBox.Text.Trim();
            if (!ValidationService.IsNotEmpty(problem))
            {
                NotificationService.ShowWarning("Опишите проблему");
                return;
            }

            int clientId = (int)ClientBox.SelectedValue;
            int? equipmentId = EquipmentBox.SelectedValue as int?;
            string status = StatusBox.SelectedItem as string;
            int? masterId = MasterBox.SelectedValue as int?;

            if (isEditMode)
            {
                currentRequest.ID_Client = clientId;
                currentRequest.ID_Equipment = equipmentId;
                currentRequest.Problem_Description = problem;
                currentRequest.Status = status;
                currentRequest.ID_Master = masterId;
                NotificationService.ShowSuccess("Заявка успешно обновлена");
            }
            else
            {
                Service_Requests newRequest = new Service_Requests
                {
                    ID_Client = clientId,
                    ID_Equipment = equipmentId,
                    Created_Date = DateTime.Now,
                    Problem_Description = problem,
                    Status = status,
                    ID_Master = masterId
                };
                App.context.Service_Requests.Add(newRequest);
                NotificationService.ShowSuccess("Заявка успешно создана");
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
