using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels
{
    public class HomePageImageViewModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string DateAdded { get; set; }
        public bool Deleted { get; set; }
        public bool Active { get; set; }
        public int? HomePageImageId { get; set; }
        public string PictureName { get; set; }
        public string SchoolName { get; set; }
    }
}
