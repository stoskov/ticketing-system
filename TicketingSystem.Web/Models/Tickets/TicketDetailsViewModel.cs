using System;
using System.Collections.Generic;
using System.Linq;
using TicketingSystem.Models;
using TicketingSystem.Web.Models.Comments;

namespace TicketingSystem.Web.Models.Tickets
{
	public class TicketDetailsViewModel
	{
		public int Id { get; set; }

		public string AuthorName { get; set; }

		public string Title { get; set; }

		public string ScreenshotUrl { get; set; }

		public string Description { get; set; }

		public int CategoryId { get; set; }

		public string CategoryName { get; set; }

		public TicketPriority Priority { get; set; }

		public TicketStatus Status { get; set; }

		public int CommentsCount { get; set; }

		public ICollection<CommentDetailsViewModel> Comments { get; set; }
	}
}