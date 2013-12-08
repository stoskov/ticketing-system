using System;
using System.Linq;
using TicketingSystem.Web.Models.Base;

namespace TicketingSystem.Web.Models.Tickets
{
	public class TicketSummaryViewModel : TicketViewModel
	{
		public int SearchRelevance { get; set; }
	}
}