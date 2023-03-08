using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class State : BaseModel
    {
        public int? CountryId { get; set; }
        [ForeignKey("CountryId")]
        public virtual Country? Country { get; set; }
    }
}
