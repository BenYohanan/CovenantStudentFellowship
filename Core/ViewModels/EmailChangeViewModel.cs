using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class EmailChangeViewModel
    {
        [Required]
        public string NewEmail { get; set; }

        [Required]
        public string ConfirmNewEmail { get; set; }
    }
}
