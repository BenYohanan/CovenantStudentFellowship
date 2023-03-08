using Core.DB;
using Core.Enums;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.EntityFrameworkCore;

namespace Logic.Helpers
{
    public class BlogHelper : IBlogHelper
    {
        private AppDbContext _context;
        private IDropdownHelper _dropdownHelper;

        public BlogHelper(AppDbContext context, IDropdownHelper dropdownHelper)
        {
            _context = context;
            _dropdownHelper = dropdownHelper;
        }


        public string CreateBlog(BlogViewModel createBlog, string imageUrl)
        {
            if (createBlog.Title != null && createBlog.BlogContent != null && createBlog.AboutMe != null)
            {
                var user = Session.GetCurrentUser();
                if (user != null)
                {
                    var newBlog = new Blog
                    {
                        AddedById = user.Id,
                        AboutMe = createBlog.AboutMe,
                        Title = createBlog.Title,
                        BlogContent = createBlog.BlogContent,
                        ImageUrl = imageUrl,
                        DateAdded = DateTime.Now,
                        Deleted = false,
                        Active = true,
                        BlogStatus = BlogStatus.Pending,
                        BlogCategoryId = createBlog.BlogCategoryId
                    };
                    _context.Blog.Add(newBlog);
                    _context.SaveChanges();
                    return ("Blog created successfully");
                }
            }
            return null;
        }

        public List<BlogViewModel> GetAllBlog()
        {
            var blogs = new List<BlogViewModel>();
            var allBlogs = _context.Blog.Where(x => x.Id != Guid.Empty && !x.Deleted && x.AddedById != null && x.BlogStatus != 0)
                .Include(x => x.AddedBy)
                .Include(x => x.BlogCategory)
                .Include(x=>x.AddedBy.School).ToList();
            if (allBlogs.Count > 0)
            {
                foreach (var item in allBlogs)
                {
                    var blogComments = GetBlogComment(item.Id);
                    var blogCategories = _dropdownHelper.ListOfBlogCategories();
                    var blogViewModel = new BlogViewModel
                    {
                        Id = item.Id,
                        AddedById = item.AddedBy.Id,
                        BlogAutorName = item.AddedBy?.FullName,
                        BlogAuthorEmail = item.AddedBy?.Email,
                        BlogAuthorPhone = item.AddedBy?.PhoneNumber,
                        BlogContent = item.BlogContent,
                        Title = item.Title,
                        AboutMe = item.AboutMe,
                        BlogComments = blogComments,
                        BlogAutorProfilePicture = item.AddedBy?.ProfilePicture,
                        BlogAutorSchool = item.AddedBy?.School?.SchoolCodeName,
                        DateCreated = item.DateAdded.ToString("D"),
                        BlogCategories = blogCategories,
                        BlogStatus = item.BlogStatus.ToString(),
                        BlogCategoryName = item.BlogCategory?.Name,
                        BlogCategoryId = item.BlogCategoryId,
                        ShortDetails = item.ShortDetails
                    };
                    blogs.Add(blogViewModel);
                }
                return blogs;
            }
            return blogs;
        }

        public bool DeleteBlogById(Guid id)
        {
            if (id != Guid.Empty)
            {
                var blog = _context.Blog.Where(d => d.Id == id && d.Active && !d.Deleted && d.AddedById != null).Include(d => d.AddedBy).FirstOrDefault();
                if (blog != null)
                {
                    blog.Deleted = true;
                    _context.Blog.Update(blog);
                    _context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool EditBlog(BlogViewModel blogViewModel, string imageUrl)
        {
            if (blogViewModel != null)
            {
                var blog = _context.Blog.Where(c => c.Id == blogViewModel.Id && !c.Deleted && c.Active && c.AddedById != null).Include(c => c.AddedBy).FirstOrDefault();
                if (blog != null)
                {
                    blog.Id = blogViewModel.Id;
                    blog.Title = blogViewModel.Title;
                    blog.BlogContent = blogViewModel.BlogContent;
                    blog.Deleted = false;
                    blog.ImageUrl = imageUrl;
                    blog.Active = true;

                    _context.Update(blog);
                    _context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public BlogViewModel GetSingleBlog(Guid blogId)
        {
            if (blogId != Guid.Empty)
            {
                var blog = _context.Blog.Where(c => c.Id == blogId && !c.Deleted && c.Active && c.AddedById != null && c.BlogCategoryId != null)
                    .Include(c => c.AddedBy)
                    .Include(c => c.BlogCategory)
                    .Include(c => c.AddedBy.School).FirstOrDefault();
                if (blog != null)
                {
                    var blogComments = GetBlogComment(blog.Id);
                    var blogCategories = _dropdownHelper.ListOfBlogCategories();
                    var blogModel = new BlogViewModel
                    {
                        Id = blog.Id,
                        AddedById = blog.AddedBy.Id,
                        BlogAutorName = blog.AddedBy?.FullName,
                        BlogAuthorEmail = blog.AddedBy?.Email,
                        BlogAuthorPhone = blog.AddedBy?.PhoneNumber,
                        BlogAutorSchool = blog.AddedBy?.School?.SchoolCodeName,
                        BlogContent = blog.BlogContent,
                        Title = blog.Title,
                        AboutMe = blog.AboutMe,
                        BlogComments = blogComments,
                        BlogAutorProfilePicture = blog.AddedBy?.ProfilePicture,
                        DateCreated = blog.DateAdded.ToString("D"),
                        BlogCategories = blogCategories,
                        BlogStatus = blog.BlogStatus.ToString(),
                        BlogCategoryName = blog.BlogCategory?.Name,
                        BlogCategoryId = blog.BlogCategoryId
                    };
                    return blogModel;
                }
            }
            return null;
        }

        public Blog ApproveBlogByBlogId(Guid blogId)
        {
            if (blogId != Guid.Empty)
            {
                var blog = _context.Blog.Where(d => d.Id == blogId && d.Active && !d.Deleted && d.AddedById != null).Include(x=>x.AddedBy).FirstOrDefault();
                if (blog != null)
                {
                    blog.BlogStatus = BlogStatus.Approved;
                    _context.Blog.Update(blog);
                    _context.SaveChanges();
                    return blog;
                }
            }
            return null;
        }

        public Blog DeclineBlogByBlogId(Guid blogId)
        {
            if (blogId != Guid.Empty)
            {
                var blog = _context.Blog.Where(d => d.Id == blogId && d.Active && !d.Deleted && d.AddedById != null).Include(x=>x.AddedBy).FirstOrDefault();
                if (blog != null)
                {
                    blog.BlogStatus = BlogStatus.Decline;
                    _context.Blog.Update(blog);
                    _context.SaveChanges();
                    return blog;
                }
            }
            return null;
        }

        public List<BlogCommentViewModel> GetBlogComment(Guid blogId)
        {
            var comments = new List<BlogCommentViewModel>();
            if (blogId != Guid.Empty)
            {
                var allComment = _context.BlogComment.Where(x => x.Id != Guid.Empty && !x.Deleted && x.FullName != null && x.Email != null && x.BlogId == blogId)
                    .Include(x => x.Blog).Include(x => x.BlogCategory)
                    .Select(x => new BlogCommentViewModel
                    {
                        Comment = x.Comment,
                        Email = x.Email,
                        FullName = x.FullName,
                        BlogName = x.Blog.Name,
                        DateAdded = x.DateAdded,
                        BlogCategoryName = x.BlogCategory.Name,
                        BlogCategoryId = x.BlogCategoryId,
                        ImageUrl = x.ImageUrl
                    }).ToList();
                if (allComment.Count > 0)
                {
                    return allComment;
                }
            }
            return comments;
        }

        public BlogComment AddComment(BlogCommentViewModel blogCommentViewModel, Guid blogId, string userId)
        {
            if (userId != null)
            {
                var loggedInUser = _context.ApplicationUser.Where(x => x.Id == userId && !x.Deactivated).FirstOrDefault();
                if (loggedInUser != null)
                {
                    var comment = new BlogComment
                    {
                        FullName = loggedInUser.FullName,
                        Email = loggedInUser.Email,
                        Comment = blogCommentViewModel.Comment,
                        BlogId = blogId,
                        DateAdded = DateTime.Now,
                        Deleted = false,
                        Active = true,
                        ImageUrl = loggedInUser.ProfilePicture
                    };
                    _context.BlogComment.Add(comment);
                    _context.SaveChanges();
                    return comment;
                }
                return null;
            }
            else
            {
                if (blogCommentViewModel.FullName != null && blogCommentViewModel.Email != null && blogCommentViewModel.Comment != null)
                {
                    var comment = new BlogComment
                    {
                        FullName = blogCommentViewModel.FullName,
                        Email = blogCommentViewModel.Email,
                        Comment = blogCommentViewModel.Comment,
                        BlogId = blogId,
                        DateAdded = DateTime.Now,
                        Deleted = false,
                        Active = true,
                    };
                    _context.BlogComment.Add(comment);
                    _context.SaveChanges();
                    return comment;
                }
                return null;
            }
        }

        public List<BlogSubscription> GetAllSubscribredUser()
        {
            var allSubscribers = new List<BlogSubscription>();
            var subscriber = _context.BlogSubscription.Where(x => x.Id != Guid.Empty && x.Active && !x.Deleted && x.Email != null).ToList();
            if (subscriber.Any())
            {
                return subscriber;
            }
            return allSubscribers;
        }

        public Blog BlogAuthor(string userId)
        {
            var author = _context.Blog.Where(a => a.Id != Guid.Empty && a.Active && !a.Deleted && a.AddedById == userId).Include(a => a.AddedBy).FirstOrDefault();
            if (author != null)
            {
                return author;
            }
            return null;
        }

        public List<BlogViewModel> GetBlogByUserId(string userId)
        {
            var allMyBlog = new List<BlogViewModel>();
            if (userId != null)
            {
                var blog = _context.Blog.Where(c => !c.Deleted && c.Active && c.AddedById == userId).Include(c => c.AddedBy).Include(c => c.BlogCategory).ToList();
                if (blog != null)
                {
                    foreach (var myBlog in blog)
                    {
                        var blogComment = GetBlogComment(myBlog.Id);
                        var blogModel = new BlogViewModel
                        {
                            Id = myBlog.Id,
                            BlogAutorName = myBlog.AddedBy?.FullName,
                            AboutMe = myBlog.AboutMe,
                            BlogContent = myBlog.ShortDetails,
                            DateCreated = myBlog.DateAdded.ToString("D"),
                            Title = myBlog.Title,
                            BlogComments = blogComment,
                            Active = true,
                            ImageUrl = myBlog.ImageUrl,
                            BlogCategoryName = myBlog.BlogCategory?.Name,
                            BlogCategoryId = myBlog.BlogCategoryId
                        };
                        allMyBlog.Add(blogModel);
                    }
                    return allMyBlog;
                }
            }
            return allMyBlog;
        }
    }
}
