namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-связь сущности "Проект" и "Участник"
	/// </summary>
	public class ProjectParticipant
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Идентификатор участника
		/// </summary>
		public int ParticipantId { get; set; }

		/// <summary>
		/// Идентификатор проекта
		/// </summary>
		public int ProjectId { get; set; }

		/// <summary>
		/// Участник
		/// </summary>
		public virtual Participant Participant { get; set; }

		/// <summary>
		/// Проект
		/// </summary>
		public virtual Project Project { get; set; }
	}
}
