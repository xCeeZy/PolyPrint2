using System;
using System.Windows;

namespace PolyPrint2.AppData
{
    public static class ThemeService
    {
        #region Поля

        private static bool isDarkTheme = false;
        private static string currentLanguage = "ru";

        #endregion

        #region Свойства

        public static bool IsDarkTheme
        {
            get { return isDarkTheme; }
        }

        public static string CurrentLanguage
        {
            get { return currentLanguage; }
        }

        #endregion

        #region Управление темой

        public static void SetTheme(bool dark)
        {
            isDarkTheme = dark;

            ResourceDictionary themeDictionary = new ResourceDictionary();

            if (dark)
            {
                themeDictionary.Source = new Uri("/Resources/DarkTheme.xaml", UriKind.Relative);
            }
            else
            {
                themeDictionary.Source = new Uri("/Resources/LightTheme.xaml", UriKind.Relative);
            }

            Application.Current.Resources.MergedDictionaries.RemoveAt(0);
            Application.Current.Resources.MergedDictionaries.Insert(0, themeDictionary);

            Properties.Settings.Default.IsDarkTheme = dark;
            Properties.Settings.Default.Save();
        }

        public static void LoadSavedTheme()
        {
            bool savedDarkTheme = Properties.Settings.Default.IsDarkTheme;
            SetTheme(savedDarkTheme);
        }

        #endregion

        #region Управление языком

        public static void SetLanguage(string language)
        {
            currentLanguage = language;

            ResourceDictionary stringsDictionary = new ResourceDictionary();

            if (language == "en")
            {
                stringsDictionary.Source = new Uri("/Resources/Strings.en.xaml", UriKind.Relative);
            }
            else
            {
                stringsDictionary.Source = new Uri("/Resources/Strings.ru.xaml", UriKind.Relative);
            }

            if (Application.Current.Resources.MergedDictionaries.Count > 2)
            {
                Application.Current.Resources.MergedDictionaries.RemoveAt(2);
            }
            Application.Current.Resources.MergedDictionaries.Add(stringsDictionary);

            Properties.Settings.Default.Language = language;
            Properties.Settings.Default.Save();
        }

        public static void LoadSavedLanguage()
        {
            string savedLanguage = Properties.Settings.Default.Language;
            if (string.IsNullOrEmpty(savedLanguage))
            {
                savedLanguage = "ru";
            }
            SetLanguage(savedLanguage);
        }

        #endregion

        #region Инициализация

        public static void Initialize()
        {
            LoadSavedTheme();
            LoadSavedLanguage();
        }

        #endregion
    }
}
