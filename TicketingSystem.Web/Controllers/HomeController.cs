using System;
using System.Linq;
using System.Web.Mvc;
using TicketingSystem.Web.Models;

namespace TicketingSystem.Web.Controllers
{
	public class HomeController : BaseController
	{
		public ActionResult Index()
		{
			var data = this.Data.Tickets.All()
						   .OrderByDescending(t => t.Comments.Count)
						   .Take(Properties.Settings.Default.HomePageTicketsNumber)
						   .Select(t => new TicketSummaryViewModel
								  {
									  Id = t.Id,
									  Title = t.Title,
									  CategoryName = t.Category.Name,
									  AuthorName = t.Author.UserName,
									  CommentsCount = t.Comments.Count,
									  Priority = t.Priority,
									  Status = t.Status
								  })
						   .ToList();

			return this.View(data);
		}
	}
}