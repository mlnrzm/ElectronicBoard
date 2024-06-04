namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-связь сущностей "Статья" и "Автор"
	/// </summary>
	public class ArticleAuthor
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Идентификатор статьи
		/// </summary>
		public int ArticleId { get; set; }

		/// <summary>
		/// Идентификатор автора
		/// </summary>
		public int AuthorId { get; set; }

		/// <summary>
		/// Статья
		/// </summary>
		public virtual Article Article { get; set; }

		/// <summary>
		/// Автор
		/// </summary>
		public virtual Author Author { get; set; }
	}
}
