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
using Newtonsoft.Json;

namespace CovenantStudentsFellowship.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private AppDbContext _context;
        private IDropdownHelper _dropdownHelper;
        private IEmailService _emailService;
        private IGeneralConfiguration _generalConfiguration;
        private IUserHelper _userHelper;
        private IBlogHelper _blogHelper;
        private IAdminHelper _adminHelper;
        public IPaystackLogic _paystackLogic;
        private UserManager<ApplicationUser> _userManager;

        public UserController(AppDbContext context, IDropdownHelper dropdownHelper, IBlogHelper blogHelper, IAdminHelper adminHelper,
            IUserHelper userHelper, IGeneralConfiguration generalConfiguration, IEmailService emailService, IPaystackLogic paystackLogic, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _dropdownHelper = dropdownHelper;
            _emailService = emailService;
            _userHelper = userHelper;
            _userHelper = userHelper;
            _generalConfiguration = generalConfiguration;
            _blogHelper = blogHelper;
            _adminHelper = adminHelper;
            _paystackLogic = paystackLogic;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.BlogCategory = _dropdownHelper.GetDropdownByKey(DropdownEnums.BlogCategory).Result;
            var documents = _adminHelper.GetAllDocument();
            var weeklyActivities = _adminHelper.GetActivityForTheWeek();
            var blogs = _blogHelper.GetAllBlog();
            var dues = _adminHelper.GetAllDues();
            var events = _adminHelper.GetAllEvent();
            var topEvent = events?.OrderByDescending(x => x.DateAdded).Take(5).ToList();
            ViewBag.AllBlogs = blogs;
            ViewBag.AllEvent = events;
            var userIndex = new AdminPageViewModel()
            {
                DocumentViewModel = documents,
                BlogViewModel = blogs,
                DuesViewModel = dues,
                Events = topEvent,
                WeeklyActivityViewModel = weeklyActivities,
            };
            return View(userIndex);
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile(string userId)
        {
            ViewBag.Gender = await _dropdownHelper.GetDropdownByKey(DropdownEnums.Gender).ConfigureAwait(false);
            ViewBag.Religion = await _dropdownHelper.GetDropdownByKey(DropdownEnums.Religion).ConfigureAwait(false);
            ViewBag.Country = await _dropdownHelper.GetCountry().ConfigureAwait(false);
            ViewBag.School = _dropdownHelper.GetSchools();
            ViewBag.State = await _dropdownHelper.GetState().ConfigureAwait(false);
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

        [HttpPost]
        public async Task<JsonResult> EditUserProfilePicture(string userId, string base64)
        {
            try
            {
                if (userId != null && base64 != null)
                {
                    var user =await _userHelper.FindByIdAsync(userId).ConfigureAwait(false);
                    if (user == null)
                    {
                        return Json(new { isError = true, msg = "User not found" });
                    }
                    user.ProfilePicture = base64;
                    _context.Update(user);
                    _context.SaveChanges();
                    return Json(new { isError = false, msg = "Picture Uploaded Successfully", data = user, url = "/User/UserProfile?userId=" + user.Id });
                }
                return Json(new { isError = true, msg = "choose a correct file" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Edit User Profile Picture Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when user is trying to Edit profile picture," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }

        }

        [HttpGet]
        public async Task<JsonResult> EditUserProfile(string userId)
        {
            ViewBag.Gender = await _dropdownHelper.GetDropdownByKey(DropdownEnums.Gender).ConfigureAwait(false);
            ViewBag.Religion = await _dropdownHelper.GetDropdownByKey(DropdownEnums.Religion).ConfigureAwait(false);
            ViewBag.State = await _dropdownHelper.GetState().ConfigureAwait(false);
            ViewBag.Country = await _dropdownHelper.GetCountry().ConfigureAwait(false);
            try
            {
                if (userId != null)
                {
                    var user = _userHelper.FindUserById(userId);
                    if (user != null)
                    {
                        return Json(user);
                    }
                }
                return Json(new { isError = true, msg = "Unable to retrieve user info!" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Edit User Profile Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when user is trying to get view on Edit user profile," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpPost]
        public JsonResult SubmitUserProfile(string applicationViewModel)
        {
            try
            {
                var userDetails = JsonConvert.DeserializeObject<ApplicationUserViewModel>(applicationViewModel);
                if (userDetails != null)
                {
                    return Json(new { isError = true, msg = "Email is required, please enter email" });
                }
                if (userDetails.PhoneNumber == "")
                {
                    return Json(new { isError = true, msg = "Phone number is required, please enter phone number" });
                }
                if (userDetails.Department != null)
                {
                    return Json(new { isError = true, msg = "Department is required, please add department to continue " });
                }
                if (userDetails.HomeSynagogue == "")
                {
                    return Json(new { isError = true, msg = "Home Synagogue is required, please enter Home Synagogue" });
                }
                if (userDetails.SchoolAddress == "")
                {
                    return Json(new { isError = true, msg = "School Address is required, please enter School Address" });
                }
                var editedUserProfile = _userHelper.EditUserProfile(userDetails);
                if (editedUserProfile)
                {
                    return Json(new { isError = false, msg = " User profile edited Successfully", url = "/User/UserProfile?userId=" + userDetails.Id });
                }
                return Json(new { isError = true, msg = "Error Occurred while updating your details please retry." });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Edit User Profile Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when user is trying to submit Edit user profile," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Photos()
        {
            try
            {
                ViewBag.host = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString();
                var picture = _adminHelper.GetAllPicture();
                if (picture.Count > 0)
                {
                    return View(picture);
                }
                return View(picture);
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "All Picture Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to display Pictures to user," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult AddSubscribersEmail(string blogUpdate)
        {
            try
            {
                var blogUpdateModel = JsonConvert.DeserializeObject<BlogSubscription>(blogUpdate);
                if (blogUpdateModel != null && blogUpdateModel.Email != null)
                {
                    var addUser = _userHelper.AddSubscriber(blogUpdateModel);
                    if (addUser != null)
                    {
                        return Json(new { isError = false, msg = "Subscription successful, you will be updated once a new blog is uploaded" });
                    }
                }
                return Json(new { isError = true, msg = "Please enter email" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Send Subscriber Email Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Send Subscriber Email on blog update," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        public JsonResult DownloadPdf(Guid documentId)
        {
            if (documentId != Guid.Empty)
            {
                var document = _context.Document.Where(x => x.Id == documentId && x.FileUrl != null).FirstOrDefault();
                if (document != null)
                {
                    return Json(new { isError = false, msg = "Downloaded Successfully", data = document });
                }
            }
            return Json(new { isError = true, msg = "Download Failed" });
        }

        [HttpGet]
        public async Task<IActionResult> CompleteRegistration(string userId)
        {
            var loggedInUser = await _userHelper.FindByIdAsync(userId).ConfigureAwait(false);
            if (loggedInUser != null)
            {
               ViewBag.host = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString();
                ViewBag.Religion = await _dropdownHelper.GetDropdownByKey(DropdownEnums.Religion).ConfigureAwait(false);
                ViewBag.State = await _dropdownHelper.GetState().ConfigureAwait(false);
                ViewBag.Country = await _dropdownHelper.GetCountry().ConfigureAwait(false);
                return View();
            }
            return View();
        }

        [HttpPost]
        public JsonResult CompleteRegistration(string base64, string applicationUserViewModel)
        {
            try
            {
                var loggedInUser = Session.GetCurrentUser();
                var userDetails = JsonConvert.DeserializeObject<ApplicationUserViewModel>(applicationUserViewModel);
                if (userDetails != null)
                {
                    var updateUserDetails = _userHelper.CompleteRegistration(userDetails, loggedInUser, base64);
                    if (updateUserDetails != null)
                    {
                        var dashboard = "";
                        var userRole = loggedInUser.UserRole;
                        if (userRole == Session.Constants.SuperAdminRole)
                        {
                            updateUserDetails.UserRole = Session.Constants.SuperAdminRole;
                            dashboard = "SuperAdmin/Index";
                        }
                        else if (userRole == Session.Constants.SchoolAdminRole)
                        {
                            updateUserDetails.UserRole = Session.Constants.SchoolAdminRole;
                            dashboard = "Admin/Index";
                        }
                        else
                        {
                            updateUserDetails.UserRole = Session.Constants.UserRole;
                            dashboard = "User/Index";
                        }
                        var currentUser = JsonConvert.SerializeObject(updateUserDetails);
                        HttpContext.Session.SetString("user", currentUser);
                        return Json(new { isError = false, msg = "Details updated Successfully", dashboard = dashboard });
                    }
                    return Json(new { isError = true, msg = "Unable to update user details " });
                }
                return Json(new { isError = true, msg = "Unable to retrieve data" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Update User Details";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Complete user registration," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Donation()
        {
            ViewBag.host = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString();
            ViewBag.DonationCategory = _dropdownHelper.DonationCategory();
            ViewBag.DonationCurrency = _dropdownHelper.DonationCurrency();
            ViewBag.County = _dropdownHelper.GetCountry().Result;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult MakePayment(string paymentDetails)
        {
            if (paymentDetails != null)
            {
                var donationViewModel = JsonConvert.DeserializeObject<DonationViewModel>(paymentDetails);
                if (donationViewModel != null)
                {
                    var paystackPaymentResponse = _paystackLogic.GeneratePaymentParameters(donationViewModel).Result;
                    if (paystackPaymentResponse != null)
                    {
                        return Json(paystackPaymentResponse.data.authorization_url);
                    }
                    return Json(new { isError = true, msg = "Donation couldn't be processed, please retry later" });
                }
            }
            return Json(new { isError = true, msg = "Error occurred" });
        }

        [HttpGet]
        public IActionResult SemesterManual()
        {
            ViewBag.Speakers = _dropdownHelper.GetUsers();
            var semesterActivities = _adminHelper.SemesterManuals();
            if (semesterActivities != null && semesterActivities.Count > 0)
            {
                return View(semesterActivities);
            }
            return View(semesterActivities);
        }
        public IActionResult PaystackResponseFeedback(Paystack paystack)
        {
            try
            {
                var walletPaymentForm = _context.DonationPackage.Where(x => x.InvoiceNumber == paystack.reference).FirstOrDefault();
                if (walletPaymentForm != null || walletPaymentForm.Id != Guid.Empty)
                {
                    var isPaymentVerified = _paystackLogic.VerifyPayment(paystack.reference);
                    return RedirectToAction("Index", "Home", new { Area = "" });
                }
                return RedirectToAction("Donation", "User", new { Area = "" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = " Pay-stack Feedback after every payment Page Exception on CSF";
                string message = ex.Message + " ,This exception message is from this current action being hit, If you are a CSF developer, Please attend to it immediately.<br> The last link user accessed was => ";
                _emailService.SendEmail(toEmail, subject, message);

                throw;
            }


        }

        [HttpGet]
        public IActionResult Document()
        {
            ViewBag.Document = _dropdownHelper.GetDocumentFromEnums();
            var documents = _adminHelper.GetAllDocument();
            if (documents.Any())
            {
                return View(documents);
            }
            return View(documents);
        }

        [HttpGet]
        public IActionResult AllDues()
        {
            ViewBag.host = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString();
            var dues = _adminHelper.GetAllDues();
            if (dues.Any())
            {
                return View(dues);
            }
            return View(dues);
        }
        [HttpGet]
        public IActionResult AllEvent()
        {
            var events = _adminHelper.GetAllEvent();
            if (events != null && events.Count > 0)
            {
                return View(events);
            }
            return View(events);
        }
    }
}
