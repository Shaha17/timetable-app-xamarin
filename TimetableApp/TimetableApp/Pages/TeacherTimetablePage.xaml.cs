using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableApp.Services;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimetableApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TeacherTimetablePage : ContentPage
    {
        private readonly TimetableService _timetableService;
        public string teacherName { get; set; }
        private readonly string _cancelString = "Отмена";

        public TeacherTimetablePage()
        {
            InitializeComponent();
            _timetableService = new TimetableService();
        }

        private async Task GetTeacherFio()
        {
            var teacher = await DisplayPromptAsync("Введите преподавателя", "", "Готово", "Отмена",
                "Например: Коваленко, Давлятов",
                20, Keyboard.Text);
            this.teacherName = teacher;
            //await this.DisplayToastAsync(teacher);
        }

        private bool IsTeacherFioValid()
        {
            if (string.IsNullOrWhiteSpace(this.teacherName))
                return false;
            if (this.teacherName == _cancelString)
                return false;

            return true;
        }

        protected override async void OnAppearing()
        {
            if (!IsTeacherFioValid())
            {
                await GetTeacherFio();
                if (IsTeacherFioValid())
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

            try
            {
                if (!IsTeacherFioValid())
                {
                    return;
                }

                LessonList.IsRefreshing = true;

                var lessonGroups = (await _timetableService.GetTeacherTimetable(teacherName)).ToList();
                var teacherFullName = lessonGroups.FirstOrDefault().FirstOrDefault().Teacher.Split('|')
                    .Single(p => p.Contains('.')).Trim()
                    .Split(',')
                    .Select(t => t.Trim())
                    .Single(x => x.Contains(this.teacherName));
                TeacherLabel.Text = string.IsNullOrWhiteSpace(teacherFullName)
                    ? $"{this.teacherName}"
                    : $"{teacherFullName}";


                LessonList.ItemsSource = lessonGroups;
            }
            catch (Exception e)
            {
                LessonList.IsRefreshing = false;

                await this.DisplayToastAsync("Ошибка " + e.Message, TimeSpan.FromSeconds(2).Milliseconds);
            }
            finally
            {
                LessonList.IsRefreshing = false;
            }
        }

        private async void ChooseTeacherButton_OnClicked(object sender, EventArgs e)
        {
            await GetTeacherFio();
            if (IsTeacherFioValid())
            {
                await UpdateTimetable();
            }
        }
    }
}