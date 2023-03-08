using Core.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }
        public int? GenderId { get; set; }
        [Display(Name = "Gender")]
        [ForeignKey("GenderId")]
        public virtual CommonDropdown? Gender { get; set; }
        public int? StateId { get; set; }
        [ForeignKey("StateId")]
        [Display(Name = "State Of Origin")]
        public virtual State? State { get; set; }
        public string? HomeAddress { get; set; }
        public string? SchoolAddress { get; set; }
        public Guid? SchoolId { get; set; }
        [ForeignKey("SchoolId")]
        public virtual School? School { get; set; }
        public int? ReligionId { get; set; }
        [ForeignKey("ReligionId")]
        public virtual CommonDropdown? Religion { get; set; }
        public int? NationalityId { get; set; }
        [ForeignKey("NationalityId")]
        public virtual Country? Nationality { get; set; }
        public string? HomeSynagogue { get; set; }
        public string? Department { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime DateRegistered { get; set; }
        public bool Deactivated { get; set; }
        public bool IsSchoolAdmin { get; set; }
        public Guid? BlogId { get; set; }
        [ForeignKey("BlogId")]
        public virtual Blog? MyBlog { get; set; }
        public VerificationStatus? VerificationStatus { get; set; }
        public string? ContactName { get; set; }
        public string? ContactRelationship { get; set; }
        public string? ContactPhoneNumber { get; set; }
        [NotMapped]
        public string FullName => FirstName + " " + MiddleName + " " + LastName;
        [NotMapped]
        public List<string> Roles { get; set; }
        [NotMapped]
        public string UserRole { get; set; }
    }
}
