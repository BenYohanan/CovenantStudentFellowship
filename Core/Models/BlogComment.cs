using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class BlogComment : BaseModel
    {
        [Key]
        public new Guid Id { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Comment { get; set; }
        public Guid? BlogId { get; set; }
        [ForeignKey("BlogId")]
        public virtual Blog? Blog { get; set; }
        public int? BlogCategoryId { get; set; }
        [ForeignKey("BlogCategoryId")]
        public virtual CommonDropdown? BlogCategory { get; set; }
        public string? ImageUrl { get; set; }

    }
}
