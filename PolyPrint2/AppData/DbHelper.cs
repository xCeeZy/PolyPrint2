using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;

namespace PolyPrint2.AppData
{
    public static class DbHelper
    {
        #region Обработка ошибок валидации

        public static string GetValidationErrorMessage(DbEntityValidationException ex)
        {
            StringBuilder errorMessage = new StringBuilder();
            errorMessage.AppendLine("Ошибка валидации данных:");
            errorMessage.AppendLine();

            foreach (DbEntityValidationResult validationResult in ex.EntityValidationErrors)
            {
                string entityName = validationResult.Entry.Entity.GetType().Name;
                errorMessage.AppendLine(string.Format("Сущность: {0}", entityName));

                foreach (DbValidationError error in validationResult.ValidationErrors)
                {
                    errorMessage.AppendLine(string.Format("  • Поле: {0}", error.PropertyName));
                    errorMessage.AppendLine(string.Format("    Ошибка: {0}", error.ErrorMessage));
                }
                errorMessage.AppendLine();
            }

            return errorMessage.ToString();
        }

        #endregion
    }
}
