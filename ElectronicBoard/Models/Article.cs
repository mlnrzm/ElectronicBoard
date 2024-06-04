using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-модель сущности "Статья"
	/// </summary>
	public class Article
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Идентификатор мероприятия, в которое загружена статья
		/// </summary>
		public int EventId { get; set; }

		/// <summary>
		/// Название статьи
		/// </summary>
		public string ArticleName { get; set; }

		/// <summary>
		/// Тестовое поле для доп. информации
		/// </summary>
		public string ArticleText { get; set; }

		/// <summary>
		/// Аннотация
		/// </summary>
		public string ArticleAnnotation { get; set; }

		/// <summary>
		/// Место публикации
		/// </summary>
		public string ArticlePlaceOfPublication { get; set; }

		/// <summary>
		/// Ключевые слова статьи
		/// </summary>
		public string ArticleKeyWords { get; set; }

		/// <summary>
		/// Статус статьи
		/// </summary>
		public StatusArticle ArticleStatus { get; set; }

		/// <summary>
		/// Рисунок
		/// </summary>
		public byte[] Picture { get; set; }

		/// <summary>
		/// Файлы, прикрепленные к статье
		/// </summary>
		[ForeignKey("ArticleId")]
		public virtual List<File> Files { get; set; }

		/// <summary>
		/// Авторы статьи
		/// </summary>
		[ForeignKey("ArticleId")]
		public virtual List<ArticleAuthor> ArticleAuthors { get; set; }

		/// <summary>
		/// Агрегаторы статьи
		/// </summary>
		[ForeignKey("ArticleId")]
		public virtual List<ArticleAggregator> ArticleAggregators { get; set; }
	}
}
