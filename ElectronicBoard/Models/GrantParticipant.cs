namespace ElectronicBoard.Models
{
	public class GrantParticipant
	{
		public int Id { get; set; }
		public int ParticipantId { get; set; }
		public int GrantId { get; set; }
		public virtual Participant Participant { get; set; }
		public virtual Grant Grant { get; set; }
	}
}
