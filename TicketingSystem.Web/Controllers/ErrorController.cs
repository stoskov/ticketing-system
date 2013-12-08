using System;
using System.Linq;
using System.Web.Mvc;

namespace TicketingSystem.Web.Controllers
{
	public class ErrorController : Controller
	{
		public ActionResult NotFound()
		{
			return View();
		}
	}
}