using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TicketingSystem.Models;
using TicketingSystem.Web.Helpers;
using TicketingSystem.Web.Models.Comments;
using TicketingSystem.Web.Models.Tickets;

namespace TicketingSystem.Web.Controllers
{
	[Authorize]
	public class TicketsController : BaseController
	{
		[AllowAnonymous]
		public ActionResult Index(TicketFilter filter, int? page)
		{
			var filteredTicketsList = this.ApplyFilter(filter);
			var pageIndex = page.GetValueOrDefault(1);
			var pagedTicketsList = filteredTicketsList
													  .AsEnumerable()
													  .Select(t => new TicketSummaryViewModel
															 {
																 Id = t.Id,
																 Title = t.Title,
																 CategoryName = t.Category.Name,
																 AuthorName = t.Author.UserName,
																 Priority = t.Priority,
																 Status = t.Status,
																 CommentsCount = t.Comments.Count,
																 SearchRelevance = this.CalcualteRelevance(t, filter.Q)
															 })
													  .Where(t => t.SearchRelevance > 0)
													  .OrderByDescending(t => t.SearchRelevance)
													  .ThenByDescending(t => t.Id)
													  .Skip((pageIndex - 1) * Properties.Settings.Default.TicketsPageSize)
													  .Take(Properties.Settings.Default.TicketsPageSize);

			var pagesCount = (int)Math.Ceiling((double)filteredTicketsList.Count() / Properties.Settings.Default.TicketsPageSize);
			var categoriesList = this.GetFilterList(t => new SelectListItem { Value = t.CategoryId.ToString(), Text = t.Category.Name });
			var prioritiesList = this.GetFilterList(t => new SelectListItem { Value = t.Priority.ToString(), Text = t.Priority.ToString() });
			var statusesList = this.GetFilterList(t => new SelectListItem { Value = t.Status.ToString(), Text = t.Status.ToString() });
			var authorsList = this.GetFilterList(t => new SelectListItem { Value = t.AuthorId.ToString(), Text = t.Author.UserName.ToString() });
			var titlesList = this.GetFilterList(t => new SelectListItem { Value = t.Title.ToString(), Text = t.Title.ToString() });

			var viewModel = new TicketsSummaryContainer
			{
				TicketsList = pagedTicketsList,
				Filter = filter,
				PagesCount = pagesCount,
				CategoriesList = categoriesList,
				PrioritiesList = prioritiesList,
				StatuesList = statusesList,
				AuthorsList = authorsList,
				TitlesList = titlesList
			};

			return View(viewModel);
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

			return this.RedirectToAction("Index");
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

			return this.RedirectToAction("Index");
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

		private IEnumerable<SelectListItem> GetCategoriesList(bool withEmptyValue = false)
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

		private IEnumerable<SelectListItem> GetPrioritiesList()
		{
			var priorities = from TicketPriority p in Enum.GetValues(typeof(TicketPriority))
							 select new SelectListItem
							 {
								 Value = p.ToString(),
								 Text = p.ToString()
							 };

			return priorities.ToList();
		}

		private IEnumerable<SelectListItem> GetStatusesList()
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

		private IQueryable<Ticket> ApplyFilter(TicketFilter filter)
		{
			var filteredTicketsList = this.Data.Tickets.All();

			if (filter.Title != null)
			{
				filteredTicketsList = filteredTicketsList.Where(t => t.Title == filter.Title);
			}

			if (filter.CategoryId != null)
			{
				filteredTicketsList = filteredTicketsList.Where(t => t.CategoryId == filter.CategoryId);
			}

			if (filter.AuthorId != null)
			{
				filteredTicketsList = filteredTicketsList.Where(t => t.AuthorId == filter.AuthorId);
			}

			if (filter.Priority != null)
			{
				filteredTicketsList = filteredTicketsList.Where(t => t.Priority == filter.Priority);
			}

			if (filter.Status != null)
			{
				filteredTicketsList = filteredTicketsList.Where(t => t.Status == filter.Status);
			}

			return filteredTicketsList;
		}

		private IEnumerable<SelectListItem> GetFilterList(Func<Ticket, SelectListItem> filter)
		{
			var filterList = this.Data.Tickets.All()
								 .Select(filter)
								 .Distinct(new SelectListItemComparer())
								 .ToList();

			var result = new List<SelectListItem>();
			result.Add(new SelectListItem { Text = "", Value = "" });
			result.AddRange(filterList);

			return result;
		}

		private int CalcualteRelevance(Ticket ticket, string queryString)
		{
			if (string.IsNullOrEmpty(queryString))
			{
				return 1;
			}

			var relevance = 0;
			var words = queryString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			var searchTarget = string.Format("{0}{1}{2}{3}", ticket.Title, ticket.Author.UserName, ticket.Status.ToString(), ticket.Priority.ToString()).ToLower();

			foreach (var word in words)
			{
				if (searchTarget.IndexOf(word.ToLower()) >= 0)
				{
					relevance++;
				}
			}

			return relevance;
		}
	}
}