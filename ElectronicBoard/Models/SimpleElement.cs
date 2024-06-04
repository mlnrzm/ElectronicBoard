using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-модель сущности "Простой элемент"
	/// </summary>
	public class SimpleElement
    {
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Идентификатор блока, в котором находится элемент
		/// </summary>
		public int BlockId { get; set; }

		/// <summary>
		/// Название простого элемента
		/// </summary>
		public string SimpleElementName { get; set; }

		/// <summary>
		/// Текстовое содержание элемента
		/// </summary>
		public string SimpleElementText { get; set; }

		/// <summary>
		/// Рисунок
		/// </summary>
		public byte[] Picture { get; set; }

		/// <summary>
		/// Стикеры, прикрепленные к элементу
		/// </summary>
		[ForeignKey("SimpleElementId")]
        public virtual List<Sticker> Stikers { get; set; }

		/// <summary>
		/// Файлы, прикрепленные к элементу
		/// </summary>
		[ForeignKey("SimpleElementId")]
		public virtual List<File> Files { get; set; }
	}
}
