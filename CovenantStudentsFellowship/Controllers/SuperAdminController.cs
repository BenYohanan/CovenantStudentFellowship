using Core.DB;
using Core.Enums;
using Core.Models;
using Core.ViewModels;
using Logic.Helpers;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CovenantStudentsFellowship.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : BaseController
    {
        private AppDbContext _context;
        private IDropdownHelper _dropdownHelper;
        private IBlogHelper _blogHelper;
        private IUserHelper _userHelper;
        private IGeneralConfiguration _generalConfiguration;
        private ISuperAdminHelper _superAdminHelper;
        private IAdminHelper _adminHelper;
        private IEmailService _emailService;
        private IEmailHelper _emailHelper;
        private readonly UserManager<ApplicationUser> _userManager;
        public SuperAdminController(AppDbContext context, IDropdownHelper dropdownHelper, IBlogHelper blogHelper,
           IUserHelper userHelper, ISuperAdminHelper superAdminHelper, IGeneralConfiguration generalConfiguration, IEmailService emailService, UserManager<ApplicationUser> userManager, IAdminHelper adminHelper, IEmailHelper emailHelper)
        {
            _context = context;
            _dropdownHelper = dropdownHelper;
            _userHelper = userHelper;
            _blogHelper = blogHelper;
            _superAdminHelper = superAdminHelper;
            _generalConfiguration = generalConfiguration;
            _emailService = emailService;
            _userManager = userManager;
            _adminHelper = adminHelper;
            _emailHelper = emailHelper;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var superAdmin = new SuperAdminViewModel();
            superAdmin.BlogCount = _blogHelper.GetAllBlog().Count;
            superAdmin.BlogSubscriberCount = _blogHelper.GetAllSubscribredUser().Count;
            superAdmin.UserCount = _superAdminHelper.AllUserCount();
            superAdmin.SchoolCount = _superAdminHelper.AllRegisteredSchool().Count;
            return View(superAdmin);
        }

        [HttpPost]
        public async Task<JsonResult> AddSchool(string schoolViewModel)
        {
            if (schoolViewModel != null)
            {
                var schoolDetails = JsonConvert.DeserializeObject<SchoolViewModel>(schoolViewModel);
                if (schoolDetails != null)
                {
                    var existingUser = await _userHelper.FindByEmailAsync(schoolDetails.Email).ConfigureAwait(false);
                    if (existingUser != null)
                    {
                        return Json(new { isError = true, msg = "Email Address already belong to another user" });
                    }
                    var phoneNumber = _userHelper.FindByPhoneNumber(schoolDetails.SchoolAdinPhoneNumber);
                    if (phoneNumber != null)
                    {
                        return Json(new { isError = true, msg = "Phone Number already belong to another User" });
                    }
                    var addSchoolAndRegisterAdmin = await _superAdminHelper.AddSchool(schoolDetails).ConfigureAwait(false);
                    if (addSchoolAndRegisterAdmin != null)
                    {
                        var userToken = await _emailHelper.CreateUserToken(addSchoolAndRegisterAdmin.Email).ConfigureAwait(false);
                        if (userToken != null)
                        {
                            string linkToClick = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString() + "/Account/EmailVerified?token=" + userToken.Token;
                            var SendEmail = _emailHelper.SendSchoolAdminMessage(addSchoolAndRegisterAdmin, linkToClick, schoolDetails.SchoolCodeName);
                            if (SendEmail)
                            {
                                return Json(new { isError = false, msg = "School added successfully" });
                            }
                        }
                        return Json(new { isError = true, msg = "School added successfully, but error occurred while adding school admin" });
                    }
                }
            }
            return Json(new { isError = true, msg = "Unable to add school" });
        }

        [HttpGet]
        public JsonResult EditRegisteredSchool(Guid schoolId)
        {
            if (schoolId != Guid.Empty)
            {
                var schoolToBeEdited = _context.School.Where(x => x.Id == schoolId && x.Active && !x.Deleted).FirstOrDefault();
                if (schoolToBeEdited != null)
                {
                    return Json(schoolToBeEdited);
                }
            }
            return Json(new { isError = true, msg = "Unable to fetch school details" });
        }

        [HttpPost]
        public JsonResult EditRegisteredSchool(string schoolViewModel)
        {
            try
            {
                if (schoolViewModel != null)
                {
                    var schoolDetails = JsonConvert.DeserializeObject<SchoolViewModel>(schoolViewModel);
                    if (schoolDetails != null)
                    {
                        var editSchool = _superAdminHelper.EditSchool(schoolDetails);
                        if (editSchool)
                        {
                            return Json(new { isError = false, msg = "School edited successfully" });
                        }
                    }
                }
                return Json(new { isError = true, msg = "Unable to edit school" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Edit School Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to edit School," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpPost]
        public JsonResult DeleteSchool(Guid schoolId)
        {
            try
            {
                if (schoolId != Guid.Empty)
                {
                    var deleteSchool = _superAdminHelper.DeleteSchoolById(schoolId);
                    if (deleteSchool)
                    {
                        return Json(new { isError = false, msg = "School deleted successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Unable to deleted school" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Delete School Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to delete School," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpGet]
        public IActionResult HomePageImages()
        {
            ViewBag.Categories = _dropdownHelper.GetDropdownByKey(DropdownEnums.HomePageImage).Result;
            var homepPagePicture = _superAdminHelper.GetHomePagePictures();
            if (homepPagePicture.Any())
            {
                return View(homepPagePicture);
            }
            return View(homepPagePicture);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public JsonResult AddHomePagePicture(int pictureType, string base64)
        {
            try
            {
                ViewBag.host = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString();
                ViewBag.Layout = _userHelper.GetRoleLayout();
                if (pictureType != 0 && base64 != null)
                {
                    var picture = _superAdminHelper.AddHomePagePicture(pictureType, base64);
                    if (picture != null)
                    {
                        return Json(new { isError = false, data = picture, msg = "Picture Uploaded successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Sorry Unable to Uploaded picture" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "HomePage Picture Upload Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Upload Homepage picture," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpGet]
        public JsonResult EditHomepagePicture(int imageId)
        {
            ViewBag.Dropdown = _dropdownHelper.GetDropdownByKey(DropdownEnums.HomePageImage).Result.ToList();
            if (imageId != 0)
            {
                var picture = _context.HomePageImages.Where(b => b.Id == imageId && !b.Deleted && b.Active && b.HomePageImageId != null).Include(b => b.HomePagePicture).FirstOrDefault();
                if (picture != null)
                {
                    return Json(new { isError = false, data = picture });
                }
            }
            return Json(new { isError = true, msg = "Unable to retrieve data, please try again." });
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public JsonResult EditHomepagePicture(string homePagePicutreViewModel, string base64)
        {
            try
            {
                var homepagePicture = JsonConvert.DeserializeObject<HomePageImageViewModel>(homePagePicutreViewModel);
                if (homepagePicture != null)
                {
                    var picture = _superAdminHelper.EditHomePagePicture(homepagePicture, base64);
                    if (picture)
                    {
                        return Json(new { isError = false, msg = "Picture edited successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Error occurred while updating your data." });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = " Edit Homepage Picture Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Edit Homepage Picture," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpPost]
        public JsonResult DeleteHomePagePicture(int homepageImageId)
        {
            try
            {
                if (homepageImageId != 0)
                {
                    var pictureToBeDeleted = _superAdminHelper.DeletHomepagePictureById(homepageImageId);
                    if (pictureToBeDeleted)
                    {
                        return Json(new { isError = false, msg = "Homepage picture deleted successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Unable to delete homepage Picture" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Delete Homepage picture Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to get delete Homepage picture," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpGet]
        public IActionResult Roles()
        {
            ViewBag.Roles = _context.Roles.ToList();
            var userRoles = _superAdminHelper.GetRoleList();
            if (userRoles != null && userRoles.Count >0)
            {
                return View(userRoles);
            }
            return View(userRoles);
        }

        [HttpPost]
        public JsonResult AddUserRole(string roleViewModel)
        {
            ViewBag.roles = _context.Roles.ToList();
            ViewBag.Layout = _userHelper.GetRoleLayout();
            try
            {
                var roles = JsonConvert.DeserializeObject<RoleViewModel>(roleViewModel);
                var userDetails = _userHelper.FindByEmail(roles.User.Email);
                if (userDetails != null)
                {
                    var getUserRole = _userManager.GetRolesAsync(userDetails).Result;
                    foreach (var role in getUserRole)
                    {
                        var removeFromRole = _userManager.RemoveFromRoleAsync(userDetails, role).Result;
                    }
                    var getRoleName = _context.Roles.Where(c => c.Id == roles.Role.Id).FirstOrDefault();
                    if (getRoleName != null)
                    {
                        var addToRole = _userManager.AddToRoleAsync(userDetails, getRoleName.Name).Result;
                        if (addToRole != null)
                        {
                            return Json(new { isError = false, msg = "Role assigned successfully" });
                        }
                        return Json(new { isError = false, msg = "Unable to assign Role" });
                    }
                }
                return Json(new { isError = true, msg = "Unable to add Role" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Add User Role Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Add User Role," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;

            };
        }

        [HttpPost]
        public JsonResult DeleteRole(string roleId, string userId)
        {
            ViewBag.roles = _context.Roles.ToList();
            try
            {
                if (roleId != null && userId != null)
                {
                    var userRole = _context.Roles.Where(c => c.Id == roleId).FirstOrDefault();
                    var userDetails = _userHelper.FindByIdAsync(userId).Result;
                    if (userDetails != null && userRole != null)
                    {
                        var deteteRole = new IdentityResult();
                        deteteRole = _userManager.RemoveFromRoleAsync(userDetails, userRole.Name).Result;
                        if (deteteRole.Succeeded)
                        {
                            return Json(new { isError = false, msg = "Role removed successfully" });
                        }
                    }
                    return Json(new { isError = false, msg = "Unable to remove Role" });
                }
                return Json(new { isError = false, msg = "Error occurred" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Delete User Role Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to delete User Role," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> RegisteredSchools()
        {
            ViewBag.State = await _dropdownHelper.GetState().ConfigureAwait(false);
            ViewBag.Gender = await _dropdownHelper.GetDropdownByKey(DropdownEnums.Gender).ConfigureAwait(false);
            var allRegisteredSchool = _superAdminHelper.AllRegisteredSchool();
            if (allRegisteredSchool.Count > 0)
            {
                return View(allRegisteredSchool);
            }
            return View(allRegisteredSchool);
        }

        [HttpGet]
        public IActionResult DropDown()
        {
            var commonDropdowns = _context.CommonDropdowns.Where(d => d.Id > 0 && d.Name != null && d.Active && !d.Deleted).ToList();
            ViewBag.Dropdownkeys = _dropdownHelper.GetDropDownEnumsList();
            ModelState.Clear();
            if (commonDropdowns != null)
            {
                return View(commonDropdowns);
            }
            return View(commonDropdowns);
        }

        [HttpPost]
        public async Task<JsonResult> AddDropdown(string commonDropdowns)
        {
            ViewBag.host = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString();
            ViewBag.Layout = _userHelper.GetRoleLayout();
            ViewBag.Dropdownkeys = _dropdownHelper.GetDropDownEnumsList();
            try
            {
                var commonDropdown = JsonConvert.DeserializeObject<CommonDropdown>(commonDropdowns);
                if (commonDropdown != null)
                {
                    var dropdown = await _dropdownHelper.CreateDropdownsAsync(commonDropdown);
                    if (dropdown)
                    {
                        return Json(new { isError = false, msg = "Drop-down created successfully" });
                    }
                }
                return Json(new { isError = true, msg = "Sorry Unable to Create DropDown" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Add Drop-down Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to add drop-down," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpGet]
        public JsonResult EditDropdown(int dropdownId)
        {
            try
            {
                ViewBag.Dropdownkeys = _dropdownHelper.GetDropDownEnumsList();
                if (dropdownId > 0)
                {
                    var dropdown = _context.CommonDropdowns.Where(drp => drp.Id == dropdownId).FirstOrDefault();
                    var dropdownEnums = _dropdownHelper.GetDropDownEnumsList().Where(d => d.Id == dropdown.DropdownKey).ToList();
                    if (dropdownEnums.Count > 0)
                    {
                        return Json(new { isError = false, drop = dropdownEnums, data = dropdown });
                    }
                }
                return Json(new { isError = true, msg = "Unable to retrieve User Data,please try again." });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Edit Drop-down Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Edit Drop down," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpPost]
        public JsonResult EditDropdown(CommonDropdown commonDropdown)
        {
            ViewBag.Dropdownkeys = _dropdownHelper.GetDropDownEnumsList();
            if (commonDropdown.Name != null)
            {
                var dropdown = _dropdownHelper.EditDropdown(commonDropdown);
                if (dropdown)
                {
                    return Json(new { isError = false, msg = "Drop-down Edited Successfully" });
                }
            }
            return Json(new { isError = true, msg = "Error occurred" });
        }

        [HttpPost]
        public JsonResult DeleteDropdown(int dropdownId)
        {
            ViewBag.Dropdownkeys = _dropdownHelper.GetDropDownEnumsList();
            if (dropdownId > 0)
            {
                var dropownToBeDeleted = _dropdownHelper.DeleteDropdownById(dropdownId);
                if (dropownToBeDeleted)
                {
                    return Json(new { isError = false, msg = "Drop-down deleted successfully" });
                }
            }
            return Json(new { isError = true, msg = "Unable to delete drop-down" });
        }
    }
}
