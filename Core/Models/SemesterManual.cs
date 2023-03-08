using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class SemesterManual : BaseModel
    {
        public DateTime Date { get; set; }
        public string? BibleVerse { get; set; }
        public string? SpeakerId { get; set; }
        [ForeignKey("SpeakerId")]
        public virtual ApplicationUser? Speaker { get; set; }
        public string? CordinatorId { get; set; }
        [ForeignKey("CordinatorId")]
        public virtual ApplicationUser? Cordinator { get; set; }
    }
}
