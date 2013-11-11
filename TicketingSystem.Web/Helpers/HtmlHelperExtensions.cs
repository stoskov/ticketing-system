using System;
using System.Web.Mvc;

namespace TicketingSystem.Web.Helpers
{
	public static class HtmlHelperExtensions
	{
		public static string TrimText(this HtmlHelper helper, string text, int maxCharLength)
		{
			if (text.Length > maxCharLength)
			{
				return text.Substring(0, maxCharLength) + " ...";
			}

			return text;
		}
	}
}