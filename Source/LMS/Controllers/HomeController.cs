using LMS.DataAccess;
using LMS.Models.AppData;
using LMS.ViewModels;
using LMS.ViewModels.Widgets;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
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

#region Create

		[HttpGet]
		[Authorize(Roles = Role.Teacher)]
		public PartialViewResult CreateCourse()
		{
			return PartialView();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Role.Teacher)]
		public ActionResult CreateCourse([Bind(Include = "Id,Name,Description,StartDate,EndDate")] Course course)
		{
			if (ModelState.IsValid)
			{
				db.Courses.Add(course);
				db.SaveChanges();

				return new JsonResult() { Data = new { TreeViewData = TreeView().JsonData } };
			}

			return PartialView(course);
		}

		[HttpGet]
		[Authorize(Roles = Role.Teacher)]
		public PartialViewResult CreateModule(int courseId)
		{
			ViewBag.CourseId = courseId;
			return PartialView();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Role.Teacher)]
		public ActionResult CreateModule(int courseId, [Bind(Include = "Id,Name,Description,StartDate,EndDate,ColorCode")] Module module)
		{
			var course = db.Courses.Find(courseId);
			if (ModelState.IsValid && course != null)
			{
				course.Modules.Add(module);
				db.SaveChanges();

				return new JsonResult() { Data = new { TreeViewData = TreeView().JsonData } };
			}

			return PartialView(module);
		}

		[HttpGet]
		[Authorize(Roles = Role.Teacher)]
		public PartialViewResult CreateActivity(int moduleId)
		{
			ViewBag.ModuleId = moduleId;
			ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, nameof(Activity.Id), nameof(Activity.Name));
			return PartialView(new Activity { ModuleId = moduleId });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Role.Teacher)]
		public ActionResult CreateActivity(int moduleId, [Bind(Include = "Id,Name,Description,StartDate,EndDate,ActivityTypeId,ModuleId")] Activity activity)
		{
			var module = db.Modules.Find(moduleId);
			if (ModelState.IsValid && module != null)
			{
				module.Activities.Add(activity);
				db.SaveChanges();

				return new JsonResult() { Data = new { TreeViewData = TreeView().JsonData } };
			}

			ViewBag.ModuleId = moduleId;
			ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, nameof(Activity.Id), nameof(Activity.Name), activity.ActivityTypeId);
			return PartialView(activity);
		}

#endregion // Create

#region Edit

		[Authorize(Roles = Role.Teacher)]
        public ActionResult EditCourse(int? id)
        {
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			var userId = User.Identity.GetUserId();

			var course = db.Courses.Find(id);
			if (course != null)
			{
				if (User.IsInRole(Role.Teacher) || course.Users.Any(u => u.Id.Equals(userId)))
					return PartialView(nameof(EditCourse), course);
				else
					return new HttpUnauthorizedResult();
			}
			else
				return new HttpNotFoundResult();
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCourse([Bind(Include = "Id,Name,Description,StartDate,EndDate")] Course course)
		{
			if (ModelState.IsValid)
			{
				db.Entry(course).State = EntityState.Modified;
				db.SaveChanges();

				return new JsonResult() { Data = new { TreeViewData = TreeView().JsonData } };
			}

			return PartialView(course);
		}

#endregion // Edit

#region InfoBox

		[Authorize]
		public ActionResult CourseInfo(int id)
		{
			var userId = User.Identity.GetUserId();

			var course = db.Courses.Find(id);
			if (course != null)
			{
				if (User.IsInRole(Role.Teacher) || course.Users.Any(u => u.Id.Equals(userId)))
					return PartialView("_Course", course);
				else
					return new HttpUnauthorizedResult();
			}
			else
				return new HttpNotFoundResult();
		}

		[Authorize]
		public ActionResult ModuleInfo(int id)
		{
			var userId = User.Identity.GetUserId();

			var module = db.Modules.Find(id);
			if (module != null)
			{
				if (User.IsInRole(Role.Teacher) || module.Course.Users.Any(u => u.Id.Equals(userId)))
					return PartialView("_Module", module);
				else
					return new HttpUnauthorizedResult();
			}
			else
				return new HttpNotFoundResult();
		}

		[Authorize]
		public ActionResult ActivityInfo(int id)
		{
			var userId = User.Identity.GetUserId();

			var activity = db.Activities.Find(id);
			if (activity != null)
			{
				if (User.IsInRole(Role.Teacher) || activity.Module.Course.Users.Any(u => u.Id.Equals(userId)))
					return PartialView("_Activity", activity);
				else
					return new HttpUnauthorizedResult();
			}
			else
				return new HttpNotFoundResult();
		}

#endregion // InfoBox

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
				var cNode = new TreeViewNode
				{
					Text = c.Name,
					CustomData = Url.Action(nameof(CourseInfo), new { Id = c.Id })
				};

				cNode.Nodes = c.Modules.Select(m =>
				{
					var mNode = new TreeViewNode
					{
						Text = m.Name,
						CustomData = Url.Action(nameof(ModuleInfo), new { Id = m.Id })
					};

					mNode.Nodes = m.Activities.Select(a =>
					{
						var aNode = new TreeViewNode
						{
							Text = a.Name,
							CustomData = Url.Action(nameof(ActivityInfo), new { Id = a.Id })
						};

						return aNode;
					}).ToList();
					mNode.Nodes.Add(new TreeViewNode { Text = "ADD ACTIVITY", ClassList = new[] { "node-create" }, CustomData = Url.Action(nameof(CreateActivity), new { moduleId = m.Id }) });

					return mNode;
				}).ToList();
				cNode.Nodes.Add(new TreeViewNode { Text = "ADD MODULE", ClassList = new[] { "node-create" }, CustomData = Url.Action(nameof(CreateModule), new { courseId = c.Id }) });

				return cNode;
			}).ToList();
			treeData.Add(new TreeViewNode { Text = "ADD COURSE", ClassList = new[] { "node-create" }, CustomData = Url.Action(nameof(CreateCourse)) });

			return new TreeViewModel() { Data = treeData };
		}

	}
}