using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TicketingSystem.Data;
using TicketingSystem.Models;

namespace TicketingSystem.Web.Controllers
{
    public class CommentsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: /Comments/
		public ActionResult Index()
		{
			var comments = this.db.Comments.Include(c => c.Ticket).Include(c => c.User);
			return this.View(comments.ToList());
		}

        // GET: /Comments/Details/5
		public ActionResult Details(int? id)
		{
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			Comment comment = this.db.Comments.Find(id);
			if (comment == null)
			{
				return this.HttpNotFound();
			}
			return this.View(comment);
		}

        // GET: /Comments/Create
		public ActionResult Create()
		{
			this.ViewBag.TicketId = new SelectList(this.db.Tickets, "Id", "AuthorId");
			this.ViewBag.UserId = new SelectList(this.db.Users, "Id", "UserName");
			return this.View();
		}

        // POST: /Comments/Create
		// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// 
		// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Comment comment)
		{
			if (this.ModelState.IsValid)
			{
				this.db.Comments.Add(comment);
				this.db.SaveChanges();
				return this.RedirectToAction("Index");
			}

			this.ViewBag.TicketId = new SelectList(this.db.Tickets, "Id", "AuthorId", comment.TicketId);
			this.ViewBag.UserId = new SelectList(this.db.Users, "Id", "UserName", comment.UserId);
			return this.View(comment);
		}

        // GET: /Comments/Edit/5
		public ActionResult Edit(int? id)
		{
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			Comment comment = this.db.Comments.Find(id);
			if (comment == null)
			{
				return this.HttpNotFound();
			}
			this.ViewBag.TicketId = new SelectList(this.db.Tickets, "Id", "AuthorId", comment.TicketId);
			this.ViewBag.UserId = new SelectList(this.db.Users, "Id", "UserName", comment.UserId);
			return this.View(comment);
		}

        // POST: /Comments/Edit/5
		// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// 
		// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Comment comment)
		{
			if (this.ModelState.IsValid)
			{
				this.db.Entry(comment).State = EntityState.Modified;
				this.db.SaveChanges();
				return this.RedirectToAction("Index");
			}
			this.ViewBag.TicketId = new SelectList(this.db.Tickets, "Id", "AuthorId", comment.TicketId);
			this.ViewBag.UserId = new SelectList(this.db.Users, "Id", "UserName", comment.UserId);
			return this.View(comment);
		}

        // GET: /Comments/Delete/5
		public ActionResult Delete(int? id)
		{
            if (id == null)
            {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			Comment comment = this.db.Comments.Find(id);
			if (comment == null)
			{
				return this.HttpNotFound();
			}
			return this.View(comment);
		}

        // POST: /Comments/Delete/5
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
		{
			Comment comment = this.db.Comments.Find(id);
			this.db.Comments.Remove(comment);
			this.db.SaveChanges();
			return this.RedirectToAction("Index");
		}

        protected override void Dispose(bool disposing)
        {
			this.db.Dispose();
			base.Dispose(disposing);
        }
    }
}
