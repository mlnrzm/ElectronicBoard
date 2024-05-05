namespace ElectronicBoard.Models
{
	public class BlockEvent
	{
		public int Id { get; set; }
		public int EventId { get; set; }
		public int BlockId { get; set; }
		public virtual Event Event { get; set; }
		public virtual Block Block { get; set; }
	}
}
