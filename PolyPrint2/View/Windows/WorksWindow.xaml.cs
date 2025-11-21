using PolyPrint2.AppData;
using PolyPrint2.Model;
using System;
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

            List<WorkGridItem> workItems = new List<WorkGridItem>();
            decimal totalWorksCost = 0;
            decimal totalPartsCost = 0;

            foreach (Works work in works)
            {
                List<Used_Parts> usedParts = App.context.Used_Parts.Where(up => up.ID_Work == work.ID_Work).ToList();

                string partsInfo = "";
                decimal partsCost = 0;

                foreach (Used_Parts usedPart in usedParts)
                {
                    Parts part = App.context.Parts.FirstOrDefault(p => p.ID_Part == usedPart.ID_Part);
                    if (part != null)
                    {
                        if (!string.IsNullOrEmpty(partsInfo))
                        {
                            partsInfo += ", ";
                        }
                        partsInfo += string.Format("{0} x{1}", part.Name, usedPart.Quantity);
                        partsCost += part.Price * usedPart.Quantity;
                    }
                }

                WorkGridItem item = new WorkGridItem
                {
                    ID_Work = work.ID_Work,
                    Work_Date = work.Work_Date,
                    Description = work.Description,
                    Cost = work.Cost,
                    UsedPartsInfo = string.IsNullOrEmpty(partsInfo) ? "-" : partsInfo,
                    PartsCost = partsCost
                };

                workItems.Add(item);
                totalWorksCost += work.Cost;
                totalPartsCost += partsCost;
            }

            WorksGrid.ItemsSource = workItems;

            WorksTotalText.Text = string.Format("Стоимость работ: {0:N2} руб.", totalWorksCost);
            PartsTotalText.Text = string.Format("Стоимость запчастей: {0:N2} руб.", totalPartsCost);
            GrandTotalText.Text = string.Format("Итого: {0:N2} руб.", totalWorksCost + totalPartsCost);
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

    #region Вспомогательный класс

    public class WorkGridItem
    {
        public int ID_Work { get; set; }
        public DateTime Work_Date { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public string UsedPartsInfo { get; set; }
        public decimal PartsCost { get; set; }
    }

    #endregion
}
