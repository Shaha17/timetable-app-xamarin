using System.Globalization;

namespace TimetableApp.Utils
{
    public static class AppSettings
    {
        public static object GetProperty(string key)
        {
            if (App.Current.Properties.ContainsKey(key))
            {
                return App.Current.Properties[key];
            }
            return null;
        }

        public static void SetProperty(string key, object value)
        {
            if (App.Current.Properties.ContainsKey(key)) App.Current.Properties[key] = value;
            else App.Current.Properties.Add(key, value);
        }
        
        
    }
}