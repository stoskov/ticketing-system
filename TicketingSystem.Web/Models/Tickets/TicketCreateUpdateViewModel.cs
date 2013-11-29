using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using TicketingSystem.Models;
using TicketingSystem.Web.Attributes;

namespace TicketingSystem.Web.Models.Tickets
{
	public class TicketCreateUpdateViewModel
	{
		public int Id { get; set; }

		[Required]
		[AllowHtml]
		[ShouldNotContainText("Bug")]
		public string Title { get; set; }

		[DataType(DataType.ImageUrl)]
		public string ScreenshotUrl { get; set; }

		[AllowHtml]
		[DataType(DataType.MultilineText)]
		public string Description { get; set; }

		[Required]
		[Display(Name = "Category")]
		public int CategoryId { get; set; }

		[Required]
		[Display(Name = "Priority")]
		public TicketPriority Priority { get; set; }

		[Required]
		[Display(Name="Status")]
		public TicketStatus Status { get; set; }

		public TicketCreateUpdateViewModel()
		{
			this.Priority = TicketPriority.Medium;
		}
	}
}