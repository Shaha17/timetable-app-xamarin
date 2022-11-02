using Xamarin.Forms;

namespace TimetableApp.Pages
{
    public partial class MainPage : Shell
    {
        public MainPage()
        {
            InitializeComponent();
            Subscribe();
            TimeTablePageContent.Content = new TimetablePage();
            SearchPageContent.Content = new SearchTimetablePage();
            TeachersPageContent.Content = new TeacherTimetablePage();
            SettingsPagesContent.Content = new SettingsPage();

        }

        private void Subscribe()
        {
            MessagingCenter.Subscribe<Page>(this, Configs._messageGoToSettings,
                (sender) => { CurrentItem = SettingsPagesContent; });
        }
    }
}