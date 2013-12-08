using System;
using System.Collections.Generic;
using System.Linq;
using TicketingSystem.Web.Models.Base;

namespace TicketingSystem.Web.Models.Categories
{
	public class CategoryPagebleViewModel : PagebleViewModel
	{
		public IEnumerable<CategorySummaryViewModel> CategoriesList { get; set; }

		public FilterViewModel Filter { get; set; }
	}
}