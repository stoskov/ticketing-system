using System;
using System.Collections.Generic;
using System.Linq;
using TicketingSystem.Web.Models.Base;
using TicketingSystem.Web.Models.Tickets;

namespace TicketingSystem.Web.Models.Categories
{
	public class CategorySummaryViewModel : CategoryViewModel
	{
		public int SearchRelevance { get; set; }

		public IEnumerable<TicketSummaryViewModel> Tickets { get; set; }
	}
}