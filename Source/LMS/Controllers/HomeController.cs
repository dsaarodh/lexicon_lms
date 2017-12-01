using LMS.DataAccess;
using LMS.Models.AppData;
using LMS.ViewModels;
using LMS.ViewModels.Widgets;
using Microsoft.AspNet.Identity;
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

                
            //scheduleVM.Course = new Course() { Name = "Test Course" };
            



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

        public PartialViewResult Activity()
        {            
            Activity activity = db.Activities.Where(a => a.Id == 1).FirstOrDefault();

            return PartialView("_Activity", activity);
        }

        public PartialViewResult Course()
        {
            Course course = db.Courses.Where(c => c.Id == 1).FirstOrDefault();
            return PartialView("_Course", course);
        }

        public PartialViewResult Module()
        {
            Module module = db.Modules.Where(m => m.Id == 1).FirstOrDefault();
            module.ColorCode = "#070707";
            return PartialView("_Module", module);
        }
    }
}