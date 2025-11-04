using System;
using System.Text.RegularExpressions;

namespace PolyPrint2.AppData
{
    public static class ValidationService
    {
        #region Проверка строк

        public static bool IsNotEmpty(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static bool IsEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        public static bool IsPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return false;
            }

            string pattern = @"^[\d\s\+\-\(\)]+$";
            return Regex.IsMatch(phone, pattern) && phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Length >= 10;
        }

        #endregion

        #region Проверка чисел

        public static bool IsPositiveNumber(decimal value)
        {
            return value > 0;
        }

        public static bool IsPositiveOrZero(decimal value)
        {
            return value >= 0;
        }

        #endregion

        #region Проверка дат

        public static bool IsValidDate(DateTime date)
        {
            return date > DateTime.MinValue && date <= DateTime.Now.AddYears(10);
        }

        public static bool IsNotFutureDate(DateTime date)
        {
            return date <= DateTime.Now;
        }

        #endregion
    }
}
