using System;
using System.Linq;

namespace TicketingSystem.Web.Models.Attachments
{
	public class AttachmentViewModel
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }

		public int TicketId { get; set; }
	}
}