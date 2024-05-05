namespace ElectronicBoard.Models
{
	public class ProjectParticipant
	{
		public int Id { get; set; }
		public int ParticipantId { get; set; }
		public int ProjectId { get; set; }
		public virtual Participant Participant { get; set; }
		public virtual Project Project { get; set; }
	}
}
