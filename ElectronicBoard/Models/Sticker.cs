namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-модель сущности "Стикер"
	/// </summary>
	public class Sticker
    {
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Описание
		/// </summary>
		public string StickerDescription { get; set; }

		/// <summary>
		/// Рисунок
		/// </summary>
		public byte[] Picture { get; set; }

		/// <summary>
		/// Идентификатор элемента, к которому прикреплен стикер
		/// </summary>
		public int? SimpleElementId { get; set; }
		public int? EventId { get; set; }
		public int? ParticipantId { get; set; }
		public int? ProjectId { get; set; }
		public int? GrantId { get; set; }
	}
}
