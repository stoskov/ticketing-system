using System;
using System.Web;
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

		public static MvcHtmlString Paging(this HtmlHelper helper)
		{
			return Paging(helper, new PagingOptions());
		}

		public static MvcHtmlString Paging(this HtmlHelper helper, PagingOptions options)
		{
			var wrapper = new TagBuilder("div");
			wrapper.AddCssClass("pagination");
			wrapper.AddCssClass(options.wrapperClasses);

			var selectedPage = GetCurrentPage(helper, options.QueryParameterName);

			var startingPage = (selectedPage - options.NumberOfPages / 2);
			startingPage = Math.Min(startingPage, options.LastPage - options.NumberOfPages + 1);
			startingPage = Math.Max(startingPage, 1);

			var maxPage = Math.Min(startingPage + options.NumberOfPages - 1, options.LastPage);
			var nextPage = Math.Min(selectedPage + 1, options.LastPage);
			var prevPage = Math.Max(selectedPage - 1, 1);

			var pager = new TagBuilder("ul");

			pager.InnerHtml += GetPageLinkIfApplicable(options.HasFirstButton, options.FirstButtonText,
				GetPagedUri(helper, options.QueryParameterName, 1), 1 == selectedPage, "disabled");
			pager.InnerHtml += GetPageLinkIfApplicable(options.HasPrevButton, options.PrevButtonText,
				GetPagedUri(helper, options.QueryParameterName, prevPage), prevPage == selectedPage, "disabled");

			for (int i = startingPage; i <= maxPage; i++)
			{
				pager.InnerHtml += GetPageLinkIfApplicable(true, i.ToString(),
					GetPagedUri(helper, options.QueryParameterName, i), i == selectedPage, "active");
			}

			pager.InnerHtml += GetPageLinkIfApplicable(options.HasNextButton, options.NextButtonText,
				GetPagedUri(helper, options.QueryParameterName, nextPage), nextPage == selectedPage, "disabled");
			pager.InnerHtml += GetPageLinkIfApplicable(options.HasLastButton, options.LastButtonText,
				GetPagedUri(helper, options.QueryParameterName, options.LastPage), options.LastPage == selectedPage, "disabled");

			wrapper.InnerHtml += pager.ToString();

			return MvcHtmlString.Create(wrapper.ToString());
		}

		private static int GetCurrentPage(HtmlHelper helper, string queryParameterName)
		{
			var currentPageString = helper.ViewContext.HttpContext.Request.QueryString[queryParameterName];
			var currentPage = 0;

			int.TryParse(currentPageString, out currentPage);
			currentPage = Math.Max(currentPage, 1);

			return currentPage;
		}

		private static string GetPageLinkIfApplicable(bool isApplicable, string buttonText, string url, bool isCurrentPage, string currentPageClass)
		{
			if (!isApplicable)
			{
				return string.Empty;
			}

			return GetPageLink(buttonText, url, isCurrentPage, currentPageClass);
		}

		private static string GetPageLink(string buttonText, string url, bool isCurrentPage, string currentPageClass)
		{
			var pageTag = new TagBuilder("li");
			TagBuilder innterTag;

			if (isCurrentPage)
			{
				innterTag = new TagBuilder("span");
				pageTag.AddCssClass(currentPageClass);
			}
			else
			{
				innterTag = new TagBuilder("a");
				innterTag.MergeAttribute("href", url);
			}

			innterTag.SetInnerText(buttonText);
			pageTag.InnerHtml = innterTag.ToString();

			return pageTag.ToString();
		}

		private static string GetPagedUri(HtmlHelper helper, string queryParameterName, int page)
		{
			var currentUri = helper.ViewContext.HttpContext.Request.Url;

			var query = HttpUtility.ParseQueryString(currentUri.Query);
			query[queryParameterName] = page.ToString();

			var uri = new UriBuilder();
			uri.Host = currentUri.Host;
			uri.Path = currentUri.AbsolutePath;
			uri.Query = query.ToString();

			return uri.ToString();
		}
	}
}