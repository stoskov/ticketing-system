using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TicketingSystem.Web.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ShouldNotContainTextAttribute : ValidationAttribute
	{
		private readonly string textToSearch = "";

		public ShouldNotContainTextAttribute(string textToSearch)
		{
			this.textToSearch = textToSearch;
		}

		public override bool IsValid(object value)
		{
			var content = value.ToString();
			if (content.ToLower().Contains(this.textToSearch.ToLower()))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public override string FormatErrorMessage(string name)
		{
			return string.Format("{0} sould not contain {1}", name, this.textToSearch);
		}
	}
}