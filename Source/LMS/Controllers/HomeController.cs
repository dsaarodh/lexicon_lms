using LMS.DataAccess;
using LMS.Models.AppData;
using LMS.ViewModels;
using LMS.ViewModels.Widgets;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using static LMS.ViewModels.Widgets.TreeViewModel;

namespace LMS.Controllers
{
    public class HomeController : Controller
	{
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Teacher, Student")]
        public ActionResult Index()
        {

            var scheduleVM = new ScheduleViewModels();
            CultureInfo cultureInfo = new CultureInfo("sv-SE");
            scheduleVM.Calendar = cultureInfo.Calendar;
            var userId = User.Identity.GetUserId();
            var courseId = db.Users.Find(userId).CourseId;

            scheduleVM.Course = new Course();
            if (!User.IsInRole(Role.Teacher))
            {
                scheduleVM.Course.Name = db.Courses.Where(co => co.Id == courseId).Select(n => n.Name).FirstOrDefault().ToString();
            }

            scheduleVM.Days = new List<Day>();
            scheduleVM.Activities = db.Activities
                    .Where(a => a.Module.Course.Users.Any(u => u.Id == userId) && a.StartDate <= scheduleVM.WeekEndDate && a.EndDate >= scheduleVM.WeekStartDate)
                    .ToArray();

            for (int i = 0; i < 7; i++)
            {
                Day day = new Day
                {
                    Date = scheduleVM.WeekStartDate.AddDays(i)
                };

                if (courseId != null)
                {
                    day.Modules = db.Courses.Find(courseId).Modules.Where(c => c.StartDate.Day == day.Date.Day).ToList();
                }
                if (day.Modules != null)
                {
                    for (int j = 0; j < day.Modules.Count; j++)
                    {
                        day.Modules[j].Activities = day.Modules[j].Activities.Where(d => d.StartDate.Day == day.Date.Day).ToList();
                    }
                }
                scheduleVM.Days.Add(day);
            }

            if (!User.Identity.IsAuthenticated)
            {
                scheduleVM.TreeView = new TreeViewModel();
                return View(scheduleVM);
            }

            scheduleVM.TreeView = TreeView();

            return View(scheduleVM);
        }

        private TreeViewModel TreeView()
        {
            var userId = User.Identity.GetUserId();
            var courseId = db.Users.Find(userId).CourseId;
            bool isTeacher = User.IsInRole(Role.Teacher);

            var courses = db.Courses
                .Where(co => isTeacher ? true : co.Id == courseId)
                .Include(c => c.Modules.Select(m => m.Activities)).ToList();

            var treeData = courses.Select(c =>
            {
                var cNode = new TreeViewNode { Text = c.Name, Href = "/Home/Course"  };
                cNode.Nodes = c.Modules.Select(m =>
                {
                    var node = new TreeViewNode { Text = m.Name };
                    node.Nodes = m.Activities.Select(a => new TreeViewNode { Text = a.Name }).ToArray();
                    return node;
                }).ToArray();
                return cNode;
            }).ToArray();

            var model = new TreeViewModel()
            {
                EnableLinks = true,
                Data = treeData
            };
            return model;
        }

        public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}

        public PartialViewResult Activity(int activityId)
        {            
            Activity activity = db.Activities.Where(a => a.Id == activityId).FirstOrDefault();

            return PartialView("_Activity", activity);
        }

        public PartialViewResult Course()
        {
            Course course = db.Courses.Where(c => c.Id == 1).FirstOrDefault();
            return PartialView("_Course", course);
        }

        public PartialViewResult Module(int moduleId)
        {
            Module module = db.Modules.Where(m => m.Id == moduleId).FirstOrDefault();
            module.ColorCode = "#070707";
            return PartialView("_Module", module);
        }

        public string ColorShade(string color)
        {
            var c = System.Drawing.ColorTranslator.FromHtml(color);
            System.Drawing.Color darkColor = System.Drawing.Color.FromArgb(1,  Math.Min(c.R - 50, 255), Math.Min(c.G - 50, 255), Math.Min(c.B - 50, 255));
            return String.Format("#{0:X2}{1:X2}{2:X2}", darkColor.R, darkColor.G, darkColor.B);
        }

        public PartialViewResult GetWeek(DateTime start, DateTime end, string move)
        {
            ScheduleViewModels scheduleVM = new ScheduleViewModels();
            var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
//            var newDate = cal.AddWeeks(DateTime.Now, scheduleVM.CurrentWeek);

            if (move == "prev")
            {
       
                scheduleVM.WeekStartDate = start.AddDays(-7);
            }
            else
            {
                scheduleVM.WeekStartDate = end.AddDays(7);
            }

            //var scheduleVM = new ScheduleViewModels();
//            CultureInfo cultureInfo = new CultureInfo("sv-SE");
            scheduleVM.Calendar = cal; //cultureInfo.Calendar;

            var userId = User.Identity.GetUserId();
            var courseId = db.Users.Find(userId).CourseId;

            scheduleVM.Course = new Course();
            if (!User.IsInRole(Role.Teacher))
            {
                scheduleVM.Course.Name = db.Courses.Where(co => co.Id == courseId).Select(n => n.Name).FirstOrDefault().ToString();
            }

            scheduleVM.Days = new List<Day>();
            scheduleVM.Activities = db.Activities
                    .Where(a => a.Module.Course.Users.Any(u => u.Id == userId) && a.StartDate <= scheduleVM.WeekEndDate && a.EndDate >= scheduleVM.WeekStartDate)
                    .ToArray();

            for (int i = 0; i < 7; i++)
            {
                Day day = new Day
                {
                    Date = scheduleVM.WeekStartDate.AddDays(i)
                };

                if (courseId != null)
                {

                    day.Modules = db.Courses.Find(courseId).Modules.Where(c => c.StartDate.Day == day.Date.Day).ToList();
                }
                if (day.Modules != null)
                {
                    for (int j = 0; j < day.Modules.Count; j++)
                    {
                        day.Modules[j].Activities = day.Modules[j].Activities.Where(d => d.StartDate.Day == day.Date.Day).ToList();
                    }
                }
                scheduleVM.Days.Add(day);
            }

            if (!User.Identity.IsAuthenticated)
            {
                return PartialView("_Schedule", scheduleVM);
            }

            return PartialView("_Schedule", scheduleVM);
        }
    }
}