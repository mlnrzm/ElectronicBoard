using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	public class Author
	{
		public int Id { get; set; }
		public int? ParticipantId { get; set; }
		public string AuthorFIO { get; set; }
		public string AuthorEmail { get; set; }
		public string AuthorOrganization { get; set; }
		public virtual Participant? Participant { get; set; }
		[ForeignKey("AuthorId")]
		public virtual List<ArticleAuthor> AuthorArticles { get; set; }

	}
}
