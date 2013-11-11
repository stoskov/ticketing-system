using System;
using System.Linq;
using System.Web.Mvc;
using TicketingSystem.Data.UnitsOfWork;

namespace TicketingSystem.Web.Controllers
{
	public class BaseController : Controller
	{
		protected IUowData Data;

		public BaseController(IUowData data)
		{
			this.Data = data;
		}

		public BaseController()
			: this(new UowData())
		{
		}
	}
}