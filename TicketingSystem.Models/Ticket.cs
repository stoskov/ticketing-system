using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TicketingSystem.Models
{
	public class Ticket
	{
		private ICollection<Comment> comments;

		private ICollection<Attachment> attachments;

		[Key]
		public int Id { get; set; }

		public string AuthorId { get; set; }

		public virtual AppUser Author { get; set; }

		public string Title { get; set; }

		public TicketPriority Priority { get; set; }

		public TicketStatus Status { get; set; }

		public string Description { get; set; }

		public int CategoryId { get; set; }

		public virtual Category Category { get; set; }

		public virtual ICollection<Attachment> Attatchments
		{
			get
			{
				return this.attachments;
			}
			set
			{
				this.attachments = value;
			}
		}

		public virtual ICollection<Comment> Comments
		{
			get
			{
				return this.comments;
			}
			set
			{
				this.comments = value;
			}
		}

		public Ticket()
		{
			this.Priority = TicketPriority.Medium;
			this.Status = TicketStatus.New;
			this.attachments = new HashSet<Attachment>();
			this.comments = new HashSet<Comment>();
		}
	}
}