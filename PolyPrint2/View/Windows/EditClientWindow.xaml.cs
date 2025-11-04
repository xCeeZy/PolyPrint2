using PolyPrint2.AppData;
using PolyPrint2.Model;
using System.Windows;

namespace PolyPrint2.View.Windows
{
    public partial class EditClientWindow : Window
    {
        #region Поля

        private Clients currentClient;
        private bool isEditMode;

        #endregion

        #region Инициализация

        public EditClientWindow(Clients client)
        {
            InitializeComponent();
            currentClient = client;
            isEditMode = client != null;

            InitializeEvents();
            LoadData();
        }

        private void InitializeEvents()
        {
            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        #endregion

        #region Загрузка данных

        private void LoadData()
        {
            if (isEditMode)
            {
                TitleText.Text = "Редактирование клиента";
                OrganizationBox.Text = currentClient.Organization_Name;
                ContactNameBox.Text = currentClient.Contact_Name;
                PhoneBox.Text = currentClient.Phone;
                EmailBox.Text = currentClient.Email;
            }
            else
            {
                TitleText.Text = "Добавление клиента";
            }
        }

        #endregion

        #region Сохранение

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string organization = OrganizationBox.Text.Trim();
            string contactName = ContactNameBox.Text.Trim();
            string phone = PhoneBox.Text.Trim();
            string email = EmailBox.Text.Trim();

            if (!ValidationService.IsNotEmpty(organization))
            {
                NotificationService.ShowWarning("Введите название организации");
                return;
            }

            if (!ValidationService.IsNotEmpty(phone))
            {
                NotificationService.ShowWarning("Введите телефон");
                return;
            }

            if (!ValidationService.IsPhone(phone))
            {
                NotificationService.ShowWarning("Введите корректный номер телефона");
                return;
            }

            if (ValidationService.IsNotEmpty(email) && !ValidationService.IsEmail(email))
            {
                NotificationService.ShowWarning("Введите корректный email");
                return;
            }

            if (isEditMode)
            {
                currentClient.Organization_Name = organization;
                currentClient.Contact_Name = contactName;
                currentClient.Phone = phone;
                currentClient.Email = email;
                NotificationService.ShowSuccess("Клиент успешно обновлён");
            }
            else
            {
                Clients newClient = new Clients
                {
                    Organization_Name = organization,
                    Contact_Name = contactName,
                    Phone = phone,
                    Email = email
                };
                App.context.Clients.Add(newClient);
                NotificationService.ShowSuccess("Клиент успешно добавлен");
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
