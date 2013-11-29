using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TicketingSystem.Web.Models.Tickets;

namespace TicketingSystem.Web.Models.Categories
{
	public class CategorySummaryViewModel
	{
		[Key]
		public int Id { get; set; }

		[Display(Name = "Category Name")]
		public string Name { get; set; }

		public List<TicketSummaryViewModel> Tickets { get; set; }
	}
}