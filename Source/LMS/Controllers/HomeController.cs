using LMS.Models;
using LMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS.Controllers
{
	public class HomeController : Controller
	{
        [Authorize(Roles = "Teacher, Student")]
        public ActionResult Index()
        {
            var schedule = new ScheduleViewModels();
            CultureInfo cultureInfo = new CultureInfo("sv-SE");
            schedule.Calendar = cultureInfo.Calendar;            
            schedule.Course = new CourseModel() { Name = "Test Course" };
            return View(schedule);
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

        public PartialViewResult Module()
        {
            return PartialView("_Module");
        }
	}
}