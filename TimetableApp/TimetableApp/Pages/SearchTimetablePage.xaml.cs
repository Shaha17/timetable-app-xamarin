using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableApp.Services;
using TimetableApp.Utils;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimetableApp.Pages
{
    public partial class SearchTimetablePage : ContentPage
    {
        private readonly TimetableService _timetableService;
        public string dirName { get; set; }
        public string course { get; set; }
        private readonly string _cancelString = "Отмена";

        public SearchTimetablePage()
        {
            InitializeComponent();
            _timetableService = new TimetableService();
        }

        private async Task GetDirAndCourse()
        {
            var listOfDirs = Configs.DirectionsShort.Values.ToArray();
            var listOfCourse = Configs.Courses.Values.ToArray();
            var dir = await DisplayActionSheet("Выберите направление", this._cancelString, null, listOfDirs);
            var course = await DisplayActionSheet("Выберите курс", this._cancelString, null, listOfCourse);

            this.dirName = dir;
            this.course = course;
        }

        private bool IsDirAndCourseValid()
        {
            if (string.IsNullOrWhiteSpace(this.course) || string.IsNullOrWhiteSpace(this.dirName))
                return false;
            if (this.course == _cancelString && this.dirName == _cancelString)
                return false;
            if (!Configs.Courses.Values.Contains(this.course))
                return false;
            if (!Configs.Directions.Values.Concat(Configs.DirectionsShort.Values).Contains(this.dirName))
                return false;
            return true;
        }

        protected override async void OnAppearing()
        {
            if (!IsDirAndCourseValid())
            {
                await GetDirAndCourse();
                if (IsDirAndCourseValid())
                {
                    await UpdateTimetable();
                }
            }
        }

        private void LessonList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
        }

        private async void LessonList_OnRefreshing(object sender, EventArgs e)
        {
            await UpdateTimetable();
        }

        private async Task UpdateTimetable()
        {
            LessonList.IsRefreshing = true;
            LessonList.IsVisible = true;
            RefreshView.IsVisible = false;
            try
            {
                throw new Exception();
                var directionKey = Configs.DirectionsShort.Single(x => x.Value.Equals(this.dirName)).Key;
                var courseKey = this.course;

                if (!IsDirAndCourseValid())
                {
                    return;
                }

                LessonList.IsVisible = true;
                LessonList.IsRefreshing = true;

                string dir = this.dirName;
                string course = this.course;

                var lessonGroups = (await _timetableService.Get(directionKey, int.Parse(courseKey))).ToList();
                DirectionNameLabel.Text = $"{dir} {course}";
                LessonList.ItemsSource = lessonGroups;
            }
            catch (Exception e)
            {
                LessonList.IsRefreshing = false;

                await this.DisplayToastAsync("Ошибка " + e.Message, TimeSpan.FromSeconds(2).Milliseconds);
                var directionKey = Configs.DirectionsShort.Single(x => x.Value.Equals(this.dirName)).Key;
                var courseKey = this.course;
                var shortDirName = Configs.DirectionsShort[directionKey];
                LessonList.IsVisible = false;
                RefreshView.IsVisible = true;
                UpdateTimetableFromHeroku(shortDirName, int.Parse(courseKey));
                DirectionNameLabel.Text = $"{this.dirName} {this.course}";
            }
            finally
            {
                LessonList.IsRefreshing = false;
            }
        }

        private async void ChooseDirAndCourseButton_OnClicked(object sender, EventArgs e)
        {
            await GetDirAndCourse();
            if (IsDirAndCourseValid())
            {
                await UpdateTimetable();
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