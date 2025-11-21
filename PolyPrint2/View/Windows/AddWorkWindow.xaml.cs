using PolyPrint2.AppData;
using PolyPrint2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PolyPrint2.View.Windows
{
    public partial class AddWorkWindow : Window
    {
        #region Поля

        private int requestId;
        private List<Parts> availableParts;
        private List<UsedPartItem> usedPartItems;

        #endregion

        #region Инициализация

        public AddWorkWindow(int requestId)
        {
            InitializeComponent();
            this.requestId = requestId;
            usedPartItems = new List<UsedPartItem>();

            InitializeEvents();
            LoadParts();
        }

        private void InitializeEvents()
        {
            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
            AddPartButton.Click += AddPartButton_Click;
            RemovePartButton.Click += RemovePartButton_Click;
        }

        private void LoadParts()
        {
            availableParts = App.context.Parts.Where(p => p.Quantity > 0).ToList();
            PartsComboBox.ItemsSource = availableParts;
            if (availableParts.Count > 0)
            {
                PartsComboBox.SelectedIndex = 0;
            }
        }

        #endregion

        #region Управление запчастями

        private void AddPartButton_Click(object sender, RoutedEventArgs e)
        {
            if (PartsComboBox.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите запчасть");
                return;
            }

            int quantity = 0;
            if (!int.TryParse(PartQuantityBox.Text.Trim(), out quantity) || quantity <= 0)
            {
                NotificationService.ShowWarning("Введите корректное количество");
                return;
            }

            Parts selectedPart = PartsComboBox.SelectedItem as Parts;

            UsedPartItem existingItem = usedPartItems.FirstOrDefault(x => x.PartId == selectedPart.ID_Part);
            int totalRequired = quantity;
            if (existingItem != null)
            {
                totalRequired += existingItem.Quantity;
            }

            if (totalRequired > selectedPart.Quantity)
            {
                NotificationService.ShowWarning(string.Format("Недостаточно запчастей на складе. Доступно: {0}", selectedPart.Quantity));
                return;
            }

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.Total = existingItem.Quantity * existingItem.Price;
            }
            else
            {
                UsedPartItem newItem = new UsedPartItem
                {
                    PartId = selectedPart.ID_Part,
                    PartName = selectedPart.Name,
                    Quantity = quantity,
                    Price = selectedPart.Price,
                    Total = quantity * selectedPart.Price
                };
                usedPartItems.Add(newItem);
            }

            RefreshPartsGrid();
            PartQuantityBox.Text = "1";
        }

        private void RemovePartButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsedPartsGrid.SelectedItem == null)
            {
                NotificationService.ShowWarning("Выберите запчасть для удаления");
                return;
            }

            UsedPartItem selectedItem = UsedPartsGrid.SelectedItem as UsedPartItem;
            usedPartItems.Remove(selectedItem);
            RefreshPartsGrid();
        }

        private void RefreshPartsGrid()
        {
            UsedPartsGrid.ItemsSource = null;
            UsedPartsGrid.ItemsSource = usedPartItems;

            decimal partsTotal = usedPartItems.Sum(x => x.Total);
            PartsTotalText.Text = string.Format("Стоимость запчастей: {0:N2} руб.", partsTotal);
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

            foreach (UsedPartItem item in usedPartItems)
            {
                Used_Parts usedPart = new Used_Parts
                {
                    ID_Work = newWork.ID_Work,
                    ID_Part = item.PartId,
                    Quantity = item.Quantity
                };
                App.context.Used_Parts.Add(usedPart);

                Parts part = App.context.Parts.FirstOrDefault(p => p.ID_Part == item.PartId);
                if (part != null)
                {
                    part.Quantity -= item.Quantity;

                    if (part.Quantity <= 5)
                    {
                        NotificationService.ShowWarning(string.Format("Внимание! Низкий остаток запчасти \"{0}\": {1} шт.", part.Name, part.Quantity));
                    }
                }
            }

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

    #region Вспомогательный класс

    public class UsedPartItem
    {
        public int PartId { get; set; }
        public string PartName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }

    #endregion
}
