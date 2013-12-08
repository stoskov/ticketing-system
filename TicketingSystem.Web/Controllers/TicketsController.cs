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
			var pageIndex = page.GetValueOrDefault(1);
			var filteredTicketsList = this.ApplyFilter(filter)
										  .AsEnumerable()
										  .Select(t => new TicketSummaryViewModel
												 {
													 Id = t.Id,
													 Title = t.Title,
													 CategoryName = t.Category.Name,
													 AuthorName = t.Author.UserName,
													 Priority = t.Priority,
													 Status = t.Status,
													 SearchRelevance = Helpers.Filter.CalcualteFilterTicketRelevance(t, filter.Q)
												 })
										  .Where(t => t.SearchRelevance > 0)
										  .OrderByDescending(t => t.SearchRelevance)
										  .ThenByDescending(t => t.Id);

			var pagedTicketsList = filteredTicketsList
													  .Skip((pageIndex - 1) * Properties.Settings.Default.TicketsPageSize)
													  .Take(Properties.Settings.Default.TicketsPageSize);

			var pagesCount = (int)Math.Ceiling((double)filteredTicketsList.Count() / Properties.Settings.Default.TicketsPageSize);
			var categoriesList = this.GetFilterList(t => new SelectListItem { Value = t.CategoryId.ToString(), Text = t.Category.Name });
			var prioritiesList = this.GetFilterList(t => new SelectListItem { Value = t.Priority.ToString(), Text = t.Priority.ToString() });
			var statusesList = this.GetFilterList(t => new SelectListItem { Value = t.Status.ToString(), Text = t.Status.ToString() });
			var authorsList = this.GetFilterList(t => new SelectListItem { Value = t.AuthorId.ToString(), Text = t.Author.UserName.ToString() });
			var titlesList = this.GetFilterList(t => new SelectListItem { Value = t.Title.ToString(), Text = t.Title.ToString() });

			var viewModel = new TicketsFilterableViewModel
			{
				TicketsList = pagedTicketsList,
				Filter = filter,
				PagesCount = pagesCount,
				TotalRecordsCount = filteredTicketsList.Count(),
				MetaData = new TicketsMetaDataViewModel
				{
					CategoriesList = categoriesList,
					PrioritiesList = prioritiesList,
					StatusesList = statusesList,
					AuthorsList = authorsList,
					TitlesList = titlesList
				}
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
				Id = ticket.Id,
				AuthorName = ticket.Author.UserName,
				CategoryName = ticket.Category.Name,
				CategoryId = ticket.Category.Id,
				Description = ticket.Description,
				Priority = ticket.Priority,
				Status = ticket.Status,
				ScreenshotUrl = ticket.ScreenshotUrl,
				Title = ticket.Title,
			};

			var commentsPageIndex = commentsPage.GetValueOrDefault(1);

			var comments = ticket.Comments
								 .Select(c => new CommentDetailsViewModel()
										{
											Id = c.Id,
											UserName = c.User.UserName,
											Content = c.Content,
											PostDate = c.PostDate,
											TicketId = c.Ticket.Id,
										})
								 .OrderByDescending(c => c.PostDate)
								 .Skip((commentsPageIndex - 1) * Properties.Settings.Default.TicketPageCommentsPageSize)
								 .Take(Properties.Settings.Default.TicketPageCommentsPageSize)
								 .ToList();

			ticketViewModel.CommentsContainer = new CommentPagebleViewModel
			{
				Comments = comments,
				CurrentPage = commentsPageIndex,
				TotalRecordsCount = comments.Count(),
				PagesCount = (int)Math.Ceiling((double)ticket.Comments.Count / Properties.Settings.Default.TicketPageCommentsPageSize)
			};

			return this.View(ticketViewModel);
		}

		[HttpGet]
		public ActionResult Create()
		{
			var ticket = new TicketCreateUpdateViewModel
			{
				MedaData = new TicketsMetaDataViewModel
				{
					CategoriesList = this.GetCategoriesList(),
					StatusesList = this.GetStatusesList(),
					PrioritiesList = this.GetPrioritiesList()
				}
			};

			return this.View(ticket);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(TicketCreateUpdateViewModel ticketViewModel)
		{
			if (!this.ModelState.IsValid)
			{
				ticketViewModel.MedaData = new TicketsMetaDataViewModel
				{
					CategoriesList = this.GetCategoriesList(),
					StatusesList = this.GetStatusesList(),
					PrioritiesList = this.GetPrioritiesList()
				};

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
				Title = ticket.Title,
				Id = ticket.Id,
				MedaData = new TicketsMetaDataViewModel
				{
					CategoriesList = this.GetCategoriesList(),
					StatusesList = this.GetStatusesList(),
					PrioritiesList = this.GetPrioritiesList()
				}
			};

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
				ticketViewModel.MedaData = new TicketsMetaDataViewModel
				{
					CategoriesList = this.GetCategoriesList(),
					StatusesList = this.GetStatusesList(),
					PrioritiesList = this.GetPrioritiesList()
				};

				return this.View(ticketViewModel);
			}

			ticket.CategoryId = ticketViewModel.CategoryId;
			ticket.Description = ticketViewModel.Description;
			ticket.Priority = ticketViewModel.Priority;
			ticket.ScreenshotUrl = ticketViewModel.ScreenshotUrl;
			ticket.Title = ticketViewModel.Title;
			ticket.Status = ticketViewModel.Status;

			this.Data.Tickets.Update(ticket);
			this.Data.SaveChanges();

			return this.RedirectToAction("Details", new { Id = ticket.Id });
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
				Id = ticket.Id
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

			return this.RedirectToAction("Index", "Home");
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

		private IEnumerable<SelectListItem> GetCategoriesList()
		{
			List<SelectListItem> categories = this.Data.Categories.All()
												  .OrderBy(c => c.Name)
												  .ToList()
												  .Select(c => new SelectListItem
														 {
															 Text = c.Name,
															 Value = c.Id.ToString()
														 })
												  .ToList();

			var categoriesList = new List<SelectListItem>(categories);

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

			priorities = priorities.OrderBy(p => p.Text);

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

			statuses = statuses.OrderBy(p => p.Text);

			return statuses.ToList();
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
	}
}