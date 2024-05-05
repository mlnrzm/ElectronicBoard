namespace ElectronicBoard.Models
{
    public class Sticker
    {
        public int Id { get; set; }
        public string StickerDescription { get; set; }
		public byte[] Picture { get; set; }
		public int? SimpleElementId { get; set; }
		public int? EventId { get; set; }
		public int? ParticipantId { get; set; }
		public int? ProjectId { get; set; }
		public int? GrantId { get; set; }
	}
}
