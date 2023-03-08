using Core.Models;
using Core.ViewModels;

namespace Logic.IHelpers
{
    public interface IBlogHelper
    {
        string CreateBlog(BlogViewModel createBlog, string imageUrl);
        List<BlogViewModel> GetAllBlog();
        public bool DeleteBlogById(Guid id);
        bool EditBlog(BlogViewModel blogViewModel, string imageUrl);
        Blog ApproveBlogByBlogId(Guid blogId);
        Blog DeclineBlogByBlogId(Guid blogId);
        List<BlogCommentViewModel> GetBlogComment(Guid blogId);
        BlogComment AddComment(BlogCommentViewModel blogCommentViewModel, Guid blogId, string userId);
        List<BlogSubscription> GetAllSubscribredUser();
        Blog BlogAuthor(string userId);
        List<BlogViewModel> GetBlogByUserId(string userId);
        BlogViewModel GetSingleBlog(Guid blogId);
    }
}
