namespace ElectronicBoard.Models
{
	public class ArticleAuthor
	{
		public int Id { get; set; }
		public int ArticleId { get; set; }
		public int AuthorId { get; set; }
		public virtual Article Article { get; set; }
		public virtual Author Author { get; set; }
	}
}
