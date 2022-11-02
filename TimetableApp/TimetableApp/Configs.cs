using System.Collections.Generic;

namespace TimetableApp
{
    public static class Configs
    {
        public static string _keyCourse = "course";
        public static string _keyDirection = "direction";
        public static string _keyChangeNotificationStatus = "notifications_status";
        public static string _messageGoToSettings = "GoToSettings";

        public static Dictionary<string, string> Directions = new Dictionary<string, string>()
        {
            {"pmi", "Прикладная математика и информатика"},
            {"geo", "Геология"},
            {"hfmm", "Химия, физика и механика материалов"},
            {"mo", "Международные отношения"},
            {"gmu", "Государственное и муниципальное управление"},
            {"lin", "Лингвистика"},
        };
        public static Dictionary<string, string> DirectionsShort = new Dictionary<string, string>()
        {
            {"pmi", "ПМиИ"},
            {"geo", "Геология"},
            {"hfmm", "ХФММ"},
            {"mo", "МО"},
            {"gmu", "ГМУ"},
            {"lin", "Лингвистика"},
        };

        public static Dictionary<string, string> Courses = new Dictionary<string, string>()
        {
            {"1", "1"},
            {"2", "2"},
            {"3", "3"},
            {"4", "4"},
        };
    }
}