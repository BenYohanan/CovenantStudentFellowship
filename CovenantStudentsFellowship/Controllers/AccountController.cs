using Core.DB;
using Core.Enums;
using Core.Models;
using Core.ViewModels;
using CovenantStudentsFellowship.Models;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CovenantStudentsFellowship.Controllers
{
    public class AccountController : BaseController
    {
        private AppDbContext _context;
        private IDropdownHelper _dropdownHelper;
        private IEmailHelper _emailHelper;
        private IEmailService _emailService;
        private IGeneralConfiguration _generalConfiguration;
        private IUserHelper _userHelper;
        private SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(AppDbContext context, IDropdownHelper dropdownHelper, IEmailHelper emailHelper, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, IUserHelper userHelper, IWebHostEnvironment webHostEnvironment, IGeneralConfiguration generalConfiguration, IEmailService emailService)
        {
            _context = context;
            _dropdownHelper = dropdownHelper;
            _emailHelper = emailHelper;
            _emailService = emailService;
            _userManager = userManager;
            _userHelper = userHelper;
            _generalConfiguration = generalConfiguration;
            _webHostEnvironment = webHostEnvironment;
            _signInManager = signInManager;
        }


        [HttpGet]
        public IActionResult GetStatesOfCountry(int CountryId)
        {
            var getStates = _context.States.Where(x => x.CountryId == CountryId && x.Active && !x.Deleted).ToList();
            return Json(new SelectList(getStates, "Id", "Name"));
        }

        [HttpGet]
        public async Task<IActionResult> UserRegistration()
        {
            ViewBag.host = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString();
            ViewBag.Gender = await _dropdownHelper.GetDropdownByKey(DropdownEnums.Gender).ConfigureAwait(false);
            ViewBag.Religion = await _dropdownHelper.GetDropdownByKey(DropdownEnums.Religion).ConfigureAwait(false);
            ViewBag.School = _dropdownHelper.GetSchools();
            ViewBag.State = await _dropdownHelper.GetState().ConfigureAwait(false);
            ViewBag.Country = await _dropdownHelper.GetCountry().ConfigureAwait(false);
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> UserRegistration(string applicationViewModel)
        {
            try
            {
                if (applicationViewModel != null)
                {
                    var userDetails = JsonConvert.DeserializeObject<ApplicationUserViewModel>(applicationViewModel);
                    if (userDetails.Password != userDetails.ConfirmPassword)
                    {
                        return Json(new { isError = true, msg = "Password and Confirm Password must match" });
                    }
                    var existingUser = await _userHelper.FindByEmailAsync(userDetails.Email);
                    if (existingUser != null)
                    {
                        return Json(new { isError = true, msg = "Email Address already belong to another user" });
                    }
                    var phoneNumber = _userHelper.FindByPhoneNumber(userDetails.PhoneNumber);
                    if (phoneNumber != null)
                    {
                        return Json(new { isError = true, msg = "Phone Number already belong to another User" });
                    }
                    var applicationUser = await _userHelper.CreateUsersAsync(userDetails);
                    if (applicationUser != null)
                    {
                        await _userManager.AddToRoleAsync(applicationUser, "User").ConfigureAwait(false);
                        var userToken = await _emailHelper.CreateUserToken(applicationUser.Email).ConfigureAwait(false);
                        if (userToken != null)
                        {
                            string linkToClick = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString() + "/Account/EmailVerified?token=" + userToken.Token;
                            var SendEmail = _emailHelper.VerificationEmail(applicationUser, linkToClick);
                            if (SendEmail)
                            {
                                return Json(new { isError = false, msg = "Registration successful, Please proceed to your email to complete your registration" });
                            }
                        }
                    }
                }
                return Json(new { isError = true, msg = "Registration Failed", data = applicationViewModel });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "User Registration Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to register a new user," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }

        }

        public IActionResult EmailVerified(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                Guid userToken = Guid.Parse(token);
                var userVerification = _context.UserVerifications.Where(u => u.Token == userToken).Include(us => us.User).FirstOrDefault();
                if (userVerification == null || userVerification.Token == Guid.Empty)
                {
                    return RedirectToAction("Login");
                }
                if (userVerification.User.EmailConfirmed)
                {
                    return View();
                }
                if (userVerification.Used)
                {
                    return View();
                }
                else
                {
                    userVerification.Used = true;
                    userVerification.DateUsed = DateTime.Now;
                    userVerification.User.EmailConfirmed = true;

                    _context.Update(userVerification);
                    _context.Update(userVerification.User);

                    var sendemail = _emailHelper.Gratitude(userVerification.User);
                    _context.SaveChanges();
                    return View();
                }
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                if (email != null && password != null)
                {
                    var user = _userHelper.FindByEmail(email);
                    if (user == null)
                    {
                        return Json(new { isError = true, msg = "Login with a correct email" });
                    }
                    if (user.Deactivated == true)
                    {
                        return Json(new { isError = true, msg = "This account has been deactivated, Contact support" });
                    }
                    if (user.EmailConfirmed == false)
                    {
                        return Json(new { isError = true, msg = "Your email address is not verified, kindly check your mail to verify your account", emailNotConfirmed = true });
                    }
                    var result = await _signInManager.PasswordSignInAsync(user, password, true, false).ConfigureAwait(false);
                    if (result.Succeeded)
                    {
                        user.Roles = (List<string>)await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                        foreach (var userRole in user.Roles)
                        {
                            switch (userRole)
                            {
                                case "SuperAdmin":
                                    user.UserRole = Session.Constants.SuperAdminRole;
                                    break;
                                case "SchoolAdmin":
                                    user.UserRole = Session.Constants.SchoolAdminRole;
                                    break;
                                case "User":
                                    user.UserRole = Session.Constants.UserRole;
                                    break;
                                default:
                                    break;
                            }
                        }
                        var currentUser = JsonConvert.SerializeObject(user);
                        HttpContext.Session.SetString("user", currentUser);
                        var validatedUrl = _userHelper.GetValidatedUrl();
                        return Json(new { isError = false, dashboard = validatedUrl });
                    }
                }
                return Json(new { isError = true, msg = "You Entered a Wrong Password, Please Retry" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "User Login Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to login a new user," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            ViewBag.host = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ForgotPassword(string applicationViewModel)
        {
            try
            {
                var userdetail = JsonConvert.DeserializeObject<ApplicationUserViewModel>(applicationViewModel);
                var userToken = await _emailHelper.CreateUserToken(userdetail.Email);
                if (userToken != null)
                {
                    string linkToClick = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString()
                        + "/Account/ResetPassword?token=" + userToken.Token;
                    var applicationUser = new ApplicationUser()
                    {
                        Email = userdetail.Email,
                    };
                    var sendEmail = _emailHelper.PasswordResetLink(applicationUser, linkToClick);
                    if (sendEmail)
                    {
                        await _context.SaveChangesAsync();
                        ModelState.Clear();
                        return Json(new { isError = false, msg = "A password reset link has been sent to your email, kindly check your inbox or Spam for the next action." });
                    }
                    return Json(new { isError = true, msg = "Check the email you entered and try again" });
                }
                return Json(new { isError = true, msg = "Email Address you entered does not belong to any account." });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Forgot Password Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to help user regain password," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(Guid token)
        {
            try
            {
                ViewBag.host = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString();
                if (token != Guid.Empty)
                {
                    PasswordResetViewModel passwordResetViewModel = new PasswordResetViewModel
                    {
                        Token = token
                    };
                    return View(passwordResetViewModel);
                }
                return View("Login");
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
                return View();
            }
        }

        [HttpPost]
        public async Task<JsonResult> ResetPassword(string viewmodel)
        {
            try
            {
                var userdetail = JsonConvert.DeserializeObject<PasswordResetViewModel>(viewmodel);
                if (ModelState.IsValid)
                {
                    if (userdetail.NewPassword != userdetail.ConfirmNewPassword)
                    {
                        return Json(new { isError = false, msg = "Password and confirm password must match" });
                    }
                    var userVerification = await _emailHelper.GetUserToken(userdetail.Token).ConfigureAwait(false);
                    if (userVerification != null && !userVerification.Used)
                    {
                        await _userManager.RemovePasswordAsync(userVerification.User).ConfigureAwait(false);
                        await _userManager.AddPasswordAsync(userVerification.User, userdetail.NewPassword).ConfigureAwait(false);
                        await _context.SaveChangesAsync().ConfigureAwait(false);
                        await _emailHelper.MarkTokenAsUsed(userVerification).ConfigureAwait(false);

                        var sendEmail = _emailHelper.PasswordResetConfirmation(userVerification.User);
                        if (sendEmail)
                        {
                            return Json(new { isError = false, msg = "Your Password has been reset successfully, you can now use the new password on your next login" });
                        }
                        return Json(new { isError = false, mgs = "Sorry! The Link You Entered is Invalid or Expired " });
                    }
                }
                return Json(viewmodel);
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Reset Password Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to help user Reset Password," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            ViewBag.host = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ChangePassword(string ViewModel)
        {
            try
            {
                var userDetail = JsonConvert.DeserializeObject<PasswordChangeViewModel>(ViewModel);
                if (userDetail != null && userDetail.NewPassword != null && userDetail.NewPassword != null)
                {
                    if (userDetail.NewPassword != userDetail.ConfirmPassword)
                    {
                        return Json(new { isError = false, msg = "Password and Confirm Password not matched" });
                    }
                    var user = Session.GetCurrentUser();
                    var result = await _userManager.ChangePasswordAsync(user, userDetail.NewPassword, userDetail.ConfirmPassword).ConfigureAwait(false);
                    var passwordReset = _emailHelper.PasswordChangeConfirmation(user);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignOutAsync().ConfigureAwait(false);
                        return Json(new { isError = false, msg = "Your Password has been changed successfully, you can now use the new password on your next login" });
                    }
                    return Json(new { isError = true, msg = "Your Old Password do not match with what you entered, please correct and try again" });
                }
                return Json(new { isError = true, msg = "Error Occurred incomplete details" });

            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "Change Password Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to help user Change Password," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }

        [HttpGet]
        public IActionResult ChangeEmail()
        {
            ViewBag.host = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ChangeEmail(string changeEmailViewModel)
        {
            try
            {
                var changeEmailDetail = JsonConvert.DeserializeObject<ApplicationUserViewModel>(changeEmailViewModel);
                var applicationUser = Session.GetCurrentUser();
                if (changeEmailDetail.Email == null || changeEmailDetail.ConfirmEmail == null)
                {
                    return Json(new { isError = true, msg = "Email or Confirm email can't be empty" });
                }
                if (changeEmailDetail.Email.ToLower() != changeEmailDetail.ConfirmEmail.ToLower())
                {
                    return Json(new { isError = true, msg = "Email and Confirm email must match" });
                }
                var checkIfEmailExist = _userHelper.FindByEmail(changeEmailDetail.Email);
                if (checkIfEmailExist != null)
                {
                    return Json(new { isError = true, msg = "Email Address already exist, check inbox or spam for account verification email." });
                }
                if (applicationUser != null && checkIfEmailExist == null)
                {
                    applicationUser.Email = changeEmailDetail.Email;
                    applicationUser.NormalizedEmail = changeEmailDetail.Email.ToUpper();
                    _context.Update(applicationUser);
                    _context.SaveChanges();
                }
                var userToken = await _emailHelper.CreateUserToken(applicationUser.Email).ConfigureAwait(false);
                if (userToken != null)
                {
                    string linkToClick = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString() + "/Account/EmailVerified?token=" + userToken.Token;
                    var SendEmail = _emailHelper.VerificationEmail(applicationUser, linkToClick);
                    if (SendEmail)
                    {
                        await _context.SaveChangesAsync();
                        return Json(new { isError = false, msg = "Email successful changed, check your email for verification link to continue." });
                    }
                    return Json(new { isError = true, msg = "Error Occurred While Sending message, Please Retry" });
                }
                return Json(new { isError = true, msg = "Email couldn't be changed, Please Retry" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "ChangeEmail Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to help user Change email," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }

        }

        [HttpPost]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<JsonResult> ResendEmail(string applicationViewModel)
        {
            try
            {
                if (applicationViewModel != null)
                {
                    var userEmail = JsonConvert.DeserializeObject<ApplicationUserViewModel>(applicationViewModel);
                    var email = await _userHelper.FindByEmailAsync(userEmail.Email);
                    if (email.Email == null && email == null)
                    {
                        return Json(new { isError = true, msg = "Email not registered" });
                    }
                    else
                    {
                        var userToken = await _emailHelper.CreateUserToken(email.Email).ConfigureAwait(false);
                        if (userToken != null)
                        {
                            string linkToClick = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString() + "/Account/EmailVerified?token=" + userToken.Token;

                            var SendEmail = _emailHelper.VerificationEmail(email, linkToClick);
                            if (SendEmail)
                            {
                                await _context.SaveChangesAsync();
                                return Json(new { isError = false, msg = "Email resend successful, Please proceed to your email to complete your registration" });
                            }
                        }
                    }
                }
                return Json(new { isError = true, msg = "Error occurred" });
            }
            catch (Exception ex)
            {
                string toEmail = _generalConfiguration.DeveloperEmail;
                string subject = "ResendEmail Page Exception on CSF Nigeria";
                var CurrentUrl = UriHelper.GetDisplayUrl(Request);
                string message = ex.Message + " , <br /> This exception message occurred when trying to Resend Email," +
                    " If you are a CSF developer, Please attend to it immediately.<br> The last page user accessed was : " + CurrentUrl;
                _emailService.SendEmail(toEmail, subject, message);
                throw;
            }
        }



        
    }
}
