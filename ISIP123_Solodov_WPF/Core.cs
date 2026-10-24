using ISIP123_Solodov_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        // Флаг текущей темы (по умолчанию темная)
        public static bool IsDarkTheme = true;

        public static void ChangeTheme(bool isDark)
        {
            IsDarkTheme = isDark;
            var dictionaries = Application.Current.Resources.MergedDictionaries;

            // Очищаем текущие стили
            dictionaries.Clear();

            // Путь к словарю 
            string themePath = isDark ? "Styles/MainStemDictionary.xaml" : "Styles/LightStemDictionary.xaml";

            var newDict = new ResourceDictionary
            {
                Source = new Uri(themePath, UriKind.Relative)
            };

            dictionaries.Add(newDict);
        }
    }
}
