using System;
using System.Collections.Generic;
using System.Linq;
using TicketingSystem.Web.Models.Base;
using TicketingSystem.Web.Models.Comments;

namespace TicketingSystem.Web.Models.Tickets
{
	public class TicketDetailsViewModel : TicketViewModel
	{
		public CommentPagebleViewModel CommentsContainer { get; set; }

		public TicketsMetaDataViewModel MetaData { get; set; }
	}
}