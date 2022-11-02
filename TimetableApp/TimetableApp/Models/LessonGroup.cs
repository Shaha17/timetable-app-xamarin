using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TimetableApp.Models
{
    public class LessonGroup :List<Lesson>
    {
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public LessonGroup(string title, string shortTitle=null)
        {
            this.Title = title;
            this.ShortTitle = shortTitle;
        }
    }
}