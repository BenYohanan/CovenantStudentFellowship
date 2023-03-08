using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels
{
    public class ApplicationUserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "FullName")]

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must be the same. ")]
        public string ConfirmPassword { get; set; }

        public string ConfirmEmail { get; set; }

        [Display(Name = "Remember My Password")]
        public bool RememberPassword { get; set; }

        [Display(Name = "Date Of Birth")]
        public string DateOfBirth { get; set; }

        public string DOB { get; set; }
        public string Level { get; set; }

        public int? GenderId { get; set; }
        public string Gender { get; set; }

        public int? StateId { get; set; }
        public string State { get; set; }

        [Display(Name = "Home Address")]
        public string HomeAddress { get; set; }

        public Guid? SchoolId { get; set; }
        public string School { get; set; }

        public int? ReligionId { get; set; }
        public string Religion { get; set; }

        [Display(Name = "School Address")]
        public string SchoolAddress { get; set; }

        public int? NationalityId { get; set; }
        public string Nationality { get; set; }

        [Display(Name = "Home Synagogue")]
        public string HomeSynagogue { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        public string ProfilePicture { get; set; }
        public string Layout { get; set; }
        public bool Deactivated { get; set; }
        public Guid? BlogId { get; set; }
        public string FullName { get; set; }
        public string ContactName { get; set; }
        public string ContactRelationship { get; set; }
        public string ContactPhoneNumber { get; set; }
    }
}
