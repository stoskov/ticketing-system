using System;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TicketingSystem.Models
{
	public class AppUser : User
	{
		private const int InitialPoints = 10;

		public int Points { get; set; }

		public string Email { get; set; }

		public bool IsConfirmed { get; set; }

		public string ConfirmationToken { get; set; }

		public AppUser()
		{
			this.Points = InitialPoints;
			this.IsConfirmed = false;
		}

		public AppUser(string userName)
			: this()
		{
			this.UserName = userName;
		}
	}
}