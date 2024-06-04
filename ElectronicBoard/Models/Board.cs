using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-модель сущности "Доска"
	/// </summary>
	public class Board
    {
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Наименование доски
		/// </summary>
        public string BoardName { get; set; }

		/// <summary>
		/// Тематика доски
		/// </summary>
		public string BoardThematics { get; set; }

		/// <summary>
		/// Блоки доски
		/// </summary>
		[ForeignKey("BoardId")]
        public virtual List<Block> Blocks { get; set; }
    }
}
