using Core.DB;
using Core.Models;
using Hangfire;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace Logic.Helpers
{
    public class EmailHelper : IEmailHelper
    {
        private readonly IUserHelper _userHelper;
        private readonly IEmailService _emailService;
        private readonly AppDbContext _context;
        private readonly IGeneralConfiguration _generalConfiguration;
        public EmailHelper(AppDbContext context, IUserHelper userHelper, IEmailService emailService, IGeneralConfiguration generalConfiguration)
        {
            _context = context;
            _emailService = emailService;
            _userHelper = userHelper;
            _generalConfiguration = generalConfiguration;
        }


        public async Task<UserVerification> CreateUserToken(string userEmail)
        {
            var user = await _userHelper.FindByEmailAsync(userEmail);
            if (user != null)
            {
                UserVerification userVerification = new UserVerification()
                {
                    UserId = user.Id,
                };
                await _context.AddAsync(userVerification);
                await _context.SaveChangesAsync();
                return userVerification;
            }
            return null;
        }

        public async Task<UserVerification> GetUserToken(Guid token)
        {
            return await _context.UserVerifications.Where(t => t.Used != true && t.Token == token)?.Include(s => s.User).FirstOrDefaultAsync();
        }

        public bool VerificationEmail(ApplicationUser applicationUser, string linkToClick)
        {
            if (applicationUser != null)
            {
                string toEmail = applicationUser.Email;
                string subject = "Shalom Aleichem! Activate your account";
                string message = "Thank you for registering " + applicationUser.FullName + " You’re only one click away from accessing your account. <br/> " +
                    "<br/>" + "<a  href='" + linkToClick + "' target='_blank'>" + "<button style='color:white; background-color:#00A7EF; padding:10px; border:10px;'>Verify Email</button>" + "</a> <br/>" +
                    "<br/>" + "If the link does not work, please copy this URL and paste it in to your browser's address bar: " + linkToClick + "<br/>" +
                    "Need help? We’re here for you. Simply reply to this email to contact us. <br/>" +

                    "<br/>" +
                    "Kind regards,<br/>" +
                    "Covenant Students' Fellowship Nigeria";
                _emailService.SendEmail(toEmail, subject, message);
                return true;
            };
            return false;
        }

        public bool Gratitude(ApplicationUser applicationUser)
        {
            if (applicationUser != null)
            {
                string toEmail = applicationUser.Email;
                string subject = "TODARABA";
                string message = "Dear " + applicationUser.FullName + ", thank you for registering with us.<br/> " +
                "Need help? We’re here for you.Simply reply to this email to contact us. <br/>" +
                "<br/>" +
                "Kind regards,<br/>" +
                "Covenant Students' Fellowship Nigeria";
                _emailService.SendEmail(toEmail, subject, message);
                return true;
            };
            return false;
        }

        public bool PasswordResetLink(ApplicationUser applicationUser, string linkToClick)
        {
            if (applicationUser.Email != null)
            {
                var user = _context.ApplicationUser.Where(u => u.UserName == applicationUser.Email).FirstOrDefault();
                if (user != null)
                {
                    string toEmail = applicationUser.Email;
                    string subject = "Shalom Aleichem - RESET PASSWORD";
                    string message = "A password reset has been requested to your CSF Account email address, please click on the button below to create a new password <br>" +
                        "<br/>" + "<a style:'border:2px;' href='" + linkToClick + "' target='_blank'>" + "<button style='color:white; background-color:#00A7EF; padding:10px; border:1px;'>Reset Password</button>" + "</a>" +
                        "<br/> or copy the link below to your browser </br>" + linkToClick + "<br/>" +
                        "Please make sure you've entered the address you registered with." +
                        "Need help? We’re here for you.Simply reply to this email to contact us. <br/>" +
                        "<br/>" +
                        "<b>Kind regards,</b><br/>" +
                        "<b>Covenant Students' Fellowship Nigeria</b>";
                    _emailService.SendEmail(toEmail, subject, message);
                    return true;
                }
            };
            return false;
        }

        public bool PasswordResetConfirmation(ApplicationUser applicationUser)
        {
            if (applicationUser.Email != null)
            {
                string toEmail = applicationUser.Email;
                string subject = "Shalom Aleichem - Password Reset Successfully";
                string message = "Password reset Successfully, please login with your new details to continue" +
                    "<br/>Need help? We’re here for you.Simply reply to this email to contact us. <br/>" +
                    "<br/>" +

                    "Kind regards,<br/>" +
                    "Covenant Students' Fellowship Nigeria";
                _emailService.SendEmail(toEmail, subject, message);
                return true;
            };
            return false;
        }

        public async Task<bool> MarkTokenAsUsed(UserVerification userVerification)
        {
            var VerifiedUser = _context.UserVerifications.Where(s => s.UserId == userVerification.User.Id && s.Used != true).FirstOrDefault();
            if (VerifiedUser != null)
            {
                userVerification.Used = true;
                userVerification.DateUsed = DateTime.Now;
                _context.Update(userVerification);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public bool PasswordChangeConfirmation(ApplicationUser applicationUser)
        {
            if (applicationUser.Email != null)
            {
                string toEmail = applicationUser.Email;
                string subject = " Shalom Aleichem - Password Changed Successfully";
                string message = "Password Changed Successfully, please login with your new password to continue" +
                    "Need help? We’re here for you.Simply reply to this email to contact us. <br/>" +
                    "<br/>" +
                    "Kind regards,<br/>" +
                    "Covenant Student's Fellowship Nigeria";
                _emailService.SendEmail(toEmail, subject, message);
                return true;
            };
            return false;
        }

        public bool SendSchoolAdminMessage(ApplicationUser applicationUser, string linkToClick, string schoolCodeName)
        {
            if(applicationUser != null)
            {
                string toEmail = applicationUser.Email;
                string subject = "Shalom Aleichem! Activate your account";
                string message = "Dear " + applicationUser.FullName + ", you have made an admin of CSF" + schoolCodeName + ". <br/>" + 
                                    "Your password is 123456, you can change your password using the reset password link when you login <br/>"+
                                    "You’re only one click away from accessing your account. <br/> " +
                                    "<br/>" + "<a  href='" + linkToClick + "' target='_blank'>" + "<button style='color:white; background-color:#00A7EF; padding:10px; border:10px;'>Verify Email</button>" + "</a> <br/>" +
                                    "<br/>" + "If the link does not work, please copy this URL and paste it in to your browser's address bar: " + linkToClick + "<br/>" +
                                    "Need help? We’re here for you. Simply reply to this email to contact us. <br/>" +

                                    "Kind regards,<br/>" +
                    "Covenant Student's Fellowship Nigeria";
                _emailService.SendEmail(toEmail, subject, message);
                return true;
            };
            return false;
        }

        public bool NotificationOnNewBlogUpdate(BlogSubscription SubscriberedUser)
        {
            if (SubscriberedUser.Email != null)
            {
                string toEmail = SubscriberedUser.Email;
                string subject = "Shalom Aleichem!";
                string message = " A new blog have been uploaded to the site, read and get inspired. We are as well expecting a write up from you in the future." +
                    "<br/" + "Feel free to share your advice, we grow by taking advice from our respected members." +
                    "<br/> Simply reply to this email to contact us. <br/>" +
                    "<br/>" +

                    "Kind regards,<br/>" +
                    "Covenant Students' Fellowship Nigeria";
                _emailService.SendEmail(toEmail, subject, message);
                return true;
            };
            return false;
        }

        public bool NotificationOnBlogApprove(Blog blog)
        {
            if (blog != null)
            {
                string toEmail = blog.AddedBy?.Email;
                string subject = "Shalom Aleichem!";
                string message = blog.AddedBy?.FullName + " Your blog have been approved, thank your for adding value to lives." +
                    "<br/> We are expecting more in the future from you, Feel free to share your advice, we grow by taking advice from our respected members." +
                    "<br/> Simply reply to this email to contact us. <br/>" +
                    "<br/>" +

                    "Kind regards,<br/>" +
                    "Covenant Students' Fellowship Nigeria";
                _emailService.SendEmail(toEmail, subject, message);
                return true;
            };
            return false;
        }

        public bool NotificationOnBlogReject(Blog blog)
        {
            if (blog != null)
            {
                string toEmail = blog.AddedBy?.Email;
                string subject = "Shalom Aleichem!";
                string message = blog.AddedBy?.FullName + " Your blog have been rejected due to some write up we didn't accept." +
                    "<br/>Thank your for adding value to lives and we are expecting more from you." +
                    "<br/" + "Feel free to share your advice, we grow by taking advice from our respected users." +
                    "<br/> Simply reply to this email to contact us. <br/>" +
                    "<br/>" +

                    "Kind regards,<br/>" +
                    "Covenant Students' Fellowship Nigeria";
                _emailService.SendEmail(toEmail, subject, message);
                return true;
            };
            return false;
        }

        public void HangFireFirstCronReminderJob()
        {
            RecurringJob.AddOrUpdate(() => GetUserAndSendMessage(), _generalConfiguration.CronReminderExpressionTime, TimeZoneInfo.Utc);
        }

        public async Task<bool> GetUserAndSendMessage()
        {
            var users = _context.ApplicationUser.Where(x => x.Id != null && x.DateOfBirth.Day == DateTime.Now.Day && !x.Deactivated).ToList();
            if (users != null && users.Count > 0)
            {
                foreach (var user in users)
                {
                    string userPhoneNumber = user.PhoneNumber;

                    if (userPhoneNumber != null)
                    {
                        await SendBirthdayMessageToUser(user);
                    }
                }
                return true;
            }
            return false;
        }

        public async Task<bool> SendBirthdayMessageToUser(ApplicationUser user)
        {
            var SMSSenderName = "CSF Nigeria";
            string message = "CSF Nigeria is saying Happy Birthday " + user.FullName + "." + " Another birthday means your life journey is incomplete, may your path be paved with success and guided by love. Best wishes Beloved";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_generalConfiguration.BuckSMSBaseURL);
                var url = "/v2/app/sms?" +
                    "email=" + _generalConfiguration.BuckSMSEmail +
                    "&password=" + _generalConfiguration.BuckSMSPassword +
                    "&message=" + message +
                    "&sender_name=" + SMSSenderName +
                    "&recipients=" + user.PhoneNumber +
                    "&forcednd= " + _generalConfiguration.BuckSMSforcednd;

                var response = await client.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                var otpRespons = JsonConvert.DeserializeObject<Data>(result);
                if (otpRespons.Status == "1")
                {
                    return true;
                }
                return false;
            }
        }
    }
}
