using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace TicketingSystem.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "CompleteRegistration",
				url: "Account/CompleteRegistration/{username}/{token}",
				defaults: new { controller = "Account", action = "CompleteRegistration" });

			routes.MapRoute(
				name: "DownloadAttachment",
				url: "Tickets/DownloadAttachment/{id}/{attachmentId}",
				defaults: new { controller = "Tickets", action = "DownloadAttachment" });

			routes.MapRoute(
				name: "RemoveAttachment",
				url: "Tickets/RemoveAttachment/{id}/{attachmentId}",
				defaults: new { controller = "Tickets", action = "RemoveAttachment" });

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
		}
	}
}