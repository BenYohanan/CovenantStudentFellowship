using Core.Enums;
using Core.ViewModels;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CovenantStudentsFellowship.Controllers
{
    public class BlogController : BaseController
    {
        private IDropdownHelper _dropdownHelper;
        private IBlogHelper _blogHelper;
        private IEmailService _emailService;
        private IGeneralConfiguration _generalConfiguration;
        private IUserHelper _userHelper;

        public BlogController(IDropdownHelper dropdownHelper, IEmailHelper emailHelper, IBlogHelper blogHelper, IUserHelper userHelper, IGeneralConfiguration generalConfiguration, IEmailService emailService)
        {
            _dropdownHelper = dropdownHelper;
            _emailService = emailService;
            _userHelper = userHelper;
            _generalConfiguration = generalConfiguration;
            _blogHelper = blogHelper;
        }


        public IActionResult Index()
        {
            ViewBag.host = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString();
            var blogs = _blogHelper.GetAllBlog();
            if (blogs != null && blogs.Count > 0)
            {
                ViewBag.Blog = blogs.OrderByDescending(x => x.DateCreated).Take(5).ToList();
                var blogCategories = _dropdownHelper.ListOfBlogCategories();
                var firstFithneenBlog = blogs?.OrderByDescending(x => x.DateCreated).Take(15).ToList();
                var blogViewModel = new GeneralBlogViewModel()
                {
                    BlogCategories = blogCategories,
                    BlogViewModels = firstFithneenBlog,
                };
                return View(blogViewModel);
            }
            return View(blogs);
        }

        [Authorize]
        [HttpGet]
        public IActionResult CreateBlog()
        {
            ViewBag.BlogCategory = _dropdownHelper.GetDropdownByKey(DropdownEnums.BlogCategory).Result;
            return View();
        }

        [Authorize]
        [HttpPost]
        public JsonResult CreateBlog(string blogViewModel, string imageUrl)
        {
            try
            {
                var blog = JsonConvert.DeserializeObject<BlogViewModel>(blogViewModel);
                if (blog != null)
                {
                    var createBlog = _blogHelper.CreateBlog(blog, imageUrl);
                    if (createBlog.ToLower().Contains("successfully"))
                    {
                        return Json(new { isError = false, msg = "Blog created successfully. Kindly check your email within 24 hours for Admin review result" });
                    }
                };
                return Json(new { isError = true, msg = "Ensure that all compulsory fields are entered" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Create Blog Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to create a blog," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteBlog(Guid blogId)
        {
            try
            {
                if (blogId != Guid.Empty)
                {
                    var blogToBeDeleted = _blogHelper.DeleteBlogById(blogId);
                    if (blogToBeDeleted)
                    {
                        return Json(new { isError = false, msg = "Blog deleted successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Unable to delete Blog" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Delete Blog Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to delete a blog," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpGet]
        public IActionResult SingleBlog(Guid id)
        {
            ViewBag.Title = "Blog";
            ViewBag.host = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString();
            if (id != Guid.Empty)
            {
                var comments = _blogHelper.GetBlogComment(id);
                ViewBag.TopComment = comments.OrderByDescending(x => x.DateAdded).Take(7).ToList();
                var topBlog = _blogHelper.GetAllBlog().OrderByDescending(x => x.DateCreated).Take(5).ToList();
                var blogCategories = _dropdownHelper.ListOfBlogCategories();
                ViewBag.Blog = topBlog;
                if (id != Guid.Empty)
                {
                    var blog = _blogHelper.GetSingleBlog(id);
                    var blogViewModel = new GeneralBlogViewModel()
                    {
                        BlogCategories = blogCategories,
                        BlogViewModel = blog,
                    };
                    return View(blogViewModel);
                }
            }
            return View();
        }

        [HttpGet]
        public JsonResult AllComment(string blogCommentViewModel)
        {
            try
            {
                var blogComment = JsonConvert.DeserializeObject<BlogCommentViewModel>(blogCommentViewModel);
                if (blogComment.FullName != null && blogComment.Email != null)
                {
                    var blogs = _blogHelper.GetBlogComment(blogComment.BlogId);
                    if (blogs.Any())
                    {
                        return Json(blogs);
                    }
                }
                return Json(new { isError = true, msg = "Error occurred while fetching comment" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Get AllComment Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to get All Comment," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpPost]
        public JsonResult AddComment(string blogCommentViewModel, Guid blogId)
        {
            try
            {
                var loggedInUser = _userHelper.FindByEmail(User.Identity.Name);
                if (loggedInUser != null)
                {
                    if (blogId != Guid.Empty)
                    {
                        var blogComment = JsonConvert.DeserializeObject<BlogCommentViewModel>(blogCommentViewModel);
                        if (loggedInUser.Email != null && blogComment.Comment != null)
                        {
                            var blog = _blogHelper.AddComment(blogComment, blogId, loggedInUser.Id);
                            if (blog != null)
                            {
                                return Json(new { isError = false, msg = "Comment Added Successfully", data = blog });
                            }
                        }
                    }
                }
                else
                {
                    if (blogId != Guid.Empty)
                    {
                        var blogComment = JsonConvert.DeserializeObject<BlogCommentViewModel>(blogCommentViewModel);
                        if (blogComment.FullName != null && blogComment.Email != null && blogComment.Comment != null)
                        {
                            var blog = _blogHelper.AddComment(blogComment, blogId, loggedInUser?.Id);
                            if (blog != null)
                            {
                                return Json(new { isError = false, msg = "Comment Added Successfully", data = blog });
                            }
                        }
                    }
                }
                return Json(new { isError = true, msg = "Error occurred while add comments." });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Add Comment Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Add Comment," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult EditBlog(string blogViewModel, string imageUrl)
        {
            try
            {
                var blogModel = JsonConvert.DeserializeObject<BlogViewModel>(blogViewModel);
                if (blogModel.Id != Guid.Empty && blogModel.Title != null && blogModel.BlogContent != null)
                {
                    var blog = _blogHelper.EditBlog(blogModel, imageUrl);
                    if (blog)
                    {
                        return Json(new { isError = false, msg = "Blog Edited Successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Error occurred while updating your data." });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Edit Blog Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to edit a blog," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }
    }
}
