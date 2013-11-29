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
		[AllowAnonymous]
		public ActionResult Index(TicketFilter ticketFilter)
		{
			int categoryId = ticketFilter.CategoryId.GetValueOrDefault(-1);

			this.GetCategoriesList(true);

			var page = 1;
			var data = this.GetAllTickets(categoryId).Skip((page - 1) * Properties.Settings.Default.TicketsPageSize).Take(Properties.Settings.Default.TicketsPageSize);

			this.ViewBag.Pages = Math.Ceiling((double)this.GetAllTickets(categoryId).Count() / Properties.Settings.Default.TicketsPageSize);
			this.ViewBag.Filter = ticketFilter;

			return this.View(data);
		}

		[AllowAnonymous]
		public ActionResult Details(int? id, int? commentsPage)
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
				CategoryId = ticket.Category.Id,
				Description = ticket.Description,
				Priority = ticket.Priority,
				Status = ticket.Status,
				ScreenshotUrl = ticket.ScreenshotUrl,
				Title = ticket.Title,
				Id = ticket.Id,
				CommentsCount = ticket.Comments.Count
			};

			var commentsPageIndex = commentsPage.GetValueOrDefault(1);

			ticketViewModel.Comments = ticket.Comments
											 .Select(c => new CommentDetailsViewModel()
													{
														Id = c.Id,
														UserName = c.User.UserName,
														Content = c.Content,
														PostDate = c.PostDate,
														TicketId = c.Ticket.Id
													})
											 .OrderByDescending(c => c.PostDate)
											 .Skip((commentsPageIndex - 1) * Properties.Settings.Default.TicketPageCommentsPageSize)
											 .Take(Properties.Settings.Default.TicketPageCommentsPageSize)
											 .ToList();

			this.ViewBag.CurrentPage = commentsPageIndex;
			this.ViewBag.PagesCount = (int)Math.Ceiling((double)ticket.Comments.Count / Properties.Settings.Default.TicketPageCommentsPageSize);

			return this.View(ticketViewModel);
		}

		[HttpGet]
		public ActionResult Create()
		{
			this.SetLists();
			var ticket = new TicketCreateUpdateViewModel();

			return this.View(ticket);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(TicketCreateUpdateViewModel ticketViewModel)
		{
			if (!this.ModelState.IsValid)
			{
				SetLists();

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

			return this.RedirectToAction("Index");
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
				Status = ticket.Status,
				ScreenshotUrl = ticket.ScreenshotUrl,
				Title = ticket.Title
			};

			this.SetLists();

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
				this.SetLists();

				return this.View(ticketViewModel);
			}

			ticket.CategoryId = ticketViewModel.CategoryId;
			ticket.Description = ticketViewModel.Description;
			ticket.Priority = ticketViewModel.Priority;
			ticket.ScreenshotUrl = ticketViewModel.ScreenshotUrl;
			ticket.Title = ticketViewModel.Title;

			this.Data.Tickets.Update(ticket);
			this.Data.SaveChanges();

			return this.RedirectToAction("Index", "Tickets");
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

			var ticketViewModel = new TicketDetailsViewModel()
			{
				AuthorName = ticket.Author.UserName,
				CategoryName = ticket.Category.Name,
				CategoryId = ticket.Category.Id,
				Description = ticket.Description,
				Priority = ticket.Priority,
				Status = ticket.Status,
				ScreenshotUrl = ticket.ScreenshotUrl,
				Title = ticket.Title,
				Id = ticket.Id,
				CommentsCount = ticket.Comments.Count
			};

			return this.View(ticketViewModel);
		}
  
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int? id, TicketDetailsViewModel ticketViewModel)
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

			return this.RedirectToAction("Index", "Tickets");
		}
  
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult PostComment(CommentDetailsViewModel commentViewModel)
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

			return this.RedirectToAction("Details", new { Id = comment.TicketId });
		}
  
		private IQueryable<TicketSummaryViewModel> GetAllTickets(int categoryId)
		{
			if (categoryId >= 0)
			{
				return this.Data.Tickets.All()
						   .Where(t => t.CategoryId == categoryId)
						   .OrderByDescending(t => t.Id)
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
				return this.Data.Tickets.All()
						   .OrderByDescending(t => t.Id)
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
  
		private List<SelectListItem> GetCategoriesList(bool withEmptyValue = false)
		{
			List<SelectListItem> categories = this.Data.Categories.All()
												  .ToList()
												  .Select(c => new SelectListItem
														 {
															 Text = c.Name,
															 Value = c.Id.ToString()
														 })
												  .ToList();

			var categoriesList = new List<SelectListItem>();

			if (withEmptyValue)
			{
				categoriesList.Add(new SelectListItem { Text = "", Value = "" });
				categoriesList.AddRange(categories);
			}

			categoriesList.AddRange(categories);

			return categoriesList;
		}
  
		private List<SelectListItem> GetPrioritiesList()
		{
			var priorities = from TicketPriority p in Enum.GetValues(typeof(TicketPriority))
							 select new SelectListItem
							 {
								 Value = p.ToString(),
								 Text = p.ToString()
							 };

			return priorities.ToList();
		}

		private List<SelectListItem> GetStatusesList()
		{
			var statuses = from TicketStatus p in Enum.GetValues(typeof(TicketStatus))
						   select new SelectListItem
						   {
							   Value = p.ToString(),
							   Text = p.ToString()
						   };

			return statuses.ToList();
		}
  
		private void SetLists()
		{
			ViewBag.CategoriesList = this.GetCategoriesList();
			ViewBag.StatusesList = this.GetStatusesList();
			ViewBag.PrioritiesList = this.GetPrioritiesList();
		}
	}
}