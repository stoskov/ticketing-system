using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TicketingSystem.Web.Models.Tickets
{
	public class TicketsSummaryContainer
	{
		public IEnumerable<TicketSummaryViewModel> TicketsList { get; set; }

		public TicketFilter Filter { get; set; }

		public IEnumerable<SelectListItem> CategoriesList { get; set; }

		public IEnumerable<SelectListItem> TitlesList { get; set; }

		public IEnumerable<SelectListItem> AuthorsList { get; set; }

		public IEnumerable<SelectListItem> PrioritiesList { get; set; }

		public IEnumerable<SelectListItem> StatuesList { get; set; }

		public int PagesCount { get; set; }

		public int CurrentPage { get; set; }
	}
}