using LMS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace LMS.ViewModels
{
    public class ScheduleViewModels
    {
        public Calendar Calendar { get; set; }
        public CourseModel Course { get; set; }
        public DateTime FirstDayOfWeek { get => DateTime.Today.AddDays(-(DateTime.Today.DayOfWeek - new CultureInfo("sv-SE").DateTimeFormat.FirstDayOfWeek)); }
    }
}