using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TicketingSystem.Models
{
	public class Category
	{
		private ICollection<Ticket> tickets;

		[Key]
		public int Id { get; set; }

		public String Name { get; set; }

		public virtual ICollection<Ticket> Tickets
		{
			get
			{
				return this.tickets;
			}
			set
			{
				this.tickets = value;
			}
		}

		public Category()
		{
			this.tickets = new HashSet<Ticket>();
		}
	}
}