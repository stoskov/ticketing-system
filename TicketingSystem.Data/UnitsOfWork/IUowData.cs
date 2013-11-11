using System;
using System.Linq;
using TicketingSystem.Data.Repositories;
using TicketingSystem.Models;

namespace TicketingSystem.Data.UnitsOfWork
{
	public interface IUowData
	{
		IRepository<Category> Categories { get; }

		IRepository<Ticket> Tickets { get; }

		IRepository<Comment> Comments { get; }

		IRepository<AppUser> Users { get; }

		int SaveChanges();
	}
}