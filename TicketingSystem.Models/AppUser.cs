using System;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TicketingSystem.Models
{
	public class AppUser : User
	{
		private const int InitialPoints = 10;

		public int Points { get; set; }

		public AppUser()
		{
			this.Points = InitialPoints;
		}

		public AppUser(string userName)
		{
			this.UserName = userName;
			this.Points = InitialPoints;
		}
	}
}