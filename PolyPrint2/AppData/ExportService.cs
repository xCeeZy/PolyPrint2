using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PolyPrint2.AppData
{
    public static class ExportService
    {
        #region Экспорт в CSV

        public static void ExportToCSV<T>(List<T> data, string defaultFileName)
        {
            if (data == null || data.Count == 0)
            {
                NotificationService.ShowWarning("Нет данных для экспорта");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV файлы (*.csv)|*.csv",
                FileName = defaultFileName,
                DefaultExt = "csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    StringBuilder csvContent = new StringBuilder();
                    System.Reflection.PropertyInfo[] properties = typeof(T).GetProperties();

                    string header = string.Join(";", properties.Select(p => p.Name));
                    csvContent.AppendLine(header);

                    foreach (T item in data)
                    {
                        List<string> values = new List<string>();
                        foreach (System.Reflection.PropertyInfo prop in properties)
                        {
                            object value = prop.GetValue(item);
                            string valueString = value != null ? value.ToString().Replace(";", ",") : "";
                            values.Add(valueString);
                        }
                        csvContent.AppendLine(string.Join(";", values));
                    }

                    File.WriteAllText(saveFileDialog.FileName, csvContent.ToString(), Encoding.UTF8);
                    NotificationService.ShowSuccess("Данные успешно экспортированы");
                }
                catch (System.Exception ex)
                {
                    NotificationService.ShowError("Ошибка при экспорте: " + ex.Message);
                }
            }
        }

        #endregion
    }
}
