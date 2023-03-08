using Core.Models;
using Core.ViewModels;

namespace Logic.IHelpers
{
    public interface IUserHelper
    {
        Task<ApplicationUser> FindByUserNameAsync(string username);
        ApplicationUser FindByUserName(string username);
        string GetCurrentUserId(string username);
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<ApplicationUser> FindByIdAsync(string id);
        ApplicationUser FindByEmail(string email);
        Task<ApplicationUser> CreateUsersAsync(ApplicationUserViewModel applicationUserViewModel);
        Task<bool> CheckIfUserIsAdmin(string username);
        string GetRoleLayout();
        string GetValidatedUrl();
        ApplicationUser FindByPhoneNumber(string phoneNumber);
        bool EditUserProfile(ApplicationUserViewModel applicationUserViewModel);
        string AddSubscriber(BlogSubscription blogUpdate);
        ApplicationUser CompleteRegistration(ApplicationUserViewModel applicationUserViewModel, ApplicationUser user, string profileUrl);
        SchoolViewModel SortSchoolByName(SchoolViewModel sortSchoolViewModel);
        List<SchoolViewModel> SortSchoolByState(SchoolViewModel sortSchoolViewModel);
        ApplicationUser FindByName(string name);
        ApplicationUserViewModel FindUserById(string userId);
    }
}
