using PolyPrint2.Model;
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

            Users user = App.context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            return user;
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
