using System;
using System.Linq;

namespace TicketingSystem.Web.Models.Base
{
	public class PagebleViewModel
	{
		public int PagesCount { get; set; }

		public int CurrentPage { get; set; }

		public int TotalRecordsCount { get; set; }
	}
}