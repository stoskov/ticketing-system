using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TicketingSystem.Web.Helpers
{
	public class SelectListItemComparer : IEqualityComparer<SelectListItem>
	{
		public bool Equals(SelectListItem x, SelectListItem y)
		{
			if (Object.ReferenceEquals(x, y))
				return true;

			if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
				return false;

			return x.Value == y.Value && x.Text == y.Text;
		}

		public int GetHashCode(SelectListItem item)
		{
			if (Object.ReferenceEquals(item, null))
			{
				return 0;
			}

			return item.Value.GetHashCode() ^ item.Text.GetHashCode();
		}
	}
}