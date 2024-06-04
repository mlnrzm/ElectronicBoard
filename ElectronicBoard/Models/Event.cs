using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-модель сущности "Мероприятие"
	/// </summary>
	public class Event
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Название мероприятия
		/// </summary>
		public string EventName { get; set; }

		/// <summary>
		/// Доп. текст
		/// </summary>
		public string EventText { get; set; }

		/// <summary>
		/// Место проведения мероприятия
		/// </summary>
		public string EventPlace { get; set; }

		/// <summary>
		/// Дата начала проведения
		/// </summary>
		public DateTime EventDateStart { get; set; }

		/// <summary>
		/// Цвет, которым подсвечивается мероприятие при приближении начала проведения
		/// </summary>
		public string EventStartColor { get; set; }

		/// <summary>
		/// Дата окончания проведения мероприятия
		/// </summary>
		public DateTime EventDateFinish { get; set; }

		/// <summary>
		/// Цвет, которым подсвечивается мероприятие при приближении окончания проведения
		/// </summary>
		public string EventFinishColor { get; set; }

		/// <summary>
		/// Дата окончания приёма статей
		/// </summary>
		public DateTime EventDateFinishArticle { get; set; }

		/// <summary>
		/// Цвет, которым подсвечивается мероприятие при приближении окончания приёма статей
		/// </summary>
		public string EventFinishArticleColor { get; set; }

		/// <summary>
		/// Рисунок
		/// </summary>
		public byte[] Picture { get; set; }

		/// <summary>
		/// Стикеры, прикрепленные к мероприятию
		/// </summary>
		[ForeignKey("EventId")]
		public virtual List<Sticker> Stikers { get; set; }

		/// <summary>
		/// Файлы, прикрепленные к мероприятию
		/// </summary>
		[ForeignKey("EventId")]
		public virtual List<File> Files { get; set; }
	}
}
