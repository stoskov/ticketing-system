using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.Web.Models.Base
{
	public class CategoryViewModel
	{
		[Display(Name = "Identifier")]
		public int Id { get; set; }

		[Required]
		[Display(Name = "Name")]
		public string Name { get; set; }

		[Display(Name="Number of Tickets")]
		public int TicketsCount { get; set; }
	}
}