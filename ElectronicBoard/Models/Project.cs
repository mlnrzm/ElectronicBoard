using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	public class Project
	{
		public int Id { get; set; }
		public int BlockId { get; set; }
		public string ProjectName { get; set; }
		public string ProjectText { get; set; }
		public string ProjectDescription { get; set; }
		public byte[] Picture { get; set; }
		[ForeignKey("ProjectId")]
		public virtual List<Sticker> Stikers { get; set; }
		[ForeignKey("ProjectId")]
		public virtual List<ProjectParticipant> ProjectParticipants { get; set; }
		[ForeignKey("ProjectId")]
		public virtual List<Stage> Stages { get; set; }
		[ForeignKey("ProjectId")]
		public virtual List<File> Files { get; set; }
	}
}
