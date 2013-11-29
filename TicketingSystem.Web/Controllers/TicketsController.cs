using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TicketingSystem.Models;
using TicketingSystem.Web.Models.Comments;
using TicketingSystem.Web.Models.Tickets;

namespace TicketingSystem.Web.Controllers
{
	[Authorize]
	public class TicketsController : BaseController
	{
		private const int PageSize = 5;
  
		[AllowAnonymous]
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var ticketId = id.GetValueOrDefault();
			var ticket = this.Data.Tickets.GetById(ticketId);

			if (ticket == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);
			}

			var ticketViewModel = new TicketDetailsViewModel()
			{
				AuthorName = ticket.Author.UserName,
				CategoryName = ticket.Category.Name,
				Description = ticket.Description,
				Priority = ticket.Priority,
				ScreenshotUrl = ticket.ScreenshotUrl,
				Title = ticket.Title,
				Id = ticket.Id
			};

			ticketViewModel.Comments = ticket.Comments
											 .Select(c => new CommentViewModel()
													{
														Id = c.Id,
														UserName = c.User.UserName,
														Content = c.Content,
														TicketId = c.Ticket.Id
													})
											 .ToList();

			return this.View(ticketViewModel);
		}

		[HttpGet]
		public ActionResult Create()
		{
			this.FillCategoriesList();
			this.FillPrioritiesList();

			var ticket = new TicketCreateUpdateViewModel();

			return this.View(ticket);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(TicketCreateUpdateViewModel ticketViewModel)
		{
			if (!this.ModelState.IsValid)
			{
				this.FillCategoriesList();
				this.FillPrioritiesList();
				return this.View(ticketViewModel);
			}

			var userId = this.User.Identity.GetUserId();
			var user = this.Data.Users.GetById(userId);

			user.Points++;

			var ticket = new Ticket()
			{
				AuthorId = userId,
				CategoryId = ticketViewModel.CategoryId,
				Description = ticketViewModel.Description,
				Priority = ticketViewModel.Priority,
				ScreenshotUrl = ticketViewModel.ScreenshotUrl,
				Title = ticketViewModel.Title,
			};

			this.Data.Tickets.Add(ticket);
			this.Data.SaveChanges();

			return this.RedirectToAction("ListAll", "Tickets");
		}

		[HttpGet]
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var ticketId = id.GetValueOrDefault();
			var ticket = this.Data.Tickets.GetById(ticketId);

			if (ticket == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);
			}

			var ticketViewModel = new TicketCreateUpdateViewModel()
			{
				CategoryId = ticket.CategoryId,
				Description = ticket.Description,
				Priority = ticket.Priority,
				ScreenshotUrl = ticket.ScreenshotUrl,
				Title = ticket.Title
			};

			this.FillCategoriesList();
			this.FillPrioritiesList();

			return this.View(ticketViewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int? id, TicketCreateUpdateViewModel ticketViewModel)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var ticketId = id.GetValueOrDefault();
			var ticket = this.Data.Tickets.GetById(ticketId);

			if (ticket == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);
			}

			if (!this.ModelState.IsValid)
			{
				this.FillCategoriesList();
				this.FillPrioritiesList();
				return this.View(ticketViewModel);
			}

			ticket.CategoryId = ticketViewModel.CategoryId;
			ticket.Description = ticketViewModel.Description;
			ticket.Priority = ticketViewModel.Priority;
			ticket.ScreenshotUrl = ticketViewModel.ScreenshotUrl;
			ticket.Title = ticketViewModel.Title;

			this.Data.Tickets.Update(ticket);
			this.Data.SaveChanges();

			return this.RedirectToAction("ListAll", "Tickets");
		}

		[HttpGet]
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var ticketId = id.GetValueOrDefault();
			var ticket = this.Data.Tickets.GetById(ticketId);

			if (ticket == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);
			}

			var ticketViewModel = new TicketDeleteViewModel()
			{
				CategoryId = ticket.CategoryId,
				Description = ticket.Description,
				Priority = ticket.Priority,
				ScreenshotUrl = ticket.ScreenshotUrl,
				Title = ticket.Title
			};

			this.FillCategoriesList();
			this.FillPrioritiesList();

			return this.View(ticketViewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int? id, TicketDeleteViewModel ticketViewModel)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var ticketId = id.GetValueOrDefault();
			var ticket = this.Data.Tickets.GetById(ticketId);

			if (ticket == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);
			}

			this.Data.Tickets.Delete(ticket);
			this.Data.SaveChanges();

			return this.RedirectToAction("ListAll", "Tickets");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult PostComment(CommentViewModel commentViewModel)
		{
			if (!this.ModelState.IsValid)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var userId = this.User.Identity.GetUserId();
			var userName = this.User.Identity.GetUserName();
			var ticket = this.Data.Tickets.GetById(commentViewModel.TicketId);

			if (ticket == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);
			}

			var comment = new Comment()
			{
				UserId = userId,
				TicketId = commentViewModel.TicketId,
				Content = commentViewModel.Content
			};

			commentViewModel.UserName = userName;

			this.Data.Comments.Add(comment);
			this.Data.SaveChanges();

			return this.PartialView("_CommentPartial", commentViewModel);
		}

		public ActionResult ListAll(int? id, string categoryFilter)
		{
			int categoryId;

			if (string.IsNullOrEmpty(categoryFilter) || !int.TryParse(categoryFilter, out categoryId))
			{
				categoryId = -1;
			}

			this.FillCategoriesList(true);

			var page = id.GetValueOrDefault(1);
			var data = this.GetAllTickets(categoryId).Skip((page - 1) * PageSize).Take(PageSize);

			this.ViewBag.Pages = Math.Ceiling((double)this.GetAllTickets(categoryId).Count() / PageSize);

			return this.View(data);
		}

		private IQueryable<TicketSummaryViewModel> GetAllTickets(int categoryId)
		{
			if (categoryId >= 0)
			{
				return this.Data.Tickets.All().Where(t => t.CategoryId == categoryId).OrderByDescending(t => t.Id)
						   .Select(t => new TicketSummaryViewModel
								  {
									  Id = t.Id,
									  Title = t.Title,
									  CategoryName = t.Category.Name,
									  AuthorName = t.Author.UserName,
									  Priority = t.Priority,
									  CommentsCount = t.Comments.Count
								  });
			}
			else
			{
				return this.Data.Tickets.All().OrderByDescending(t => t.Id)
						   .Select(t => new TicketSummaryViewModel
								  {
									  Id = t.Id,
									  Title = t.Title,
									  CategoryName = t.Category.Name,
									  AuthorName = t.Author.UserName,
									  Priority = t.Priority,
									  CommentsCount = t.Comments.Count
								  });
			}
		}

		private void FillCategoriesList(bool withEmptyValue = false)
		{
			List<SelectListItem> categories = this.Data.Categories.All().ToList().Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() }).ToList();

			var categoriesList = new List<SelectListItem>();

			if (withEmptyValue)
			{
				categoriesList.Add(new SelectListItem { Text = "", Value = "" });
				categoriesList.AddRange(categories);
			}

			categoriesList.AddRange(categories);
			this.ViewBag.Categories = categoriesList;
		}

		private void FillPrioritiesList()
		{
			this.ViewBag.Priorities = from TicketPriority p in Enum.GetValues(typeof(TicketPriority))
									  select new SelectListItem { Value = p.ToString(), Text = p.ToString() };
		}
	}
}