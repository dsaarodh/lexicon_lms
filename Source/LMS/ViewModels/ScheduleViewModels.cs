using System;
using System.Globalization;
using LMS.Models.AppData;
using LMS.ViewModels.Widgets;
using System.Collections.Generic;

namespace LMS.ViewModels
{
    public class ScheduleViewModels
    {
        public Calendar Calendar { get; set; }
        public DateTime WeekStartDate { get; set; } = new DateTime(2017, 08, 30);
        public DateTime WeekEndDate => WeekStartDate.AddDays(7);
        public Course Course { get; set; }
        public DateTime FirstDayOfWeek { get => WeekStartDate.AddDays(-(WeekStartDate.DayOfWeek - new CultureInfo("sv-SE").DateTimeFormat.FirstDayOfWeek)); }
        public List<Day> Days { get; set; }
        public int CurrentWeek { get; set; }

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