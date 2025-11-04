using PolyPrint2.AppData;
using PolyPrint2.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PolyPrint2.View.Windows
{
    public partial class WorksWindow : Window
    {
        #region Поля

        private Service_Requests currentRequest;

        #endregion

        #region Инициализация

        public WorksWindow(Service_Requests request)
        {
            InitializeComponent();
            currentRequest = request;

            InitializeEvents();
            LoadData();
        }

        private void InitializeEvents()
        {
            AddWorkButton.Click += AddWorkButton_Click;
            CloseButton.Click += CloseButton_Click;
        }

        #endregion

        #region Загрузка данных

        private void LoadData()
        {
            string clientName = currentRequest.Clients != null ? currentRequest.Clients.Organization_Name : "";
            string equipmentName = currentRequest.Equipment != null ? currentRequest.Equipment.Name : "";

            RequestInfoText.Text = string.Format("Заявка #{0} от {1}",
                currentRequest.ID_Request,
                currentRequest.Created_Date.ToString("dd.MM.yyyy"));

            ProblemText.Text = string.Format("Клиент: {0} | Оборудование: {1} | Проблема: {2}",
                clientName,
                equipmentName,
                currentRequest.Problem_Description);

            List<Works> works = App.context.Works.Where(w => w.ID_Request == currentRequest.ID_Request).ToList();
            WorksGrid.ItemsSource = works;

            decimal total = works.Sum(w => w.Cost);
            TotalText.Text = string.Format("Общая стоимость: {0:N2} руб.", total);
        }

        #endregion

        #region Добавление работы

        private void AddWorkButton_Click(object sender, RoutedEventArgs e)
        {
            AddWorkWindow window = new AddWorkWindow(currentRequest.ID_Request);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        #endregion

        #region Закрытие

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
