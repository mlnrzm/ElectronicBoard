using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-модель сущности "Блок"
	/// </summary>
	public class Block
    {
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Идентификатор доски, на которой находится блок
		/// </summary>
		public int BoardId { get; set; }

		/// <summary>
		/// Наименование блока
		/// </summary>
		public string BlockName { get; set; }

		/// <summary>
		/// Видимость при открытий
		/// </summary>
		public bool VisibilityOpening { get; set; }

		/// <summary>
		/// Доска
		/// </summary>
		public virtual Board Board { get; set; }

		/// <summary>
		/// Простые элементы блока
		/// </summary>
		[ForeignKey("BlockId")]
        public virtual List<SimpleElement> BlockSimpleElements { get; set; }

		/// <summary>
		/// Участники блока
		/// </summary>
		[ForeignKey("BlockId")]
		public virtual List<BlockParticipant> BlockParticipants { get; set; }

		/// <summary>
		/// Мероприятия блока
		/// </summary>
		[ForeignKey("BlockId")]
		public virtual List<BlockEvent> BlockEvents { get; set; }

		/// <summary>
		/// Проекты блока
		/// </summary>
		[ForeignKey("BlockId")]
		public virtual List<Project> BlockProjects { get; set; }

		/// <summary>
		/// Гранты блока
		/// </summary>
		[ForeignKey("BlockId")]
		public virtual List<Grant> BlockGrants { get; set; }
	}
}
