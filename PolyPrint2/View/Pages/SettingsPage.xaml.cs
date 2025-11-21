using PolyPrint2.AppData;
using System.Windows;
using System.Windows.Controls;

namespace PolyPrint2.View.Pages
{
    public partial class SettingsPage : Page
    {
        #region Инициализация

        public SettingsPage()
        {
            InitializeComponent();
            InitializeEvents();
            LoadCurrentSettings();
        }

        private void InitializeEvents()
        {
            SaveButton.Click += SaveButton_Click;
        }

        private void LoadCurrentSettings()
        {
            if (ThemeService.IsDarkTheme)
            {
                DarkThemeRadio.IsChecked = true;
            }
            else
            {
                LightThemeRadio.IsChecked = true;
            }

            if (ThemeService.CurrentLanguage == "en")
            {
                EnglishRadio.IsChecked = true;
            }
            else
            {
                RussianRadio.IsChecked = true;
            }
        }

        #endregion

        #region Сохранение настроек

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool isDark = DarkThemeRadio.IsChecked == true;
            string language = EnglishRadio.IsChecked == true ? "en" : "ru";

            ThemeService.SetTheme(isDark);
            ThemeService.SetLanguage(language);

            NotificationService.ShowSuccess("Настройки сохранены! Перезапустите приложение для полного применения.");
        }

        #endregion
    }
}
