using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.Web.Models.Accounts
{
	public class RegisterViewModel
	{
		[Required]
		[Display(Name = "User name")]
		[StringLength(16, ErrorMessage = "The {0} must be at least {2} characters long, but not longer than {1}.", MinimumLength = 6)]
		public string UserName { get; set; }

		[Required]
		[Display(Name = "Email")]
		[RegularExpression(@"[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,4}", ErrorMessage = "Not valid email address.")]
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}
}