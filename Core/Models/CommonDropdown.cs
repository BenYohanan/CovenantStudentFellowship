using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class CommonDropdown : BaseModel
    {
        [Display(Name = "Drpdown Key")]
        public int DropdownKey { get; set; }
    }
}
