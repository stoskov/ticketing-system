using System;
using System.Linq;
using TicketingSystem.Web.Models.Base;
using TicketingSystem.Web.Models.Tickets;

namespace TicketingSystem.Web.Models.Categories
{
	public class CategoryDetailsViewModel : CategoryViewModel
	{
		public TicketsFilterableViewModel Tickets { get; set; }
	}
}