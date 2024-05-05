using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ElectronicBoard.Models
{
    public class BlockParticipant
    {
        public int Id { get; set; }
        public int ParticipantId { get; set; }
        public int BlockId { get; set; }
        public virtual Participant Participant { get; set; }
        public virtual Block Block { get; set; }
    }
}
