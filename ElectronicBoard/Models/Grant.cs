using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-модель сущности "Грант"
	/// </summary>
	public class Grant
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Идентификатор блока
		/// </summary>
		public int BlockId { get; set; }

		/// <summary>
		/// Название гранта
		/// </summary>
		public string GrantName { get; set; }

		/// <summary>
		/// Доп. текст гранта
		/// </summary>
		public string GrantText { get; set; }

		/// <summary>
		/// Описание гранта
		/// </summary>
		public string GrantDescription { get; set; }

		/// <summary>
		/// Источник гранта
		/// </summary>
		public string GrantSource { get; set; }

		/// <summary>
		/// Срок подачи заявки
		/// </summary>
		public DateTime GrantDeadline { get; set; }

		/// <summary>
		/// Статус гранта
		/// </summary>
		public string GrantStatus { get; set; }

		/// <summary>
		/// Срок начала выполнения
		/// </summary>
		public DateTime GrantDeadlineStart { get; set; }

		/// <summary>
		/// Срок окончания выполнения
		/// </summary>
		public DateTime GrantDeadlineFinish { get; set; }

		/// <summary>
		/// Рисунок
		/// </summary>
		public byte[] Picture { get; set; }

		/// <summary>
		/// Стикеры, прикрепленные к гранту
		/// </summary>
		[ForeignKey("GrantId")]
		public virtual List<Sticker> Stikers { get; set; }

		/// <summary>
		/// Участники гранта
		/// </summary>
		[ForeignKey("GrantId")]
		public virtual List<GrantParticipant> GrantParticipants { get; set; }

		/// <summary>
		/// Файлы, прикрепленные к гранту
		/// </summary>
		[ForeignKey("GrantId")]
		public virtual List<File> Files { get; set; }
	}
}
