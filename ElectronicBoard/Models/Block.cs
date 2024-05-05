using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
    public class Block
    {
        public int Id { get; set; }
        public int BoardId { get; set; }
        public string BlockName { get; set; }
        public bool VisibilityOpening { get; set; }
		public virtual Board Board { get; set; }
		[ForeignKey("BlockId")]
        public virtual List<SimpleElement> BlockSimpleElements { get; set; }
		[ForeignKey("BlockId")]
		public virtual List<BlockParticipant> BlockParticipants { get; set; }
		[ForeignKey("BlockId")]
		public virtual List<BlockEvent> BlockEvents { get; set; }
		[ForeignKey("BlockId")]
		public virtual List<Project> BlockProjects { get; set; }
		[ForeignKey("BlockId")]
		public virtual List<Grant> BlockGrants { get; set; }
	}
}
