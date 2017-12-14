using System;
using System.Globalization;
using LMS.Models.AppData;
using LMS.ViewModels.Widgets;
using System.Collections.Generic;

namespace LMS.ViewModels
{
    public class ScheduleViewModels
    {
		public ScheduleViewModels(DateTime dayInWeek)
		{
			WeekStartDate = dayInWeek.AddDays(-(dayInWeek.DayOfWeek - CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek));
			WeekEndDate = WeekStartDate.AddDays(6);

			CurrentWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dayInWeek, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
		}

		public DateTime WeekStartDate { get; private set; }
		public DateTime WeekEndDate { get; private set; }
		public List<Day> Days { get; set; }
		public int CurrentWeek { get; private set; }

		public Course Course { get; set; }
        public Activity[] Activities { get; set; }

        public TreeViewModel TreeView { get; set; }
    }

    public class Day
    {
        public DateTime Date { get; set; }
        public List<Module> Modules { get; set; }
//        public Module ModulePM { get; set; }
    }
}