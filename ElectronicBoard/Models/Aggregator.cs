using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	public class Aggregator
	{
		public int Id { get; set; }
		public string AggregatorName { get; set; }
		[ForeignKey("AggregatorId")]
		public virtual List<ArticleAggregator> AggregatorArticles { get; set; }

	}
}
