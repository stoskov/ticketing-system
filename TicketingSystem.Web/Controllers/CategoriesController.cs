using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TicketingSystem.Models;
using TicketingSystem.Web.Models.Base;
using TicketingSystem.Web.Models.Categories;
using TicketingSystem.Web.Models.Tickets;

namespace TicketingSystem.Web.Controllers
{
	public class CategoriesController : BaseController
	{
		public ActionResult Index(FilterViewModel filter, int? page)
		{
			var pageIndex = page.GetValueOrDefault(1);
			var filteredCategoriesList = this.Data.Categories.All()
											 .AsEnumerable()
											 .Select(c => new CategorySummaryViewModel
													{
														Id = c.Id,
														Name = c.Name,
														SearchRelevance = Helpers.Filter.CalcualteFilterCategoryRelevance(c, filter.Q)
													})
											 .Where(c => c.SearchRelevance > 0)
											 .OrderByDescending(c => c.SearchRelevance)
											 .ThenByDescending(c => c.Name);

			var pagedCategoriesList = filteredCategoriesList
															.Skip((pageIndex - 1) * Properties.Settings.Default.CategoriesPageSize)
															.Take(Properties.Settings.Default.CategoriesPageSize);

			var pagesCount = (int)Math.Ceiling((double)filteredCategoriesList.Count() / Properties.Settings.Default.CategoriesPageSize);

			var viewModel = new CategoryPagebleViewModel
			{
				CategoriesList = pagedCategoriesList,
				Filter = filter,
				PagesCount = pagesCount,
				TotalRecordsCount = filteredCategoriesList.Count()
			};

			return View(viewModel);
		}

		public ActionResult Details(int? id, int? ticketsPage)
		{
			return this.ReturnCategoryView(id, ticketsPage);
		}

		[HttpGet]
		public ActionResult Create()
		{
			var category = new CategoryViewModel();

			return View(category);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CategoryViewModel categoryViewModel)
		{
			return ManipulaateCategory(0, false,
				c =>
				{
					var category = new Category
					{
						Name = categoryViewModel.Name
					};

					this.Data.Categories.Add(category);
				});
		}

		[HttpGet]
		public ActionResult Edit(int? id)
		{
			return ReturnCategoryView(id, 0);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int? id, CategoryViewModel categoryViewModel)
		{
			return ManipulaateCategory(id, true,
				c =>
				{
					c.Name = categoryViewModel.Name;

					this.Data.Categories.Update(c);
				});
		}

		[HttpGet]
		public ActionResult Delete(int? id, int? ticketsPage)
		{
			return this.ReturnCategoryView(id, ticketsPage);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int? id, CategoryViewModel categoryViewModel)
		{
			return ManipulaateCategory(id, true,
				c =>
				{
					this.Data.Categories.Delete(c);
				});
		}

		private ActionResult ManipulaateCategory(int? id, bool checkExistance, Action<Category> action)
		{
			if (checkExistance && id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var category = this.Data.Categories.GetById((int)id);

			if (checkExistance && category == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);
			}

			action(category);
			this.Data.SaveChanges();

			return RedirectToAction("Index");
		}

		private ActionResult ReturnCategoryView(int? id, int? ticketsPage)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var category = this.Data.Categories.GetById((int)id);

			if (category == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);
			}

			var pageIndex = ticketsPage.GetValueOrDefault(1);
			var ticketsList = this.Data.Tickets.All()
								  .Where(t => t.CategoryId == category.Id)
								  .AsEnumerable()
								  .Select(t => new TicketSummaryViewModel
										 {
											 Id = t.Id,
											 Title = t.Title,
											 CategoryName = t.Category.Name,
											 AuthorName = t.Author.UserName,
											 Priority = t.Priority,
											 Status = t.Status
										 })
								  .OrderByDescending(t => t.SearchRelevance)
								  .ThenByDescending(t => t.Id);

			var pagedTicketsList = ticketsList
											  .Skip((pageIndex - 1) * Properties.Settings.Default.TicketsPageSize)
											  .Take(Properties.Settings.Default.TicketsPageSize);

			var pagesCount = (int)Math.Ceiling((double)ticketsList.Count() / Properties.Settings.Default.TicketsPageSize);

			var ticketsViewModel = new TicketsFilterableViewModel
			{
				TicketsList = pagedTicketsList,
				PagesCount = pagesCount,
				TotalRecordsCount = ticketsList.Count()
			};

			var viewModel = new CategoryDetailsViewModel
			{
				Id = category.Id,
				Name = category.Name,
				Tickets = ticketsViewModel
			};

			return View(viewModel);
		}
	}
}