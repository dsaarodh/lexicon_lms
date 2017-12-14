using LMS.DataAccess;
using LMS.Models.AppData;
using LMS.Models.AppData.Base;
using LMS.Models.Identity;
using LMS.ViewModels;
using LMS.ViewModels.Widgets;
using Microsoft.AspNet.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Drawing;
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

        [Authorize(Roles = Role.Teacher + "," + Role.Student)]
        public ActionResult Index()
        {
			var vm = BuildScheduleViewModel(DateTime.Now);
			vm.TreeView = TreeView();

            return View(vm);
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


        [Authorize(Roles = Role.Teacher)]
        public ActionResult UserManager()
        {
            var roleStore = new RoleStore<IdentityRole>(db);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var roles = roleManager.Roles.ToList();

            var userStore = new UserStore<Models.Identity.ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var model = db.Users.Include(p => p.Course).Select(u => new UserManagerViewModel
            {
                Id = u.Id,
                LastName = u.LastName,
                FirstName = u.FirstName,
                Email = u.Email,
                Course = u.Course.Name
            }
            ).ToList();

            foreach (var umv in model)
            {
                var rolesForId = userManager.GetRoles(umv.Id);
                string temp = "";

                foreach (var r in rolesForId)
                {
                    temp += r + " ";
                }

                umv.Role = temp;
            }

            return View(model);
        }

		public PartialViewResult Schedule(DateTime dayInWeek)
        {
			return PartialView("_Schedule", BuildScheduleViewModel(dayInWeek));
        }

		public string ColorShade(string color)
        {
            var c = System.Drawing.ColorTranslator.FromHtml(color);
            System.Drawing.Color darkColor = System.Drawing.Color.FromArgb(1,  Math.Min(c.R - 50, 255), Math.Min(c.G - 50, 255), Math.Min(c.B - 50, 255));
            return String.Format("#{0:X2}{1:X2}{2:X2}", darkColor.R, darkColor.G, darkColor.B);
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
		public ActionResult CreateCourse([Bind] Course course)
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
		public ActionResult CreateModule(int courseId, [Bind] Module module)
		{
			ValidateTimeInterval(module);

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
		public ActionResult CreateActivity(int moduleId, [Bind] Activity activity)
		{
			ValidateTimeInterval(activity);

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
				var identityRole = db.Roles.SingleOrDefault(r => r.Name == Role.Student);
				var studentUsers = db.Users
					.Where(u => u.Roles
						.Select(r => r.RoleId)
						.Contains(identityRole.Id));

				ViewBag.Users = studentUsers;
				return PartialView(nameof(EditCourse), course);
			}
			else
				return new HttpNotFoundResult();
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = Role.Teacher)]
		public ActionResult EditCourse(object[] userIds, [Bind] Course course)
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
				ViewBag.CoursesList = new SelectList(db.Courses, nameof(Course.Id), nameof(Course.Name), module.CourseId);
				return PartialView(nameof(EditModule), module);
			}
			else
				return new HttpNotFoundResult();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Role.Teacher)]
		public ActionResult EditModule([Bind] Module module)
		{
			ValidateTimeInterval(module);

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
				ViewBag.ModulesList = new SelectList(db.Modules, nameof(Module.Id), nameof(Module.Name), activity.ModuleId);
				ViewBag.ActivityTypeList = new SelectList(db.ActivityTypes, nameof(ActivityType.Id), nameof(ActivityType.Name), activity.ActivityTypeId);
				return PartialView(nameof(EditActivity), activity);
			}
			else
				return new HttpNotFoundResult();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Role.Teacher)]
		public ActionResult EditActivity([Bind] Activity activity)
		{
			ValidateTimeInterval(activity);

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
				// TODO: Add popup verification instead of DeleteConfirmed?
				return PartialView(nameof(DeleteCourse), course);
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
				// TODO: Add popup verification instead of DeleteConfirmed?
				return PartialView(nameof(DeleteModule), module);
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
				// TODO: Add popup verification instead of DeleteConfirmed?
				return PartialView(nameof(DeleteActivity), activity);
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

			var treeData = courses
				.OrderBy(c => c.Name)
				.Select(c =>
					{
						var cNode = new TreeViewNode
							{
                                Text = c.Name,
								CustomData = new { Type = nameof(Course), Id = c.Id, Action = Url.Action(nameof(CourseInfo), new { Id = c.Id }) }
							};

						cNode.Nodes = c.Modules
							.OrderBy(m => m.Name)
							.Select(m =>
								{
									var mNode = new TreeViewNode
										{
											Text = m.Name,
											CustomData = new { Type = nameof(Module), Id = m.Id, Action = Url.Action(nameof(ModuleInfo), new { Id = m.Id }) }
										};


									mNode.Nodes = m.Activities
										.OrderBy(a => a.StartDate)
										.ThenBy(a => a.EndDate)
										.ThenBy(a => a.Name)
										.Select(a =>
											{
												var aNode = new TreeViewNode
													{
														Text = a.Name,
														CustomData = new { Type = nameof(Activity), Id = a.Id, Action = Url.Action(nameof(ActivityInfo), new { Id = a.Id }) }
													};

												return aNode;
											}).ToList();

									if (isTeacher)
									{ 
										mNode.Nodes.Add(new TreeViewNode
											{
												Text = " Lägg till aktivitet",
												ClassList = new[] { "node-create" },
												Icon = "glyphicon glyphicon-log-in",
												CustomData = new { Type = nameof(Activity), Action = Url.Action(nameof(CreateActivity), new { moduleId = m.Id }) }
											});
									}

									return mNode;
								}).ToList();

						if (isTeacher)
						{ 
							cNode.Nodes.Add(new TreeViewNode
								{
									Text = " Lägg till modul",
									ClassList = new[] { "node-create" },
									Icon = "glyphicon glyphicon-log-in",
									CustomData = new { Type = nameof(Module), Action = Url.Action(nameof(CreateModule), new { courseId = c.Id }) }
								});
						}

						return cNode;
					}).ToList();

			if (isTeacher)
			{ 
				treeData.Add(new TreeViewNode
					{
						Text = " Lägg till kurs",
						ClassList = new[] { "node-create" },
						Icon = "glyphicon glyphicon-log-in",
						CustomData = new { Type = nameof(Course), Action = Url.Action(nameof(CreateCourse)) }
					});
			}

			return new TreeViewModel() { Data = treeData };
		}

		private void ValidateTimeInterval(Course model)
		{
			if (model.StartDate > model.EndDate)
				ModelState.AddModelError(nameof(Course.StartDate), "Start datum måste vara mindre än slut datum.");

			// check if activity time interval overlap any other activity from the same module
			var set = db.Set<Course>().AsNoTracking();
			var overlaps = set.Where(m => m.Id != model.Id && !(model.EndDate < m.StartDate || model.StartDate > m.EndDate));
			if (overlaps.Count() > 0)
			{            
				ModelState.AddModelError(nameof(Module.StartDate), "Tidsintervallet överlappar med en annan kurs.");
				ModelState.AddModelError(nameof(Module.EndDate), "Tidsintervallet överlappar med en annan kurs.");
			}
        }

		private void ValidateTimeInterval(Module model)
		{
			if (model.StartDate > model.EndDate)
				ModelState.AddModelError(nameof(Module.StartDate), "Start datum måste vara mindre än slut datum.");

			var parent = db.Courses.Find(model.CourseId);
			if (model.StartDate < parent.StartDate)
				ModelState.AddModelError(nameof(Module.StartDate), "Angivet start datum är tidigare än kursens start datum.");
			if (model.EndDate > parent.EndDate)
				ModelState.AddModelError(nameof(Module.EndDate), "Angivet slut datum är senare än kursens slut datum.");

			// check if activity time interval overlap any other activity from the same module
			var set = db.Set<Module>().AsNoTracking();
			var overlaps = set.Where(m => m.Id != model.Id && m.CourseId == model.CourseId && !(model.EndDate < m.StartDate || model.StartDate > m.EndDate));
			if (overlaps.Count() > 0)
			{
				ModelState.AddModelError(nameof(Module.StartDate), "Tidsintervallet överlappar med en annan modul.");
				ModelState.AddModelError(nameof(Module.EndDate), "Tidsintervallet överlappar med en annan modul.");
			}
        }

		private void ValidateTimeInterval(Activity model)
		{
			if (model.StartDate > model.EndDate)
				ModelState.AddModelError(nameof(Module.StartDate), "Start datum måste vara mindre än slut datum.");

			var parent = db.Modules.Find(model.ModuleId);
			if (model.StartDate < parent.StartDate)
				ModelState.AddModelError(nameof(Module.StartDate), "Angivet start datum är tidigare än modulens start datum.");
			if (model.EndDate > parent.EndDate)
				ModelState.AddModelError(nameof(Module.EndDate), "Angivet slut datum är senare än modulens slut datum.");

			// check if activity time interval overlap any other activity from the same module
			var set = db.Set<Activity>().AsNoTracking();
			var overlaps = set.Where(m => m.Id != model.Id && m.ModuleId == model.ModuleId && !(model.EndDate < m.StartDate || model.StartDate > m.EndDate));
			if (overlaps.Count() > 0)
			{
				ModelState.AddModelError(nameof(Module.StartDate), "Tidsintervallet överlappar med en annan aktivitet.");
				ModelState.AddModelError(nameof(Module.EndDate), "Tidsintervallet överlappar med en annan aktivitet.");
			}
        }

		private ScheduleViewModels BuildScheduleViewModel(DateTime dayInWeek)
		{
			ScheduleViewModels scheduleVM = new ScheduleViewModels(dayInWeek);

			var userId = User.Identity.GetUserId();
			var courseId = db.Users.Find(userId).CourseId;

			if (!User.IsInRole(Role.Teacher))
				scheduleVM.Course = db.Courses.Find(courseId);

			scheduleVM.Days = new List<Day>();
			scheduleVM.Activities = db.Activities
					.Where(a => a.Module.Course.Users.Any(u => u.Id == userId) && a.StartDate <= scheduleVM.WeekEndDate && a.EndDate >= scheduleVM.WeekStartDate)
					.ToArray();

			for (int i = 0; i < 7; i++)
			{
				Day day = new Day { Date = scheduleVM.WeekStartDate.AddDays(i) };

				if (courseId != null)
					day.Modules = db.Courses.Find(courseId).Modules.Where(c => c.StartDate.Day == day.Date.Day).ToList();

				if (day.Modules != null)
				{
					for (int j = 0; j < day.Modules.Count; j++)
					{
						day.Modules[j].Activities = day.Modules[j].Activities.Where(d => d.StartDate.Day == day.Date.Day).ToList();
					}
				}

				scheduleVM.Days.Add(day);
			}

			return scheduleVM;
		}
    }
}