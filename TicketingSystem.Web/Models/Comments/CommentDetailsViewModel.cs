using System;
using System.ComponentModel.DataAnnotations;
using TicketingSystem.Web.Models.Base;

namespace TicketingSystem.Web.Models.Comments
{
	public class CommentDetailsViewModel : PagebleViewModel
	{
		public int Id { get; set; }

		public string UserName { get; set; }

		public int TicketId { get; set; }

		[DataType(DataType.MultilineText)]
		public string Content { get; set; }

		[Display(Name = "Date")]
		public DateTime PostDate { get; set; }
	}
}