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

				return Json(new
					{
						Type = nameof(Course),
						TypeId = course.Id,
						TreeViewData = TreeView().JsonData
					});
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
		public ActionResult CreateModule(int courseId, [Bind(Include = "Id,Name,Description,StartDate,EndDate,ColorCode,CourseId")] Module module)
		{
			var course = db.Courses.Find(courseId);
			if (ModelState.IsValid && course != null)
			{
				course.Modules.Add(module);
				db.SaveChanges();

				return Json(new
					{
						Type = nameof(Module),
						TypeId = module.Id,
						TreeViewData = TreeView().JsonData
					});
			}

			return PartialView(module);
		}

		[HttpGet]
		[Authorize(Roles = Role.Teacher)]
		public PartialViewResult CreateActivity(int moduleId)
		{
			ViewBag.ModuleId = moduleId;
			ViewBag.ActivityTypeList = new SelectList(db.ActivityTypes, nameof(ActivityType.Id), nameof(ActivityType.Name));
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

				return Json(new
					{
						Type = nameof(Activity),
						TypeId = activity.Id,
						TreeViewData = TreeView().JsonData
					});
			}

			ViewBag.ModuleId = moduleId;
			ViewBag.ActivityTypeList = new SelectList(db.ActivityTypes, nameof(ActivityType.Id), nameof(ActivityType.Name), activity.ActivityTypeId);
			return PartialView(activity);
		}

#endregion // Create

#region Edit

		[HttpGet]
		[Authorize(Roles = Role.Teacher)]
        public ActionResult EditCourse(int? id)
        {
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			var course = db.Courses.SingleOrDefault(c => c.Id == id);
			if (course != null)
			{
				if (User.IsInRole(Role.Teacher)) // TODO: Is this needed?
				{
					var identityRole = db.Roles.SingleOrDefault(r => r.Name == Role.Student);
					var studentUsers = db.Users
						.Where(u => u.Roles
							.Select(r => r.RoleId)
							.Contains(identityRole.Id));

					ViewBag.Users = studentUsers;
					return PartialView(nameof(EditCourse), course);
				}
				else
					return new HttpUnauthorizedResult();
			}
			else
				return new HttpNotFoundResult();
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = Role.Teacher)]
		public ActionResult EditCourse(object[] userIds, [Bind(Include = "Id,Name,Description,StartDate,EndDate")] Course course)
		{
			if (ModelState.IsValid)
			{
				var identityRole = db.Roles.SingleOrDefault(r => r.Name == Role.Student);
				var studentUsers = db.Users
					.Where(u => u.Roles
						.Select(r => r.RoleId)
						.Contains(identityRole.Id))
					.ToArray()
					.Where(u => userIds.Contains(u.Id) || u.CourseId == course.Id);

				// TODO: Verification for removed users?
				foreach (var u in studentUsers)
				{
					if (userIds.Contains(u.Id))
						u.CourseId = course.Id;
					else if (u.CourseId == course.Id)
						u.CourseId = null;

					db.Entry(u).State = EntityState.Modified;
				}

				db.Entry(course).State = EntityState.Modified;
				db.SaveChanges();

				return Json(new
				{
					Type = nameof(Course),
					TypeId = course.Id,
					TreeViewData = TreeView().JsonData
				});
			}

			return PartialView(course);
		}

		[HttpGet]
		[Authorize(Roles = Role.Teacher)]
		public ActionResult EditModule(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			var module = db.Modules.Find(id);
			if (module != null)
			{
				if (User.IsInRole(Role.Teacher))
				{
					ViewBag.CoursesList = new SelectList(db.Courses, nameof(Course.Id), nameof(Course.Name), module.CourseId);
					return PartialView(nameof(EditModule), module);
				}
				else
					return new HttpUnauthorizedResult();
			}
			else
				return new HttpNotFoundResult();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Role.Teacher)]
		public ActionResult EditModule([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseId")] Module module)
		{
			if (ModelState.IsValid)
			{
				db.Entry(module).State = EntityState.Modified;
				db.SaveChanges();

				return Json(new
				{
					Type = nameof(Module),
					TypeId = module.Id,
					TreeViewData = TreeView().JsonData
				});
			}

			ViewBag.CoursesList = new SelectList(db.Courses, nameof(Course.Id), nameof(Course.Name), module.CourseId);
			return PartialView(module);
		}

		[HttpGet]
		[Authorize(Roles = Role.Teacher)]
		public ActionResult EditActivity(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			var activity = db.Activities.Find(id);
			if (activity != null)
			{
				if (User.IsInRole(Role.Teacher))
				{
					ViewBag.ModulesList = new SelectList(db.Modules, nameof(Module.Id), nameof(Module.Name), activity.ModuleId);
					ViewBag.ActivityTypeList = new SelectList(db.ActivityTypes, nameof(ActivityType.Id), nameof(ActivityType.Name), activity.ActivityTypeId);
					return PartialView(nameof(EditActivity), activity);
				}
				else
					return new HttpUnauthorizedResult();
			}
			else
				return new HttpNotFoundResult();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Role.Teacher)]
		public ActionResult EditActivity([Bind(Include = "Id,Name,Description,StartDate,EndDate,ActivityTypeId,ModuleId")] Activity activity)
		{
			if (ModelState.IsValid)
			{
				db.Entry(activity).State = EntityState.Modified;
				db.SaveChanges();

				return Json(new
				{
					Type = nameof(Activity),
					TypeId = activity.Id,
					TreeViewData = TreeView().JsonData
				});
			}

			ViewBag.ModulesList = new SelectList(db.Modules, nameof(Module.Id), nameof(Module.Name), activity.ModuleId);
			ViewBag.ActivityTypeList = new SelectList(db.ActivityTypes, nameof(ActivityType.Id), nameof(ActivityType.Name), activity.ActivityTypeId);
			return PartialView(activity);
		}

		#endregion // Edit


#region Delete

		[HttpGet]
		[Authorize(Roles = Role.Teacher)]
        public ActionResult DeleteCourse(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			var course = db.Courses.Find(id);
			if (course != null)
			{
				if (User.IsInRole(Role.Teacher))
				{
					// TODO: Add popup verification instead of DeleteConfirmed?

					return PartialView(nameof(DeleteCourse), course);
				}
				else
					return new HttpUnauthorizedResult();
			}
			else
				return new HttpNotFoundResult();
        }

        [HttpPost, ActionName(nameof(DeleteCourse))]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = Role.Teacher)]
		public ActionResult DeleteCourseConfirmed(int id)
        {
			var course = db.Courses.Find(id);
			if (course != null)
			{
				db.Courses.Remove(course);
				db.SaveChanges();

				return Json(new
				{
					Type = nameof(Course),
					TypeId = 0,
					TreeViewData = TreeView().JsonData
				});
			}

			return new HttpNotFoundResult();
		}
		
		[HttpGet]
		[Authorize(Roles = Role.Teacher)]
        public ActionResult DeleteModule(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			var module = db.Modules.Find(id);
			if (module != null)
			{
				if (User.IsInRole(Role.Teacher))
				{
					// TODO: Add popup verification instead of DeleteConfirmed?

					return PartialView(nameof(DeleteModule), module);
				}
				else
					return new HttpUnauthorizedResult();
			}
			else
				return new HttpNotFoundResult();
        }

        [HttpPost, ActionName(nameof(DeleteModule))]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = Role.Teacher)]
		public ActionResult DeleteModuleConfirmed(int id)
        {
			var module = db.Modules.Find(id);
			if (module != null)
			{
				db.Modules.Remove(module);
				db.SaveChanges();

				return Json(new
				{
					Type = nameof(Course),
					TypeId = module.CourseId, // select parent Course
					TreeViewData = TreeView().JsonData
				});
			}

			return new HttpNotFoundResult();
		}

		[HttpGet]
		[Authorize(Roles = Role.Teacher)]
        public ActionResult DeleteActivity(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			var activity = db.Activities.Find(id);
			if (activity != null)
			{
				if (User.IsInRole(Role.Teacher))
				{
					// TODO: Add popup verification instead of DeleteConfirmed?

					return PartialView(nameof(DeleteActivity), activity);
				}
				else
					return new HttpUnauthorizedResult();
			}
			else
				return new HttpNotFoundResult();
        }

        [HttpPost, ActionName(nameof(DeleteActivity))]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = Role.Teacher)]
		public ActionResult DeleteActivityConfirmed(int id)
        {
			var activity = db.Activities.Find(id);
			if (activity != null)
			{
				db.Activities.Remove(activity);
				db.SaveChanges();

				return Json(new
				{
					Type = nameof(Module),
					TypeId = activity.ModuleId, // select parent Module
					TreeViewData = TreeView().JsonData
				});
			}

			return new HttpNotFoundResult();
		}

#endregion // Delete

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
					CustomData = new { Type = nameof(Course), Id = c.Id, Action = Url.Action(nameof(CourseInfo), new { Id = c.Id }) }
				};

				cNode.Nodes = c.Modules.Select(m =>
				{
					var mNode = new TreeViewNode
					{
						Text = m.Name,
						CustomData = new { Type = nameof(Module), Id = m.Id, Action = Url.Action(nameof(ModuleInfo), new { Id = m.Id }) }
					};

					mNode.Nodes = m.Activities.Select(a =>
					{
						var aNode = new TreeViewNode
						{
							Text = a.Name,
							CustomData = new { Type = nameof(Activity), Id = a.Id, Action = Url.Action(nameof(ActivityInfo), new { Id = a.Id }) }
						};

						return aNode;
					}).ToList();
					mNode.Nodes.Add(new TreeViewNode
						{
							Text = "ADD ACTIVITY",
							ClassList = new[] { "node-create" },
							CustomData = new { Type = nameof(Activity), Action = Url.Action(nameof(CreateActivity), new { moduleId = m.Id }) }
						});

					return mNode;
				}).ToList();
				cNode.Nodes.Add(new TreeViewNode
					{
						Text = "ADD MODULE",
						ClassList = new[] { "node-create" },
						CustomData = new { Type = nameof(Module), Action = Url.Action(nameof(CreateModule), new { courseId = c.Id }) }
					});

				return cNode;
			}).ToList();
			treeData.Add(new TreeViewNode
				{
					Text = "ADD COURSE",
					ClassList = new[] { "node-create" },
					CustomData = new { Type = nameof(Course), Action = Url.Action(nameof(CreateCourse)) }
				});

			return new TreeViewModel() { Data = treeData };
		}

	}
}