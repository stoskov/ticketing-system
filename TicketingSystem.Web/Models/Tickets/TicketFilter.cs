using System;
using System.Linq;
using System.Web.Mvc;
using TicketingSystem.Models;
using TicketingSystem.Web.Models.Base;

namespace TicketingSystem.Web.Models.Tickets
{
	public class TicketFilter : FilterViewModel
	{
		[AllowHtml]
		public string Title { get; set; }

		public int? CategoryId { get; set; }

		public string AuthorId { get; set; }

		public TicketPriority? Priority { get; set; }

		public TicketStatus? Status { get; set; }
	}
}