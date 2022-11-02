using System;
using System.Linq;
using TimetableApp.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit.Extensions;
using System.Net.Http;

namespace TimetableApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            foreach (var item in Configs.Directions.Values)
            {
                DirectionPicker.Items.Add(item);
            }

            foreach (var item in Configs.Courses.Values)
            {
                CoursePicker.Items.Add(item);
            }

            string directionKey = AppSettings.GetProperty(Configs._keyDirection) as string;
            DirectionPicker.SelectedIndex = -1;
            if (!string.IsNullOrWhiteSpace(directionKey))
            {
                DirectionPicker.SelectedIndex = Configs.Directions.IndexOf(x => x.Key.Equals(directionKey));
            }

            string courseKey = AppSettings.GetProperty(Configs._keyCourse) as string;
            CoursePicker.SelectedIndex = -1;
            if (!string.IsNullOrWhiteSpace(courseKey))
            {
                CoursePicker.SelectedIndex = Configs.Courses.IndexOf(x => x.Key.Equals(courseKey));
            }

            bool.TryParse(AppSettings.GetProperty(Configs._keyChangeNotificationStatus) as string,
                out bool changeNotifStatus);
            ChangeNotifStatus.On = changeNotifStatus;
        }


        private async void SaveButton_OnClicked(object sender, EventArgs e)
        {
            
            if (DirectionPicker.SelectedIndex == -1)
            {
                HandleEmptyPicker(DirectionPicker, "Не сохранено");
                return;
            }

            var directionStr = DirectionPicker.Items[DirectionPicker.SelectedIndex];
            if (directionStr == null)
            {
                HandleEmptyPicker(DirectionPicker, "Не сохранено");
                return;
            }

            var directionItem = Configs.Directions.Single(x => x.Value == directionStr);
            AppSettings.SetProperty(Configs._keyDirection, directionItem.Key);


            if (CoursePicker.SelectedIndex == -1)
            {
                HandleEmptyPicker(CoursePicker, "Не сохранено");
                return;
            }

            var courseStr = CoursePicker.Items[CoursePicker.SelectedIndex];
            if (courseStr == null)
            {
                HandleEmptyPicker(CoursePicker, "Не сохранено");
                return;
            }

            var courseItem = Configs.Courses.Single(x => x.Value == courseStr);
            AppSettings.SetProperty(Configs._keyCourse, courseItem.Key);

            bool changeNotifStatus = ChangeNotifStatus.On;
            AppSettings.SetProperty(Configs._keyChangeNotificationStatus, changeNotifStatus.ToString());

            await this.DisplayToastAsync("Сохранено", TimeSpan.FromSeconds(1).Milliseconds);
        }

        private async void HandleEmptyPicker(Picker picker, string message)
        {
            picker.Focus();
            await this.DisplayToastAsync(message, TimeSpan.FromSeconds(1).Milliseconds);
        }
    }
}