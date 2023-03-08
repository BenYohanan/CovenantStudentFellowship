using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Gallery : BaseModel
    {
        [Key]
        public new Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Note { get; set; }
        public string? ImageUrl { get; set; }
    }
}
