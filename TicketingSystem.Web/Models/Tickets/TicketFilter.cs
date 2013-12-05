﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using TicketingSystem.Models;

namespace TicketingSystem.Web.Models.Tickets
{
	public class TicketFilter
	{
		[AllowHtml]
		public string Title { get; set; }

		public int? CategoryId { get; set; }

		public string AuthorId { get; set; }

		public string Q { get; set; }

		public TicketPriority? Priority { get; set; }

		public TicketStatus? Status { get; set; }
	}
}