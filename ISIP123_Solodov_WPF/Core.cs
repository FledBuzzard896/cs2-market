using ISIP123_Solodov_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP123_Solodov_WPF
{
    internal class Core
    {
        // Подключение к локальной БД
        public static SteamMarketEntities Context => new SteamMarketEntities();

        // Подключение к БД КИПа
        public static SteamMarketEntitiesKip ContextKIP => new SteamMarketEntitiesKip();

        // Хранение текущего пользователя
        public static Users CurrentUser = null;
    }
}
