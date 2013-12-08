using System;
using System.Linq;
using System.Web.Mvc;

namespace TicketingSystem.Web.Models.Base
{
	public class FilterViewModel
	{
		[AllowHtml]
		public string Q { get; set; }
	}
}