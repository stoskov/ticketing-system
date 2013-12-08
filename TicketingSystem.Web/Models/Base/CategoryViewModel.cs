using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.Web.Models.Base
{
	public class CategoryViewModel
	{
		[Display(Name = "Identifier")]
		public int Id { get; set; }

		[Display(Name = "Name")]
		public string Name { get; set; }
	}
}