using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using TicketingSystem.Models;

namespace TicketingSystem.Data
{
	public class AppDbContext : IdentityDbContext<AppUser, UserClaim, UserSecret, UserLogin, Role, UserRole, Token, UserManagement>
	{
		public IDbSet<Category> Categories { get; set; }

		public IDbSet<Ticket> Tickets { get; set; }

		public IDbSet<Comment> Comments { get; set; }

		public IDbSet<Attachment> Attachments { get; set; }

		public AppDbContext()
			: base("TicketingSystem")
		{
		}
	}
}