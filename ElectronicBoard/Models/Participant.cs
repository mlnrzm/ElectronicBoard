using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicBoard.Models
{
    public class Participant
	{
        public int Id { get; set; }
        public string ParticipantFIO { get; set; }
        public string ScientificInterests { get; set; }
        public string ParticipantTasks { get; set; }
        public int ParticipantRating { get; set; }
        public string ParticipantPublications { get; set; }
        public string ParticipantPatents { get; set; }

		public int IdentityId { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }

		[ForeignKey("ParticipantId")]
        public virtual List<Sticker> Stikers { get; set; }
        [ForeignKey("ParticipantId")]
        public virtual List<BlockParticipant> ParticipantsBlocks { get; set; }
		public byte[] Picture { get; set; }
		[ForeignKey("ParticipantId")]
		public virtual List<File> Files { get; set; }
	}
}
