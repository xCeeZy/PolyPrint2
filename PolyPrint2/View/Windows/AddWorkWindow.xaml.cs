using PolyPrint2.AppData;
using PolyPrint2.Model;
using System;
using System.Windows;

namespace PolyPrint2.View.Windows
{
    public partial class AddWorkWindow : Window
    {
        #region Поля

        private int requestId;

        #endregion

        #region Инициализация

        public AddWorkWindow(int requestId)
        {
            InitializeComponent();
            this.requestId = requestId;

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        #endregion

        #region Сохранение

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string description = DescriptionBox.Text.Trim();
            string costText = CostBox.Text.Trim();

            if (!ValidationService.IsNotEmpty(description))
            {
                NotificationService.ShowWarning("Опишите выполненную работу");
                return;
            }

            decimal cost = 0;
            if (!decimal.TryParse(costText, out cost))
            {
                NotificationService.ShowWarning("Введите корректную стоимость");
                return;
            }

            if (!ValidationService.IsPositiveOrZero(cost))
            {
                NotificationService.ShowWarning("Стоимость не может быть отрицательной");
                return;
            }

            Works newWork = new Works
            {
                ID_Request = requestId,
                Description = description,
                Cost = cost,
                Work_Date = DateTime.Now
            };

            App.context.Works.Add(newWork);
            App.context.SaveChanges();

            NotificationService.ShowSuccess("Работа успешно добавлена");
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
