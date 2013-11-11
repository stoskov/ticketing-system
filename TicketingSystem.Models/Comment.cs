using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TicketingSystem.Models
{
	public class Comment
	{
		[Key]
		public int Id { get; set; }

		public string UserId { get; set; }

		public virtual AppUser User { get; set; }

		public string Content { get; set; }

		public int TicketId { get; set; }

		public virtual Ticket Ticket { get; set; }
	}
}