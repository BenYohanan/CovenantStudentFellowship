namespace Core.ViewModels
{
    public class BlogCommentViewModel
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Comment { get; set; }
        public DateTime DateAdded { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public Guid BlogId { get; set; }
        public string BlogName { get; set; }
        public int? BlogCategoryId { get; set; }
        public string BlogCategoryName { get; set; }
        public string ImageUrl { get; set; }
    }
}
