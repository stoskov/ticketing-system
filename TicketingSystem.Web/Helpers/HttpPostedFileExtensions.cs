using System;
using System.Linq;
using System.Web;

namespace TicketingSystem.Web.Helpers
{
	public static class HttpPostedFileExtensions
	{
		public static bool HasFile(this HttpPostedFileBase file)
		{
			return (file != null && file.ContentLength > 0);
		}
	}
}