using System;
using System.Linq;
using TicketingSystem.Models;

namespace TicketingSystem.Web.Helpers
{
	public static class Filter
	{
		public static int CalcualteFilterTicketRelevance(Ticket ticket, string queryString)
		{
			if (string.IsNullOrEmpty(queryString))
			{
				return 1;
			}

			var relevance = 0;
			var words = queryString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			var searchTarget = string.Format("{0}{1}{2}{3}", ticket.Title, ticket.Author.UserName, ticket.Status.ToString(), ticket.Priority.ToString()).ToLower();

			foreach (var word in words)
			{
				if (searchTarget.IndexOf(word.ToLower()) >= 0)
				{
					relevance++;
				}
			}

			return relevance;
		}

		public static int CalcualteFilterCategoryRelevance(Category category, string queryString)
		{
			if (string.IsNullOrEmpty(queryString))
			{
				return 1;
			}

			var relevance = 0;
			var words = queryString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			var searchTarget = category.Name.ToLower();

			foreach (var word in words)
			{
				if (searchTarget.IndexOf(word.ToLower()) >= 0)
				{
					relevance++;
				}
			}

			return relevance;
		}
	}
}