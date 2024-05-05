using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	public class Article
	{
		public int Id { get; set; }
		public int EventId { get; set; }
		public string ArticleName { get; set; }
		public string ArticleText { get; set; }
		public string ArticleAnnotation { get; set; }
		public string ArticlePlaceOfPublication { get; set; }
		public string ArticleKeyWords { get; set; }
		public StatusArticle ArticleStatus { get; set; }
		public byte[] Picture { get; set; }
		[ForeignKey("ArticleId")]
		public virtual List<File> Files { get; set; }
		[ForeignKey("ArticleId")]
		public virtual List<ArticleAuthor> ArticleAuthors { get; set; }
		[ForeignKey("ArticleId")]
		public virtual List<ArticleAggregator> ArticleAggregators { get; set; }
	}
}
