using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-модель сущности "Автор"
	/// </summary>
	public class Author
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Идентификатор участника
		/// </summary>
		public int? ParticipantId { get; set; }

		/// <summary>
		/// ФИО автора
		/// </summary>
		public string AuthorFIO { get; set; }

		/// <summary>
		/// Email автора
		/// </summary>
		public string AuthorEmail { get; set; }

		/// <summary>
		/// Организация автора
		/// </summary>
		public string AuthorOrganization { get; set; }

		/// <summary>
		/// Участник
		/// </summary>
		public virtual Participant? Participant { get; set; }

		/// <summary>
		/// Статьи автора
		/// </summary>
		[ForeignKey("AuthorId")]
		public virtual List<ArticleAuthor> AuthorArticles { get; set; }

	}
}
