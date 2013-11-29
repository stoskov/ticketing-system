using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TicketingSystem.Models;

namespace TicketingSystem.Web.Models.Tickets
{
	public class TicketDeleteViewModel
	{
		public int Id { get; set; }

		[Required]
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

		public TicketDeleteViewModel()
		{
			this.Priority = TicketPriority.Medium;
		}
	}
}