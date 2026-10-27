using ISIP123_Solodov_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ISIP123_Solodov_WPF
{
    internal class Core
    {
        // Подключение к локальной БД
        public static SteamMarketEntitiesHome Context = new SteamMarketEntitiesHome();

        // Подключение к БД КИПа
        public static SteamMarketEntitiesKip ContextKIP = new SteamMarketEntitiesKip();

        // Хранение текущего пользователя
        public static Users CurrentUser = null;

        public static bool IsFirstHelpDialogShow = false;

        public static string HashPassword(string password) 
        {
            // Создаем объект SHA256
            using (SHA256 sha256Hash = SHA256.Create()) 
            {
                // Преобразование строки в массив байтов и хэшируем
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Преобразование обратно в строку для сохранение в БД
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // "x2" делает hex-формат (например, 0F, 1A)
                }
                return builder.ToString();
            }
        }
    }
}


/*
 * ivan - pass123
 * petr - pass456
 * anna - pass789
 * sergey - pass000
 * asd - asdasd7
 * boss - megaboss111
*/