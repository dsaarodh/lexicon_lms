using LMS.ViewModels.Widgets;
using LMS.DataAccess;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using static LMS.ViewModels.Widgets.TreeViewModel;
using System.Collections.Generic;
using LMS.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;

namespace LMS.Controllers
{
	public class HomeController : Controller
	{
        private ApplicationDbContext db = new ApplicationDbContext();

		public ActionResult Index()
		{
            if (!User.Identity.IsAuthenticated)
                return View(new TreeViewModel());

            var userId = User.Identity.GetUserId();
            var courseId = db.Users.Find(userId).CourseId;
            bool isTeacher = User.IsInRole(Role.Teacher);

            var courses = db.Courses
                .Where(co => isTeacher ? true : co.Id == courseId)
                .Include(c => c.Modules.Select(m => m.Activities)).ToList();

            var treeData = courses.Select(c =>
            {
                var cNode = new TreeViewNode { Text = c.Name, Selectable = true, CustomData = Url.Action(nameof(CourseInfo), new { Id = c.Id }) };
                cNode.Nodes = c.Modules.Select(m =>
                {
                    var node = new TreeViewNode { Text = m.Name, Selectable = true, CustomData = Url.Action(nameof(ModuleInfo), new { Id = m.Id }) };
					node.Nodes = m.Activities.Select(a => new TreeViewNode { Text = a.Name, Selectable = true, CustomData = Url.Action(nameof(ActivityInfo), new { Id = a.Id }) }).ToArray();
                    return node;
                }).ToArray();
                return cNode;
            }).ToArray();

			var model = new TreeViewModel()
			{
				Data = treeData
			};

			return View(model);
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


		public JsonResult CourseInfo(int id)
		{
			return new JsonResult() { Data = "Courses/" + id, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		public JsonResult ModuleInfo(int id)
		{
			return new JsonResult() { Data = "Modules/" + id, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		public JsonResult ActivityInfo(int id)
		{
			return new JsonResult() { Data = "Activities/" + id, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

	}
}