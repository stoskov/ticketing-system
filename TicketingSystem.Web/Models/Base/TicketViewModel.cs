using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TicketingSystem.Models;
using TicketingSystem.Web.Models.Attachments;

namespace TicketingSystem.Web.Models.Base
{
	public class TicketViewModel
	{
		[Display(Name = "Identifier")]
		public int Id { get; set; }

		[Display(Name = "Title")]
		public string Title { get; set; }

		[Display(Name = "Category Identifier")]
		public int CategoryId { get; set; }

		[Display(Name = "Category")]
		public string CategoryName { get; set; }

		[Display(Name = "Author")]
		public string AuthorName { get; set; }

		[Display(Name = "Description")]
		public string Description { get; set; }
  
		[Display(Name = "Priority")]
		public TicketPriority Priority { get; set; }
  
		[Display(Name = "Status")]
		public TicketStatus Status { get; set; }

		[Display(Name = "Comments Count")]
		public int CommentsCount { get; set; }

		[Display(Name = "Files")]
		public IEnumerable<AttachmentViewModel> Attachments { get; set; }
	}
}