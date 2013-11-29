using System;
using System.Linq;
using System.Web.Mvc;
using TicketingSystem.Models;
using TicketingSystem.Web.Models.Categories;
using TicketingSystem.Web.Models.Tickets;

namespace TicketingSystem.Web.Controllers
{
	public class HomeController : BaseController
	{
		public ActionResult Index()
		{
			var data = this.Data.Categories
						   .All()
						   .Select(c => new CategorySummaryViewModel()
								  {
									  Id = c.Id,
									  Name = c.Name,
									  Tickets = c.Tickets
												 .OrderByDescending(t => t.Comments.Count)
												 .Take(Properties.Settings.Default.HomePageTicketsNumber)
												 .Where(t => t.Status != TicketStatus.Closed &&
															 t.Status != TicketStatus.Duplicate)
												 .Select(t => new TicketSummaryViewModel()
														{
															Id = t.Id,
															Title = t.Title,
															CategoryName = t.Category.Name,
															AuthorName = t.Author.UserName,
															CommentsCount = t.Comments.Count,
															Priority = t.Priority,
															Status = t.Status
														})
												 .ToList()
								  })
						   .OrderByDescending(c => c.Tickets.Count())
						   .Take(Properties.Settings.Default.HomePageCategoryNumber)
						   .ToList();

			return this.View(data);
		}
	}
}