using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-модель сущности "Этап проекта"
	/// </summary>
	public class Stage
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Идентификатор проекта, в котором находится этап
		/// </summary>
		public int ProjectId { get; set; }

		/// <summary>
		/// Название этапа
		/// </summary>
		public string StageName { get; set; }

		/// <summary>
		/// Доп. текст
		/// </summary>
		public string StageText { get; set; }

		/// <summary>
		/// Описание этапа
		/// </summary>
		public string StageDescription { get; set; }

		/// <summary>
		/// Дата начала этапа
		/// </summary>
		public DateTime? DateStart { get; set; }

		/// <summary>
		/// Дата окончания этапа
		/// </summary>
		public DateTime? DateFinish { get; set; }

		/// <summary>
		/// Статус этапа
		/// </summary>
		public string Status { get; set; }

		/// <summary>
		/// Рисунок
		/// </summary>
		public byte[] Picture { get; set; }

		/// <summary>
		/// Файлы, прикрепленные к этапу
		/// </summary>
		[ForeignKey("StageId")]
		public virtual List<File> Files { get; set; }
	}
}
