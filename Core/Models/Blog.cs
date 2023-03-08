using Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Blog : BaseModel
    {
        [Key]
        public new Guid Id { get; set; }
        public string? Title { get; set; }
        public string? ImageUrl { get; set; }
        public string? BlogContent { get; set; }
        public BlogStatus? BlogStatus { get; set; }
        public string? AddedById { get; set; }
        [ForeignKey("AddedById")]
        public virtual ApplicationUser? AddedBy { get; set; }
        public string? AboutMe { get; set; }
        public int? BlogCategoryId { get; set; }
        [ForeignKey("BlogCategoryId")]
        public virtual CommonDropdown? BlogCategory { get; set; }
        public Guid? BlogCommentId { get; set; }
        [ForeignKey("BlogCommentId")]
        public virtual List<BlogComment>? BlogComment { get; set; }
        [NotMapped]
        public List<BlogComment>? BlogCommentCount { get; set; }
        [NotMapped]
        public string? ShortDetails
        {
            get
            {
                var details = BlogContent;
                if (details != null && details.Length > 200)
                {
                    return BlogContent.Substring(0, 200) + "....";
                }
                return details;
            }
        }
    }
}
