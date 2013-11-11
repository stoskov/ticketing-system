namespace TicketingSystem.Data.Migrations
{
	using System;
	using System.Data.Entity.Migrations;
	using System.Linq;
	using TicketingSystem.Models;

	public sealed class AppDbConfiguration : DbMigrationsConfiguration<TicketingSystem.Data.AppDbContext>
	{
		public AppDbConfiguration()
		{
			this.AutomaticMigrationsEnabled = true;
			this.AutomaticMigrationDataLossAllowed = true;
		}

		protected override void Seed(AppDbContext context)
		{
			if (context.Tickets.Count() > 0)
			{
				return;
			}

			Random rand = new Random();

			var user = new AppUser("Customer");

			for (int i = 0; i < 15; i++)
			{
				var category = new Category()
				{
					Name = "Category No:" + i.ToString() + "<script>alert('Hack!')</script>"
				};

				var ticketsCount = rand.Next(5, 50);
				for (int j = 0; j < ticketsCount; j++)
				{
					var ticket = new Ticket()
					{
						Author = user,
						Category = category,
						Description = "Ticket description. Ticket description. Ticket description. Ticket description. Ticket description. <script>alert('Hack!')</script>",
						Title = "Ticket title No:" + j.ToString() + " - " + category.Name + "<script>alert('Hack!')</script>",
						ScreenshotUrl = "http://telerikacademy.com/Content/Images/news-img01.png"
					};

					var commentsCount = rand.Next(15, 100);
					for (int k = 0; k < commentsCount; k++)
					{
						var comment = new Comment()
						{
							User = user,
							Content = "Comment content No:" + k.ToString() + " - " + ticket.Title + "<script>alert('Hack!')</script>"
						};

						ticket.Comments.Add(comment);
					}

					context.Tickets.Add(ticket);
				}
			}

			context.SaveChanges();

			base.Seed(context);
		}
	}
}