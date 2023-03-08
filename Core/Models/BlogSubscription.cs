using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class BlogSubscription : BaseModel
    {
        [Key]
        public new Guid Id { get; set; }
        public string? Email { get; set; }
    }

}
