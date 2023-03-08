using Core.DB;
using Core.Enums;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace CovenantStudentsFellowship.Controllers
{
    [Authorize(Roles = "SuperAdmin, SchoolAdmin")]
    public class AdminController : BaseController
    {
        private AppDbContext _context;
        private IDropdownHelper _dropdownHelper;
        private IEmailHelper _emailHelper;
        private IBlogHelper _blogHelper;
        private IEmailService _emailService;
        private IGeneralConfiguration _generalConfiguration;
        private IUserHelper _userHelper;
        private SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager;
        private IAdminHelper _adminHelper;

        public AdminController(AppDbContext context, IDropdownHelper dropdownHelper, IEmailHelper emailHelper, SignInManager<ApplicationUser> signInManager, IBlogHelper blogHelper,
            UserManager<ApplicationUser> userManager, IUserHelper userHelper, IGeneralConfiguration generalConfiguration, IEmailService emailService, IAdminHelper adminHelper)
        {
            _context = context;
            _dropdownHelper = dropdownHelper;
            _emailHelper = emailHelper;
            _emailService = emailService;
            _userManager = userManager;
            _userHelper = userHelper;
            _generalConfiguration = generalConfiguration;
            _signInManager = signInManager;
            _adminHelper = adminHelper;
            _blogHelper = blogHelper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var adminPageDetails = new AdminPageViewModel();
            var weeklyActivities = _adminHelper.GetActivityForTheWeek();
            var blogs = _blogHelper.GetAllBlog();
            var recentBlogs = blogs?.OrderByDescending(x => x.DateCreated).Take(5).ToList();
            var events = _adminHelper.GetAllEvent();
            var recentEvents = events?.OrderByDescending(x => x.DateAdded).Take(5).ToList();
            adminPageDetails.BlogViewModel = recentBlogs;
            adminPageDetails.BlogCount = blogs.Count();
            adminPageDetails.Events = recentEvents;
            adminPageDetails.EventCount = events.Count();
            adminPageDetails.ActiveUserCount = _dropdownHelper.GetUsers().Count();
            adminPageDetails.WeeklyActivityViewModel = weeklyActivities;
            return View(adminPageDetails);
        }

        [HttpGet]
        public IActionResult AllBlogs()
        {
            var blogs = _blogHelper.GetAllBlog();
            if (blogs != null && blogs.Count > 0)
            {
                return View(blogs);
            }
            return View(blogs);
        }

        [HttpGet]
        public IActionResult BlogDetails(Guid blogId)
        {
            if (blogId != Guid.Empty)
            {
                var blogToBeEdited = _blogHelper.GetSingleBlog(blogId);
                if (blogToBeEdited != null)
                {
                    return View(blogToBeEdited);
                }
            }
            return View();
        }

        public IActionResult ApproveBlog(Guid blogId)
        {
            try
            {
                if (blogId != Guid.Empty)
                {
                    var approvedBlog = _blogHelper.ApproveBlogByBlogId(blogId);
                    if (approvedBlog != null) 
                    {
                        var sendBlogAuthorEmail = _emailHelper.NotificationOnBlogApprove(approvedBlog);
                        var subscribedUsers = _blogHelper.GetAllSubscribredUser();
                        if (subscribedUsers != null && subscribedUsers.Count > 0)
                        {
                            foreach (var blogSubscriber in subscribedUsers)
                            {
                                var sendEmailOnBlogUpdate = _emailHelper.NotificationOnNewBlogUpdate(blogSubscriber);
                            }
                        }
                        if (sendBlogAuthorEmail)
                        {
                            return RedirectToAction("AllBlogs", "Admin");
                        }
                    }
                }
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Approve Blog Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Approve blog," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        public IActionResult DelineBlog(Guid blogId)
        {
            try
            {
                var blog = _blogHelper.DeclineBlogByBlogId(blogId);
                if (blog != null)
                {
                    var sendBlogAuthorEmail = _emailHelper.NotificationOnBlogReject(blog);
                    if (sendBlogAuthorEmail)
                    {
                        return RedirectToAction("AllBlogs", "Admin");
                    }
                    return View();
                }
                
                return View();
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Approve Blog Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Approve blog," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpPost]
        public JsonResult AddEvent(string eventViewModel)
        {
            try
            {
                var loggedInUser = Session.GetCurrentUser();
                var eventModel = JsonConvert.DeserializeObject<EventViewModel>(eventViewModel);
                if (eventModel != null)
                {
                    var events = _adminHelper.AddEvent(eventModel, loggedInUser.SchoolId);
                    if (events != null)
                    {
                        return Json(new { isError = false, data = events, msg = "Event created successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Sorry Unable to Create Event" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Add Event Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to add an event," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpGet]
        public JsonResult EditEvent(int eventId)
        {
            try
            {
                if (eventId != 0)
                {
                    var myEvent = _context.Events.Where(e => e.Id == eventId).FirstOrDefault();
                    if (myEvent != null)
                    {
                        return Json(myEvent);
                    }
                }
                return Json(new { isError = true, msg = "Unable to retrieve event, please try again." });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Edit Event Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to get Edit Event page," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpPost]
        public JsonResult EditEvent(string eventViewModel)
        {
            try
            {
                var eventModel = JsonConvert.DeserializeObject<EventViewModel>(eventViewModel);
                if (eventModel != null)
                {
                    var myEvent = _adminHelper.EditEvent(eventModel);
                    if (myEvent)
                    {
                        return Json(new { isError = false, msg = "Event Edited Successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Error occurred while updating your data." });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Edit Event Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Edit Event," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventId)
        {
            try
            {
                if (eventId != 0)
                {
                    var eventToBeDeleted = _adminHelper.DeletEventById(eventId);
                    if (eventToBeDeleted)
                    {
                        return Json(new { isError = false, msg = "Event deleted successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Unable to delete event" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Delete Event Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to get delete Event page," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpGet]
        public IActionResult UploadedPictures()
        {
            var allPicture = _adminHelper.GetAllPicture();
            if (allPicture != null)
            {
                return View(allPicture);
            }
            return View(allPicture);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public JsonResult UploadPicture(string galleryViewModel, string fileUrl)
        {
            try
            {
                ViewBag.host = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString();
                ViewBag.Layout = _userHelper.GetRoleLayout();
                var loggedInUser = Session.GetCurrentUser();
                var galleryModel = JsonConvert.DeserializeObject<GalleryViewModel>(galleryViewModel);
                if (galleryModel != null)
                {
                    var addPicture = _adminHelper.AddPicture(galleryModel, fileUrl, loggedInUser.SchoolId);
                    if (addPicture != null)
                    {
                        return Json(new { isError = false, data = addPicture, msg = "Picture Uploaded successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Sorry Unable to Uploaded picture" });

            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Uploaded Picture Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Upload picture," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpPost]
        public JsonResult DeleteUploadedPicture(Guid photoId)
        {
            try
            {
                if (photoId != Guid.Empty)
                {
                    var pictureToBeDeleted = _adminHelper.DeletPictureById(photoId);
                    if (pictureToBeDeleted)
                    {
                        return Json(new { isError = false, msg = "Picture deleted successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Unable to delete picture" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Delete picture Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to get delete picture," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public JsonResult AddDocument(string documentViewModel, string base64)
        {
            try
            {
                ViewBag.Layout = _userHelper.GetRoleLayout();
                var loggedInUser = Session.GetCurrentUser();
                var documentModel = JsonConvert.DeserializeObject<DocumentViewModel>(documentViewModel);
                if (documentModel != null)
                {
                    var document = _adminHelper.AddDocument(documentModel, base64, loggedInUser.SchoolId);
                    if (document != null)
                    {
                        return Json(new { isError = false, data = document, msg = "Document uploaded successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Sorry unable to uploaded document" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Document Uploaded Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Upload document," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpPost]
        public JsonResult DeleteDocument(Guid documentId)
        {
            try
            {
                if (documentId != Guid.Empty)
                {
                    var document = _adminHelper.DeleteDocumentById(documentId);
                    if (document)
                    {
                        return Json(new { isError = false, msg = "Document deleted successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Unable to delete document" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Delete Document Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Delete Document," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }



        [HttpPost]
        public JsonResult AddDues(string duesViewModel)
        {
            try
            {
                var loggedInUser = Session.GetCurrentUser();
                var duesModel = JsonConvert.DeserializeObject<DuesViewModel>(duesViewModel);
                if (duesModel != null)
                {
                    var dues = _adminHelper.AddDues(duesModel, loggedInUser.SchoolId);
                    if (dues != null)
                    {
                        return Json(new { isError = false, data = duesModel, msg = "Dues added successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Sorry unable to create event" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Add Dues Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Add Dues," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpGet]
        public JsonResult EditDues(Guid duesId)
        {
            try
            {
                if (duesId != Guid.Empty)
                {
                    var dues = _context.Dues.Where(e => e.Id == duesId).FirstOrDefault();
                    if (dues != null)
                    {
                        return Json(dues);
                    }
                }
                return Json(new { isError = true, msg = "Unable to retrieve dues data, please try again." });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Edit Dues Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to get Edit dues page," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpPost]
        public JsonResult EditDues(string duesViewModel)
        {
            try
            {
                var duesModel = JsonConvert.DeserializeObject<DuesViewModel>(duesViewModel);
                if (duesModel.Id != Guid.Empty && duesModel.Name != null && duesModel.Amount > 0)
                {
                    var dues = _adminHelper.EditDues(duesModel);
                    if (dues)
                    {
                        return Json(new { isError = false, msg = "Dues edited successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Error occurred while updating your data." });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Edit Dues Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Edit dues," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpPost]
        public JsonResult DeleteDues(Guid duesId)
        {
            try
            {
                if (duesId != Guid.Empty)
                {
                    var duesToBeDeleted = _adminHelper.DeletDuesById(duesId);
                    if (duesToBeDeleted)
                    {
                        return Json(new { isError = false, msg = "Dues deleted successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Unable to delete dues" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Delete Dues Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to get delete dues page," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpGet]
        public IActionResult UserProfile(string userId)
        {
            if (userId != null)
            {
                var user = _userHelper.FindUserById(userId);

                if (user != null)
                {
                    return View(user);
                }
            }
            return View();
        }

        public JsonResult FilterBlog(BlogViewModel sortingModel)
        {
            try
            {
                if (sortingModel.Title != null)
                {
                    var filtredBlog = _adminHelper.GetFilteredBlog(sortingModel);
                    if (filtredBlog != null && filtredBlog.Count != 0)
                    {
                        return Json(new { isError = false, data = filtredBlog });
                    }
                }
                return Json(new { IsError = true, msg = "No Blog Found" });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public IActionResult AllUsers()
        {
            var allUsers = _adminHelper.GetAllUsers();
            if (allUsers.Any())
            {
                return View(allUsers);
            }
            return View(allUsers);
        }

        [HttpPost]
        public JsonResult DeactivatedUser(string userId)
        {
            try
            {
                if (userId != null)
                {
                    var user = _adminHelper.DeactivatedUserById(userId);
                    if (user)
                    {
                        return Json(new { isError = false, msg = "Users Deactivated successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Unable to Deactivated user" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Delete User Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Deactivated User," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpPost]
        public JsonResult ReactivateUser(string userId)
        {
            try
            {
                if (userId != null)
                {
                    var user = _adminHelper.ReactivateUserById(userId);
                    if (user)
                    {
                        return Json(new { isError = false, msg = "Users reactivated successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Unable to reactivated user" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Delete User Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Reactivated User," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }


        [HttpPost]
        public JsonResult AddManual(string manualDetails)
        {
            var semesterManual = JsonConvert.DeserializeObject<SemesterManualViewModel>(manualDetails);
            if (semesterManual != null)
            {
                var setupManual = _adminHelper.AddSemesterManualActivity(semesterManual);
                if (setupManual)
                {
                    return Json(new { isError = false, msg = "Activity added successfully" });
                }
            }
            return Json(new { isError = true, msg = "Error occurred" });
        }

        [HttpGet]
        public IActionResult SetUpSemesterMaual()
        {
            ViewBag.Speakers = _dropdownHelper.GetUsers();
            ViewBag.Cordinator = _dropdownHelper.GetUsers();
            return View();
        }
    }
}
