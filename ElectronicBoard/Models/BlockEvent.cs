namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-связь сущности "Блок" и "Мероприятие"
	/// </summary>
	public class BlockEvent
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Идентификатор мероприятия
		/// </summary>
		public int EventId { get; set; }

		/// <summary>
		/// Идентификатор блока
		/// </summary>
		public int BlockId { get; set; }

		/// <summary>
		/// Мероприятие
		/// </summary>
		public virtual Event Event { get; set; }

		/// <summary>
		/// Блок
		/// </summary>
		public virtual Block Block { get; set; }
	}
}
