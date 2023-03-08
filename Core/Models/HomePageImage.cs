using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class HomePageImage : BaseModel
    {
        public string? ImageUrl { get; set; }
        public int? HomePageImageId { get; set; }
        [ForeignKey("HomePageImageId")]
        public virtual CommonDropdown? HomePagePicture { get; set; }
    }
}
