using LMS.ViewModels.Widgets;
using System.Web.Mvc;

namespace LMS.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			/*
			var m = new TreeViewModel()
			{
					EnableLinks = true,
					Data = new[]
						{
							new TreeViewModel.TreeViewNode()
							{
								Text = "First",
								Nodes =
									{
										new TreeViewModel.TreeViewNode() { Text = "First" },
										new TreeViewModel.TreeViewNode()
											{
												Text = "Second",
												Href = Url.Action("About")
											}
									}
							},
							new TreeViewModel.TreeViewNode()
							{
								Text = "Second",
								Nodes =
									{
										new TreeViewModel.TreeViewNode() { Text = "First" },
										new TreeViewModel.TreeViewNode() { Text = "Second"}
									}
							}
						}
				};
			return View(m);
			*/

			return View();
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