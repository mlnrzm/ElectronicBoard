using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	public class Stage
	{
		public int Id { get; set; }
		public int ProjectId { get; set; }
		public string StageName { get; set; }
		public string StageText { get; set; }
		public string StageDescription { get; set; }
		public DateTime? DateStart { get; set; }
		public DateTime? DateFinish { get; set; }
		public string Status { get; set; }
		public byte[] Picture { get; set; }
		[ForeignKey("StageId")]
		public virtual List<File> Files { get; set; }
	}
}
