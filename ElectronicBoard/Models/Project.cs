using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-модель сущности "Проект"
	/// </summary>
	public class Project
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Идентификатор блока, в котором находится проект
		/// </summary>
		public int BlockId { get; set; }

		/// <summary>
		/// Название проекта
		/// </summary>
		public string ProjectName { get; set; }

		/// <summary>
		/// Допю текст
		/// </summary>
		public string ProjectText { get; set; }

		/// <summary>
		/// Описание проекта
		/// </summary>
		public string ProjectDescription { get; set; }

		/// <summary>
		/// Рисунок
		/// </summary>
		public byte[] Picture { get; set; }

		/// <summary>
		/// Стикеры, прикрепленные к проекту
		/// </summary>
		[ForeignKey("ProjectId")]
		public virtual List<Sticker> Stikers { get; set; }

		/// <summary>
		/// Участники проекта
		/// </summary>
		[ForeignKey("ProjectId")]
		public virtual List<ProjectParticipant> ProjectParticipants { get; set; }

		/// <summary>
		/// Этапы проекта
		/// </summary>
		[ForeignKey("ProjectId")]
		public virtual List<Stage> Stages { get; set; }

		/// <summary>
		/// Файлы, прикрепленные к проекту
		/// </summary>
		[ForeignKey("ProjectId")]
		public virtual List<File> Files { get; set; }
	}
}
