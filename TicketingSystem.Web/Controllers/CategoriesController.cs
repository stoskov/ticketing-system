using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TicketingSystem.Models;
using TicketingSystem.Web.Models.Base;
using TicketingSystem.Web.Models.Categories;

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

		public ActionResult Details(int? id)
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

			var viewModel = new CategoryViewModel
			{
				Id = category.Id,
				Name = category.Name
			};

			return View(viewModel);
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
			if (!ModelState.IsValid)
			{
				return View(categoryViewModel);
			}

			var category = new Category
			{
				Name = categoryViewModel.Name
			};

			this.Data.Categories.Add(category);
			this.Data.SaveChanges();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult Edit(int? id)
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

			var viewModel = new CategoryViewModel
			{
				Id = category.Id,
				Name = category.Name
			};

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int? id, CategoryViewModel categoryViewModel)
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

			if (!ModelState.IsValid)
			{
				return View(categoryViewModel);
			}

			category.Name = categoryViewModel.Name;

			this.Data.Categories.Update(category);
			this.Data.SaveChanges();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult Delete(int? id)
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

			var viewModel = new CategoryViewModel
			{
				Id = category.Id,
				Name = category.Name
			};

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int? id, CategoryViewModel categoryViewModel)
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

			this.Data.Categories.Delete(category);
			this.Data.SaveChanges();

			return RedirectToAction("Index");
		}
	}
}