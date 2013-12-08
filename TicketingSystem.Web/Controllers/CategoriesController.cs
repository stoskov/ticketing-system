using System;
using System.Linq;
using System.Web.Mvc;
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
		//// GET: /Categories/Details/5
		//public ActionResult Details(int? id)
		//{
		//	if (id == null)
		//	{
		//		return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
		//	}
		//	Category category = db.Categories.Find(id);
		//	if (category == null)
		//	{
		//		return HttpNotFound();
		//	}
		//	return View(category);
		//}
		//// GET: /Categories/Create
		//public ActionResult Create()
		//{
		//	return View();
		//}
		//// POST: /Categories/Create
		//// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		//// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		//// 
		//// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public ActionResult Create(Category category)
		//{
		//	if (ModelState.IsValid)
		//	{
		//		db.Categories.Add(category);
		//		db.SaveChanges();
		//		return RedirectToAction("Index");
		//	}
		//	return View(category);
		//}
		//// GET: /Categories/Edit/5
		//public ActionResult Edit(int? id)
		//{
		//	if (id == null)
		//	{
		//		return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
		//	}
		//	Category category = db.Categories.Find(id);
		//	if (category == null)
		//	{
		//		return HttpNotFound();
		//	}
		//	return View(category);
		//}
		//// POST: /Categories/Edit/5
		//// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		//// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		//// 
		//// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public ActionResult Edit(Category category)
		//{
		//	if (ModelState.IsValid)
		//	{
		//		db.Entry(category).State = EntityState.Modified;
		//		db.SaveChanges();
		//		return RedirectToAction("Index");
		//	}
		//	return View(category);
		//}
		//// GET: /Categories/Delete/5
		//public ActionResult Delete(int? id)
		//{
		//	if (id == null)
		//	{
		//		return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
		//	}
		//	Category category = db.Categories.Find(id);
		//	if (category == null)
		//	{
		//		return HttpNotFound();
		//	}
		//	return View(category);
		//}
		//// POST: /Categories/Delete/5
		//[HttpPost, ActionName("Delete")]
		//[ValidateAntiForgeryToken]
		//public ActionResult DeleteConfirmed(int id)
		//{
		//	Category category = db.Categories.Find(id);
		//	db.Categories.Remove(category);
		//	db.SaveChanges();
		//	return RedirectToAction("Index");
		//}
		//protected override void Dispose(bool disposing)
		//{
		//	db.Dispose();
		//	base.Dispose(disposing);
		//}
	}
}