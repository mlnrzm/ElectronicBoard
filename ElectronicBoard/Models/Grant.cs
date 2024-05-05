using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	public class Grant
	{
		public int Id { get; set; }
		public int BlockId { get; set; }
		public string GrantName { get; set; }
		public string GrantText { get; set; }
		public string GrantDescription { get; set; }
		public string GrantSource { get; set; }
		public DateTime GrantDeadline { get; set; }
		public string GrantStatus { get; set; }
		public DateTime GrantDeadlineStart { get; set; }
		public DateTime GrantDeadlineFinish { get; set; }
		public byte[] Picture { get; set; }
		[ForeignKey("GrantId")]
		public virtual List<Sticker> Stikers { get; set; }
		[ForeignKey("GrantId")]
		public virtual List<GrantParticipant> GrantParticipants { get; set; }
		[ForeignKey("GrantId")]
		public virtual List<File> Files { get; set; }
	}
}
