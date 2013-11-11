using System.ComponentModel.DataAnnotations;
namespace TicketingSystem.Web.Models
{
	public class CommentViewModel
	{
		public int Id { get; set; }

		public string UserName { get; set; }

		public int TicketId { get; set; }

		[DataType(DataType.MultilineText)]
		public string Content { get; set; }
	}
}