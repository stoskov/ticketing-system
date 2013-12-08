using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TicketingSystem.Web.Models.Tickets
{
	public class TicketsMetaDataViewModel
	{
		public IEnumerable<SelectListItem> CategoriesList { get; set; }

		public IEnumerable<SelectListItem> TitlesList { get; set; }

		public IEnumerable<SelectListItem> AuthorsList { get; set; }

		public IEnumerable<SelectListItem> PrioritiesList { get; set; }

		public IEnumerable<SelectListItem> StatusesList { get; set; }
	}
}