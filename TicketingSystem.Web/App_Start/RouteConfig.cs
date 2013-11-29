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
				name: "AllTickets",
				url: "Tickets",
				defaults: new { controller = "Tickets", action = "Index", id = UrlParameter.Optional });

			routes.MapRoute(
				name: "Comments",
				url: "Comments/{action}/{id}",
				defaults: new { controller = "Comments", action = "Index", id = UrlParameter.Optional });

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
		}
	}
}