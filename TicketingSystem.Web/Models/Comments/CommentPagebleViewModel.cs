using System;
using System.Collections.Generic;
using TicketingSystem.Web.Models.Base;

namespace TicketingSystem.Web.Models.Comments
{
	public class CommentPagebleViewModel : PagebleViewModel
	{
		public IEnumerable<CommentDetailsViewModel> Comments { get; set; }
	}
}