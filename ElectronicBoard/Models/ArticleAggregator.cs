namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-связь сущностей "Статья" и "Агрегатор"
	/// </summary>
	public class ArticleAggregator
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
		/// Идентификатор агрегатора
		/// </summary>
		public int AggregatorId { get; set; }

		/// <summary>
		/// Статья
		/// </summary>
		public virtual Article Article { get; set; }

		/// <summary>
		/// Агрегатор
		/// </summary>
		public virtual Aggregator Aggregator { get; set; }
	}
}
