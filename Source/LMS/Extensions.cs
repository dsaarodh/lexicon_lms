using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace LMS
{
	/// <summary>
	/// Html helpers
	/// Shamelessly grabbed from https://stackoverflow.com/questions/5433531/using-sections-in-editor-display-templates/5433722#5433722
	/// </summary>
	public static class HtmlExtensions
	{
		public static MvcHtmlString Script(this HtmlHelper htmlHelper, Func<object, HelperResult> template)
		{
			htmlHelper.ViewContext.HttpContext.Items["_script_" + Guid.NewGuid()] = template;
			return MvcHtmlString.Empty;
		}

		public static IHtmlString RenderScripts(this HtmlHelper htmlHelper)
		{
			foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
			{
				if (key.ToString().StartsWith("_script_"))
				{
					var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, HelperResult>;
					if (template != null)
					{
						htmlHelper.ViewContext.Writer.Write(template(null));
					}
				}
			}
			return MvcHtmlString.Empty;
		}
	}

	/// <summary>
	/// Controller helpers/extensions
	/// </summary>
	public static class ControllerExtensions
	{
		public static string ViewToString(this Controller controller, string viewName, object model)
		{
			using (var sw = new StringWriter())
			{
				controller.ViewData.Model = model;
				var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
				var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
				viewResult.View.Render(viewContext, sw);
				viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);

				return sw.GetStringBuilder().ToString();
			}
		}
	}
}