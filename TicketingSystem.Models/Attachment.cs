using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TicketingSystem.Models
{
	public class Attachment
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Path { get; set; }

		public int TicketId { get; set; }

		public virtual Ticket Ticket { get; set; }
	}
}