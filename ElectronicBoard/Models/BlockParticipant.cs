namespace ElectronicBoard.Models
{
	/// <summary>
	/// Класс-связь сущности "Блок" и "Участник"
	/// </summary>
	public class BlockParticipant
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
		/// Идентификатор блока
		/// </summary>
		public int BlockId { get; set; }

		/// <summary>
		/// Участник
		/// </summary>
		public virtual Participant Participant { get; set; }

		/// <summary>
		/// Блок
		/// </summary>
		public virtual Block Block { get; set; }
    }
}
