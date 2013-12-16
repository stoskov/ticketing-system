using System;
using System.Linq;

namespace TicketingSystem.Web.Models.Files
{
	public class AttachmentViewModel
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }
	}
}