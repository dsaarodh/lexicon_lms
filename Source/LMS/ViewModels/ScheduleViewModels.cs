using System;
using System.Globalization;
using LMS.Models.AppData;
using LMS.ViewModels.Widgets;

namespace LMS.ViewModels
{
    public class ScheduleViewModels
    {
        public Calendar Calendar { get; set; }
        public Course Course { get; set; }
        public DateTime FirstDayOfWeek { get => DateTime.Today.AddDays(-(DateTime.Today.DayOfWeek - new CultureInfo("sv-SE").DateTimeFormat.FirstDayOfWeek)); }
        public TreeViewModel TreeView { get; set; }
    }
}