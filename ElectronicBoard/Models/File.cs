namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-модель сущности "Файл"
	/// </summary>
	public class File
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Название файла
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// Тип файла
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// Путь
		/// </summary>
		public string? Path { get; set; }

		/// <summary>
		/// Идентификатор элемента, к которому прикреплён файл
		/// </summary>
		public int? SimpleElementId { get; set; }
		public int? EventId { get; set; }
		public int? ParticipantId { get; set; }
		public int? ProjectId { get; set; }
		public int? GrantId { get; set; }
		public int? StageId { get; set; }
		public int? ArticleId { get; set; }
	}
}
