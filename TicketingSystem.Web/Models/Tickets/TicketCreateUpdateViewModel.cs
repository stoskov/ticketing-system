using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TicketingSystem.Models;
using TicketingSystem.Web.Attributes;

namespace TicketingSystem.Web.Models.Tickets
{
	public class TicketCreateUpdateViewModel
	{
		public int Id { get; set; }

		[Required]
		[ShouldNotContainText("Bug")]
		public string Title { get; set; }

		[DataType(DataType.ImageUrl)]
		public string ScreenshotUrl { get; set; }

		[DataType(DataType.MultilineText)]
		public string Description { get; set; }

		[Required]
		[Display(Name = "Category")]
		public int CategoryId { get; set; }

		[Required]
		public TicketPriority Priority { get; set; }

		public TicketCreateUpdateViewModel()
		{
			this.Priority = TicketPriority.Medium;
		}
	}
}