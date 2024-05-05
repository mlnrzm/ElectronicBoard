using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	public class Event
	{
		public int Id { get; set; }
		public string EventName { get; set; }
		public string EventText { get; set; }
		public string EventPlace { get; set; }
		public DateTime EventDateStart { get; set; }
		public string EventStartColor { get; set; }
		public DateTime EventDateFinish { get; set; }
		public string EventFinishColor { get; set; }
		public DateTime EventDateFinishArticle { get; set; }
		public string EventFinishArticleColor { get; set; }

		[ForeignKey("EventId")]
		public virtual List<Sticker> Stikers { get; set; }
		public byte[] Picture { get; set; }
		[ForeignKey("EventId")]
		public virtual List<File> Files { get; set; }
	}
}
