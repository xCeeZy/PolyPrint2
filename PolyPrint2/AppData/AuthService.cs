using PolyPrint2.Model;
using System;
using System.Linq;

namespace PolyPrint2.AppData
{
    public static class AuthService
    {
        #region Авторизация

        public static Users Authenticate(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            try
            {
                Users user = App.context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка подключения к базе данных. Проверьте:\n1. Запущен ли SQL Server\n2. Правильность строки подключения в App.config\n\nТехническая информация: " + ex.Message);
            }
        }

        #endregion

        #region Проверка прав доступа

        public static bool IsAdmin(Users user)
        {
            if (user == null)
            {
                return false;
            }
            return user.Role == "Администратор";
        }

        public static bool IsManager(Users user)
        {
            if (user == null)
            {
                return false;
            }
            return user.Role == "Менеджер";
        }

        public static bool IsMaster(Users user)
        {
            if (user == null)
            {
                return false;
            }
            return user.Role == "Мастер";
        }

        #endregion
    }
}
