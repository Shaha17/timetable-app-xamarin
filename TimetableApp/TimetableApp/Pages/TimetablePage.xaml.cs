using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TimetableApp.Models;
using TimetableApp.Services;
using TimetableApp.Utils;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;


namespace TimetableApp.Pages
{
    public partial class TimetablePage : ContentPage
    {
        private readonly TimetableService _timetableService;

        public TimetablePage()
        {
            InitializeComponent();
            _timetableService = new TimetableService();
        }

        protected override async void OnAppearing()
        {
            var directionKey = (AppSettings.GetProperty(Configs._keyDirection)) as string;
            var courseKey = (AppSettings.GetProperty(Configs._keyCourse)) as string;

            if (string.IsNullOrWhiteSpace(courseKey) || string.IsNullOrWhiteSpace(directionKey) ||
                !Configs.Directions.ContainsKey(directionKey) || !Configs.Courses.ContainsKey(courseKey))
            {
                await DisplayAlert("Не установлена основная группа", "", "Установить");

                GoToSettings();
                return;
            }

            string direction = Configs.Directions[directionKey];
            string course = Configs.Courses[courseKey];

            await UpdateTimetable();
        }

        private void LessonList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
        }

        private void GoToSettings()
        {
            MessagingCenter.Send<Page>(this, Configs._messageGoToSettings);
        }

        private async void LessonList_OnRefreshing(object sender, EventArgs e)
        {
            await UpdateTimetable();
        }

        private async Task UpdateTimetable()
        {
            RefreshView.IsVisible = false;
            try
            {
                throw new Exception();
                var directionKey = (AppSettings.GetProperty(Configs._keyDirection)) as string;
                var courseKey = (AppSettings.GetProperty(Configs._keyCourse)) as string;

                if (string.IsNullOrWhiteSpace(courseKey) || string.IsNullOrWhiteSpace(directionKey) ||
                    !Configs.Directions.ContainsKey(directionKey) || !Configs.Courses.ContainsKey(courseKey))
                {
                    await DisplayAlert("Не установлена основная группа", "", "Установить");

                    GoToSettings();
                    return;
                }

                LessonList.IsVisible = true;
                DirectionNameLabel.IsVisible = true;
                LessonList.IsRefreshing = true;

                string direction = Configs.Directions[directionKey];
                string course = Configs.Courses[courseKey];

                var lessonGroups = (await _timetableService.Get(directionKey, int.Parse(courseKey))).ToList();
                DirectionNameLabel.Text = $"{direction} {course}";
                LessonList.ItemsSource = lessonGroups;
            }
            catch (Exception e)
            {
                LessonList.IsRefreshing = false;
                await this.DisplayToastAsync("Ошибка " + e.Message, TimeSpan.FromSeconds(2).Milliseconds);

                var directionKey = (AppSettings.GetProperty(Configs._keyDirection)) as string;
                var courseKey = (AppSettings.GetProperty(Configs._keyCourse)) as string;
                var shortDirName = Configs.DirectionsShort[directionKey];

                DirectionNameLabel.IsVisible = false;
                LessonList.IsVisible = false;
                RefreshView.IsVisible = true;
                UpdateTimetableFromHeroku(shortDirName, int.Parse(courseKey));
            }
            finally
            {
                LessonList.IsRefreshing = false;
            }
        }

        private void UpdateTimetableFromHeroku(string dir, int course)
        {
            var url = _timetableService.GetUrlForHurokuBack(dir, course);
            WebViewTable.Source = url;
        }

        private async void RefreshView_OnRefreshing(object sender, EventArgs e)
        {
            await UpdateTimetable();
        }

        private void WebViewTable_OnNavigated(object sender, WebNavigatedEventArgs e)
        {
            RefreshView.IsRefreshing = false;
            LessonList.ItemsSource = null;
        }
    }
}