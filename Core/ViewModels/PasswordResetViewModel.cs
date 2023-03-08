using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class PasswordResetViewModel
    {
        [Key]
        public Guid Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }


        [DataType(DataType.Password)]
        [Required]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must be the same. ")]
        public string ConfirmNewPassword { get; set; }
    }
}
