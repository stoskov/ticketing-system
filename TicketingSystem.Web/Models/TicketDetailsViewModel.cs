﻿using System;
using System.Collections.Generic;
using System.Linq;
using TicketingSystem.Models;

namespace TicketingSystem.Web.Models
{
	public class TicketDetailsViewModel
	{
		public int Id { get; set; }

		public string AuthorName { get; set; }

		public string Title { get; set; }

		public string ScreenshotUrl { get; set; }

		public string Description { get; set; }

		public string CategoryName { get; set; }

		public TicketPriority Priority { get; set; }

		public ICollection<CommentViewModel> Comments { get; set; }
	}
}