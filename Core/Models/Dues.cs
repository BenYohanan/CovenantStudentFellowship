using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Dues : BaseModel
    {
        [Key]
        public new Guid Id { get; set; }
        public double? Amount { get; set; }
    }
}
