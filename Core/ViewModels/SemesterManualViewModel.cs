using System.ComponentModel.DataAnnotations.Schema;

namespace Core.ViewModels
{
    public class SemesterManualViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public Guid? SchoolId { get; set; }
        [ForeignKey("SchoolId")]
        public string SchoolName { get; set; }
        public string Date { get; set; }
        public string Day { get; set; }
        public string? BibleVerse { get; set; }
        public string? SpeakerId { get; set; }
        public string? SpeakerName { get; set; }
        public string? SpeakerProfilePicture { get; set; }
        public string? CordinatorId { get; set; }
        public string CordinatorName { get; set; }
        public string CordinatorProfilePicture { get; set; }
    }
}
