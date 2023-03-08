using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class School
    {
        [Key]
        public Guid Id { get; set; }
        public int? StateId { get; set; }
        [ForeignKey("StateId")]
        public virtual State? State { get; set; }
        public string? Address { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string? SchoolName { get; set; }
        public string? SchoolCodeName { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
