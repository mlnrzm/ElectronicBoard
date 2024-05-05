using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
    /// <summary>
    /// Доска
    /// </summary>
    public class Board
    {
        public int Id { get; set; }
        [Required]
        public string BoardName { get; set; }
        [Required]
        public string BoardThematics { get; set; }
        [ForeignKey("BoardId")]
        public virtual List<Block> Blocks { get; set; }
    }
}
