using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TicketingSystem.Models;

namespace TicketingSystem.Web.Models.Tickets
{
	public class TicketSummaryViewModel
	{
		public int Id { get; set; }

		[Display(Name = "Title")]
		public string Title { get; set; }

		[Display(Name = "Category")]
		public string CategoryName { get; set; }

		[Display(Name = "Author")]
		public string AuthorName { get; set; }

		[Display(Name = "Priority")]
		public TicketPriority Priority { get; set; }
		
		[Display(Name = "Status")]
		public TicketStatus Status { get; set; }

		public int CommentsCount { get; set; }
	}
}