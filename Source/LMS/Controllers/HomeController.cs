using LMS.ViewModels.Widgets;
using LMS.DataAccess;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using static LMS.ViewModels.Widgets.TreeViewModel;
using System.Collections.Generic;

namespace LMS.Controllers
{
	public class HomeController : Controller
	{
        private ApplicationDbContext db = new ApplicationDbContext();

		public ActionResult Index()
		{
			var courses = db.Courses.Include(c => c.Modules.Select(m => m.Activities)).ToList();
			var treeData = courses.Select(c =>
				{
					var cNode = new TreeViewNode { Text = c.Name };
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
	}
}