using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.Web.Models.Accounts
{
	public class ExternalLoginConfirmationViewModel
	{
		[Required]
		[Display(Name = "User name")]
		public string UserName { get; set; }
	}
}