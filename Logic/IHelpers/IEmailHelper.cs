using Core.Models;

namespace Logic.IHelpers
{
    public interface IEmailHelper
    {
        Task<UserVerification> CreateUserToken(string userEmail);

        bool VerificationEmail(ApplicationUser applicationUser, string linkToClick);

        public bool Gratitude(ApplicationUser applicationUser);

        public bool PasswordResetLink(ApplicationUser applicationUser, string linkToClick);

        public bool PasswordResetConfirmation(ApplicationUser applicationUser);

        Task<bool> MarkTokenAsUsed(UserVerification userVerification);

        Task<UserVerification> GetUserToken(Guid token);

        bool PasswordChangeConfirmation(ApplicationUser applicationUser);

        bool SendSchoolAdminMessage(ApplicationUser applicationUser, string linkToClick, string schoolCodeName);

        bool NotificationOnBlogApprove(Blog blog);

        bool NotificationOnBlogReject(Blog blog);

        bool NotificationOnNewBlogUpdate(BlogSubscription SubscriberedUser);

        void HangFireFirstCronReminderJob();

        Task<bool> GetUserAndSendMessage();

        Task<bool> SendBirthdayMessageToUser(ApplicationUser user);
    }
}
