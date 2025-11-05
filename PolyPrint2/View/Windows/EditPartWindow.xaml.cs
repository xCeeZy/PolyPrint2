using PolyPrint2.AppData;
using PolyPrint2.Model;
using System.Windows;

namespace PolyPrint2.View.Windows
{
    public partial class EditPartWindow : Window
    {
        #region Поля

        private Parts currentPart;
        private bool isEditMode;

        #endregion

        #region Инициализация

        public EditPartWindow(Parts part)
        {
            InitializeComponent();
            currentPart = part;
            isEditMode = part != null;

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
                TitleText.Text = "Редактирование запчасти";
                NameBox.Text = currentPart.Name;
                PriceBox.Text = currentPart.Price.ToString();
                QuantityBox.Text = currentPart.Quantity.ToString();
            }
            else
            {
                TitleText.Text = "Добавление запчасти";
            }
        }

        #endregion

        #region Сохранение

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text.Trim();
            string priceText = PriceBox.Text.Trim();
            string quantityText = QuantityBox.Text.Trim();

            if (!ValidationService.IsNotEmpty(name))
            {
                NotificationService.ShowWarning("Введите название запчасти");
                return;
            }

            decimal price;
            if (!decimal.TryParse(priceText, out price) || price < 0)
            {
                NotificationService.ShowWarning("Введите корректную цену");
                return;
            }

            int quantity;
            if (!int.TryParse(quantityText, out quantity) || quantity < 0)
            {
                NotificationService.ShowWarning("Введите корректное количество");
                return;
            }

            if (isEditMode)
            {
                currentPart.Name = name;
                currentPart.Price = price;
                currentPart.Quantity = quantity;
                NotificationService.ShowSuccess("Запчасть успешно обновлена");
            }
            else
            {
                Parts newPart = new Parts
                {
                    Name = name,
                    Price = price,
                    Quantity = quantity
                };
                App.context.Parts.Add(newPart);
                NotificationService.ShowSuccess("Запчасть успешно добавлена");
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
