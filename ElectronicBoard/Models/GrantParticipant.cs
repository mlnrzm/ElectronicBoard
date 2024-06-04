namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-связь сущности "Грант" и "Участник"
	/// </summary>
	public class GrantParticipant
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
		/// Идентификатор гранта
		/// </summary>
		public int GrantId { get; set; }

		/// <summary>
		/// Участник
		/// </summary>
		public virtual Participant Participant { get; set; }

		/// <summary>
		/// Грант
		/// </summary>
		public virtual Grant Grant { get; set; }
	}
}
