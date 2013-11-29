using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TicketingSystem.Models;

namespace TicketingSystem.Web.Models.Tickets
{
	public class TicketFilter
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public int? CategoryId { get; set; }

		[Display(Name = "Author")]
		public string AuthorName { get; set; }

		[Display(Name = "Priority")]
		public TicketPriority Priority { get; set; }
		
		[Display(Name = "Status")]
		public TicketStatus Status { get; set; }

		public int CommentsCount { get; set; }
	}
}