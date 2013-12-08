using System;
using System.Collections.Generic;
using System.Linq;
using TicketingSystem.Web.Models.Base;

namespace TicketingSystem.Web.Models.Tickets
{
	public class TicketsFilterableViewModel : PagebleViewModel
	{
		public IEnumerable<TicketSummaryViewModel> TicketsList { get; set; }

		public TicketFilter Filter { get; set; }

		public TicketsMetaDataViewModel MetaData { get; set; }
	}
}