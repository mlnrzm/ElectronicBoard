using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
    public class SimpleElement
    {
        public int Id { get; set; }
        public int BlockId { get; set; }
        public string SimpleElementName { get; set; }
        public string SimpleElementText { get; set; }
        [ForeignKey("SimpleElementId")]
        public virtual List<Sticker> Stikers { get; set; }
        public byte[] Picture { get; set; }
		[ForeignKey("SimpleElementId")]
		public virtual List<File> Files { get; set; }
	}
}
