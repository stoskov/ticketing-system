using System;

namespace TicketingSystem.Web.Helpers
{
	public class PagingOptions
	{
		public bool HasPrevButton { get; set; }

		public bool HasNextButton { get; set; }

		public bool HasFirstButton { get; set; }

		public bool HasLastButton { get; set; }

		public string PrevButtonText { get; set; }

		public string NextButtonText { get; set; }

		public string FirstButtonText { get; set; }

		public string LastButtonText { get; set; }

		public int NumberOfPages { get; set; }

		public string QueryParameterName { get; set; }
		public int LastPage { get; set; }

		public PagingOptions()
		{
			this.HasNextButton = true;
			this.HasPrevButton = true;
			this.HasFirstButton = true;
			this.HasLastButton = true;
			this.NextButtonText = ">";
			this.PrevButtonText = "<";
			this.FirstButtonText = "<<";
			this.LastButtonText = ">>";
			this.QueryParameterName = "page";
			this.NumberOfPages = 5;
			this.LastPage = 0;
			this.wrapperClasses = "";
		}

		public string wrapperClasses { get; set; }
	}
}