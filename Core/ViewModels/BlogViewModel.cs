using Core.Models;

namespace Core.ViewModels
{
    public class BlogViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string ImageUrl { get; set; }

        public string BlogContent { get; set; }

        public string DateCreated { get; set; }

        public bool Deleted { get; set; }

        public bool Active { get; set; }
        public string? BlogStatus { get; set; }
        public string ShortDetails { get; set; }
        public string BlogAutorProfilePicture { get; set; }
        public string AddedById { get; set; }
        public string BlogAutorName { get; set; }
        public string BlogAutorSchool { get; set; }
        public string BlogAuthorEmail { get; set; }
        public string BlogAuthorPhone { get; set; }
        public string AboutMe { get; set; }
        public List<BlogCommentViewModel> BlogComments { get; set; }

        public int? BlogCategoryId { get; set; }
        public string BlogCategoryName { get; set; }
        public List<CommonDropdown> BlogCategories { get; set; }
    }
}
