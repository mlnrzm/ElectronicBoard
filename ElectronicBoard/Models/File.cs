namespace ElectronicBoard.Models
{
	public class File
	{
		public int Id { get; set; }
		public string FileName { get; set; }
		public string ContentType { get; set; }
		public string? Path { get; set; }
		public int? SimpleElementId { get; set; }
		public int? EventId { get; set; }
		public int? ParticipantId { get; set; }
		public int? ProjectId { get; set; }
		public int? GrantId { get; set; }
		public int? StageId { get; set; }
		public int? ArticleId { get; set; }
	}
}
