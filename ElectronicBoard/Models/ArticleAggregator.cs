namespace ElectronicBoard.Models
{
	public class ArticleAggregator
	{
		public int Id { get; set; }
		public int ArticleId { get; set; }
		public int AggregatorId { get; set; }
		public virtual Article Article { get; set; }
		public virtual Aggregator Aggregator { get; set; }
	}
}
