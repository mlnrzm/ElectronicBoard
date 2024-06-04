using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-модель сущности "Агрегатор"
	/// </summary>
	public class Aggregator
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Наименование агрегатора
		/// </summary>
		public string AggregatorName { get; set; }

		/// <summary>
		/// Статьи, связанные с агрегатором
		/// </summary>
		[ForeignKey("AggregatorId")]
		public virtual List<ArticleAggregator> AggregatorArticles { get; set; }

	}
}
