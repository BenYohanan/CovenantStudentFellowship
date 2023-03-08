using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Core.ViewModels
{
    public class SchoolViewModel
    {
        public Guid Id { get; set; }
        public string State { get; set; }
        public int? StateId { get; set; }
        public string Address { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string SchoolName { get; set; }
        public string SchoolCodeName { get; set; }
        public DateTime DateAdded { get; set; }
        public string AdminName { get; set; }
        public string ProfilePicture { get; set; }
        public string PhoneNumber { get; set; }
        public string AdminDepartment { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        public string Email { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string SchoolAdinPhoneNumber { get; set; }

        [Display(Name = "Date Of Birth")]
        public string DateOfBirth { get; set; }
        public int? GenderId { get; set; }
    }
}
