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

			var pager = new TagBuilder("ul");

			pager.InnerHtml += GetPageLink(helper, options.HasFirstButton, options.FirstButtonText, options.QueryParameterName, 1, selectedPage, "disabled");
			pager.InnerHtml += GetPageLink(helper, options.HasPrevButton, options.PrevButtonText, options.QueryParameterName, Math.Max(selectedPage - 1, 1), selectedPage, "disabled");

			for (int i = startingPage; i <= maxPage; i++)
			{
				pager.InnerHtml += GetPageLink(helper, true, i.ToString(), options.QueryParameterName, i, selectedPage, "active");
			}

			pager.InnerHtml += GetPageLink(helper, options.HasNextButton, options.NextButtonText, options.QueryParameterName, Math.Min(selectedPage + 1, options.LastPage), selectedPage, "disabled");
			pager.InnerHtml += GetPageLink(helper, options.HasLastButton, options.LastButtonText, options.QueryParameterName, options.LastPage, selectedPage, "disabled");

			wrapper.InnerHtml += pager.ToString();

			return MvcHtmlString.Create(wrapper.ToString());
		}

		private static int GetCurrentPage(HtmlHelper helper, string queryParameterName)
		{
			var currentPageString = helper.ViewContext.HttpContext.Request.QueryString[queryParameterName];
			var currentPage = 0;

			int.TryParse(currentPageString, out currentPage);
			if (currentPage == 0)
			{
				currentPage = 1;
			}

			return currentPage;
		}

		private static string GetPageLink(HtmlHelper helper, bool shouldRender, string buttonText,
			string queryParameterName, int page, int currentPage, string currentPageClass)
		{
			if (!shouldRender)
			{
				return string.Empty;
			}

			var pageTag = new TagBuilder("li");
			TagBuilder innterTag;

			if (page == currentPage)
			{
				innterTag = new TagBuilder("span");
				pageTag.AddCssClass(currentPageClass);
			}
			else
			{
				innterTag = new TagBuilder("a");
				innterTag.MergeAttribute("href", GetPagedUri(helper, queryParameterName, page));
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
			uri.Port = currentUri.Port;
			uri.Path = currentUri.AbsolutePath;
			uri.Query = query.ToString();

			return uri.ToString();
		}
	}
}