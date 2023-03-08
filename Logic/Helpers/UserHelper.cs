using Core.DB;
using Core.Enums;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Logic.Helpers
{
    public class UserHelper : IUserHelper
    {
        private AppDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public UserHelper(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> FindByUserNameAsync(string username)
        {
            return await _userManager.Users.Where(s => s.UserName == username)?.FirstOrDefaultAsync();
        }

        public ApplicationUser FindByUserName(string username)
        {
            return _userManager.Users.Where(s => s.UserName == username).Include(x => x.School)?.FirstOrDefault();
        }

        public string GetCurrentUserId(string username)
        {
            return _userManager.Users.Where(s => s.UserName == username)?.FirstOrDefaultAsync().Result.Id?.ToString();
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.Users.Where(s => s.Email == email)?.Include(x => x.Gender).Include(s => s.Religion)
                    .Include(x => x.Nationality).Include(x => x.School).Include(x => x.State).Include(x => x.MyBlog).FirstOrDefaultAsync();
        }

        public async Task<ApplicationUser> FindByIdAsync(string id)
        {
            return await _userManager.Users.Where(s => s.Id == id).Include(x => x.Gender).Include(s => s.State)
                    .Include(x => x.Religion).Include(x => x.Nationality).Include(x => x.School).Include(x => x.MyBlog).FirstOrDefaultAsync();
        }
        public ApplicationUserViewModel FindUserById(string userId)
        {
            var user = _userManager.Users.Where(s => s.Id == userId).Include(x => x.Gender).Include(s => s.State)
                    .Include(x => x.Religion).Include(x => x.Nationality).Include(x => x.School).Include(x => x.MyBlog).FirstOrDefault();
            if (user != null)
            {
                var userDetails = new ApplicationUserViewModel()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    MiddleName = user?.MiddleName,
                    PhoneNumber = user.PhoneNumber,
                    Nationality = user?.Nationality?.Name,
                    Gender = user?.Gender?.Name,
                    HomeAddress = user?.HomeAddress,
                    SchoolAddress = user?.SchoolAddress,
                    State = user?.State?.Name,
                    School = user?.School?.SchoolName,
                    Religion = user?.Religion?.Name,
                    Department = user?.Department,
                    HomeSynagogue = user?.HomeSynagogue,
                    Email = user.Email,
                    DateOfBirth = user.DateOfBirth.ToString("D"),
                    BlogId = user.BlogId,
                    ProfilePicture = user?.ProfilePicture,
                    ContactName = user?.ContactName,
                    ContactPhoneNumber = user?.ContactPhoneNumber,
                    ContactRelationship = user?.ContactRelationship,
                    FullName = user?.FullName,
                    GenderId = user.GenderId,
                    NationalityId = user.NationalityId,
                    StateId = user.StateId,
                    SchoolId = user.SchoolId,
                };
                return userDetails;
            }
            return null;
        }
        public ApplicationUser FindByEmail(string email)
        {
            var user = _userManager.Users.Where(s => s.Email == email)?.Include(x => x.Gender).Include(s => s.Religion)
                    .Include(x => x.Nationality).Include(x => x.School).Include(x => x.State).Include(x => x.MyBlog).FirstOrDefault();
            if (user != null)
            {
                return user;
            }
            return null;
        }

        public async Task<ApplicationUser> CreateUsersAsync(ApplicationUserViewModel applicationUserViewModel)
        {
            if (applicationUserViewModel.Email != null && applicationUserViewModel.FirstName != null)
            {
                var applicationUser = new ApplicationUser()
                {
                    UserName = applicationUserViewModel.Email,
                    FirstName = applicationUserViewModel.FirstName,
                    LastName = applicationUserViewModel.LastName,
                    MiddleName = applicationUserViewModel.MiddleName,
                    Email = applicationUserViewModel.Email,
                    DateOfBirth = DateTime.Parse(applicationUserViewModel.DateOfBirth),
                    Deactivated = false,
                    PhoneNumber = applicationUserViewModel.PhoneNumber,
                    GenderId = applicationUserViewModel.GenderId,
                    SchoolId = applicationUserViewModel.SchoolId,
                    VerificationStatus = VerificationStatus.InComplete,
                    IsSchoolAdmin = false,
                };
                applicationUser.DateRegistered = DateTime.Now;
                var result = await _userManager.CreateAsync(applicationUser, applicationUserViewModel.Password).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return applicationUser;
                }

            }
            return null;
        }

        public async Task<bool> CheckIfUserIsAdmin(string username)
        {
            if (username != null)
            {
                var currentUser = FindByUserNameAsync(username);
                var userDetails = await _userManager.Users.Where(s => s.UserName == currentUser.Result.UserName)?.FirstOrDefaultAsync();
                if (userDetails != null)
                {
                    var goAdmin = await _userManager.IsInRoleAsync(userDetails, "Admin");
                    if (goAdmin)
                    {
                        return goAdmin;
                    }
                }
            }
            return false;
        }

        public string GetRoleLayout()
        {
            var loggedInUser = Session.GetCurrentUser();
            if (loggedInUser != null)
            {
                var superAdmin = loggedInUser.Roles.Contains(Session.Constants.SuperAdminRole);
                if (superAdmin)
                {
                    return Session.Constants.SuperAdminLayout;
                }
                else
                {
                    var isSchoolAdmin = loggedInUser.Roles.Contains(Session.Constants.SchoolAdminRole);
                    if (isSchoolAdmin)
                    {
                        return Session.Constants.SchoolAdminLayout;
                    }
                    else
                    {
                        var isUser = loggedInUser.Roles.Contains(Session.Constants.UserRole);
                        if (isUser)
                        {
                            return Session.Constants.UserLayout;
                        }
                    }
                }
            }
            return Session.Constants.UserLayout;
        }
        public string GetValidatedUrl()
        {
            var loggedInUser = Session.GetCurrentUser();
            var superAdmin = loggedInUser.Roles.Contains(Session.Constants.SuperAdminRole);
            if (superAdmin)
            {
                return "/SuperAdmin/Index";
            }
            var isSchoolAdmin = loggedInUser.Roles.Contains(Session.Constants.SchoolAdminRole);
            if (isSchoolAdmin)
            {
                return "/Admin/Index";
            }
            var isCompanyManager = loggedInUser.Roles.Contains(Session.Constants.UserRole);
            if (isCompanyManager)
            {
                return "/User/Index";
            }
            return "/Home/Index";
        }

        public ApplicationUser FindByPhoneNumber(string phoneNumber)
        {
            var user = _userManager.Users.Where(p => p.PhoneNumber == phoneNumber).Include(x => x.Gender).Include(x => x.Religion)
                    .Include(x => x.School).Include(x => x.Nationality).Include(x => x.State).Include(x => x.MyBlog).FirstOrDefault();
            if (user != null)
            {
                return user;
            }
            return null;
        }

        public bool EditUserProfile(ApplicationUserViewModel applicationUserViewModel)
        {
            if (applicationUserViewModel != null)
            {
                var user = _context.ApplicationUser.Where(c => c.Id == applicationUserViewModel.Id && !c.Deactivated).FirstOrDefault();
                if (user != null)
                {
                    user.Id = applicationUserViewModel.Id;
                    user.FirstName = applicationUserViewModel.FirstName;
                    user.LastName = applicationUserViewModel.LastName;
                    user.SchoolAddress = applicationUserViewModel.SchoolAddress;
                    user.Email = applicationUserViewModel.Email;
                    user.DateOfBirth = DateTime.Parse(applicationUserViewModel.DateOfBirth);
                    user.Deactivated = false;
                    user.Nationality.Name = applicationUserViewModel.Nationality;
                    user.State.Name = applicationUserViewModel.State;
                    user.PhoneNumber = applicationUserViewModel.PhoneNumber;
                    user.MiddleName = applicationUserViewModel.MiddleName;
                    user.Religion.Name = applicationUserViewModel.Religion;
                    user.Gender.Name = applicationUserViewModel.Gender;
                    user.School.SchoolName = applicationUserViewModel.School;
                    user.HomeSynagogue = applicationUserViewModel.HomeSynagogue;
                    user.DateRegistered = DateTime.Now;
                    user.Department = applicationUserViewModel.Department;
                    user.ContactName = applicationUserViewModel?.ContactName;
                    user.ContactPhoneNumber = applicationUserViewModel?.ContactPhoneNumber;
                    user.ContactRelationship = applicationUserViewModel?.ContactRelationship;

                    _context.Update(user);
                    _context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public string AddSubscriber(BlogSubscription blogUpdate)
        {
            if (blogUpdate.Email != null)
            {
                var newUser = new BlogSubscription
                {
                    Email = blogUpdate.Email,
                    DateAdded = DateTime.Now,
                    Active = true,
                    Deleted = false,
                };
                _context.BlogSubscription.Add(newUser);
                _context.SaveChanges();
                return "User Subscribed Successfully";
            }
            return null;
        }

        public ApplicationUser CompleteRegistration(ApplicationUserViewModel applicationUserViewModel, ApplicationUser user, string profileUrl)
        {
            if (applicationUserViewModel != null && user.Id != null)
            {
                user.StateId = applicationUserViewModel.StateId;
                user.ReligionId = applicationUserViewModel.ReligionId;
                user.SchoolId = applicationUserViewModel.SchoolId;
                user.VerificationStatus = VerificationStatus.Completed;
                user.SchoolAddress = applicationUserViewModel.SchoolAddress;
                user.ContactName = applicationUserViewModel.ContactName;
                user.ContactPhoneNumber = applicationUserViewModel.ContactPhoneNumber;
                user.ContactRelationship = applicationUserViewModel.ContactRelationship;
                user.Department = applicationUserViewModel.Department;
                user.HomeSynagogue = applicationUserViewModel.HomeSynagogue;
                user.HomeAddress = applicationUserViewModel.HomeAddress;
                user.NationalityId = applicationUserViewModel.NationalityId;
                user.ProfilePicture = profileUrl;
                _context.Update(user);
                 _context.SaveChanges();
                return user;
            }
            return null;
        }

        public List<SchoolViewModel> SortSchoolByState(SchoolViewModel sortSchoolViewModel)
        {

            if (sortSchoolViewModel.StateId != null)
            {
                var searchedByState = _context.School.Where(com => com.StateId == sortSchoolViewModel.StateId && com.Active && !com.Deleted).ToList();
                var sortedData = new List<SchoolViewModel>();
                foreach (var item in searchedByState)
                {
                    var detail = new SchoolViewModel()
                    {
                        Id = item.Id,
                        SchoolName = item.SchoolName,
                        Address = item.Address,
                        StateId = item.StateId,
                        DateAdded = item.DateAdded,
                        SchoolCodeName = item.SchoolCodeName,
                        Active = true,
                        Deleted = false,
                    };
                    sortedData.Add(detail);
                }
                return sortedData;
            }
            return null;
        }

        public SchoolViewModel SortSchoolByName(SchoolViewModel sortSchoolViewModel)
        {
            if (sortSchoolViewModel.SchoolName != "")
            {
                var searchedByName = _context.School.Where(com => com.SchoolName.ToLower().Contains(sortSchoolViewModel.SchoolName.ToLower()) && com.Active && !com.Deleted).Include(x => x.State).FirstOrDefault();
                var sortedData = new SchoolViewModel();

                var detail = new SchoolViewModel()
                {
                    Id = searchedByName.Id,
                    SchoolName = searchedByName.SchoolName,
                    Address = searchedByName.Address,
                    StateId = searchedByName.StateId,
                    DateAdded = searchedByName.DateAdded,
                    SchoolCodeName = searchedByName.SchoolCodeName,
                    Active = true,
                    Deleted = false,
                };
                sortedData = detail;
                return sortedData;
            }
            return null;
        }
        public ApplicationUser FindByName(string name)
        {
            if (name != null)
            {
                var names = name.Split(' ');
                var user = _userManager.Users.Where(s => s.FirstName == names[0] && (s.MiddleName == names[1] || s.LastName == names[2]) && !s.Deactivated).FirstOrDefault();
                if (user != null)
                {
                    return user;
                }
            }
            return null;
        }


        public async Task<string> GetUserLayout(string userId)
        {
            if (userId != null)
            {
                var returnUrl = "";
                var user =await FindByIdAsync(userId).ConfigureAwait(false);
                var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                foreach (var userRole in roles)
                {
                    switch (userRole)
                    {
                        case "SuperAdmin":
                            returnUrl = "/SuperAdmin/Index";
                            break;
                        case "Admin":
                            returnUrl = "/Admin/Index";
                            break;
                        default:
                            break;
                    }
                }
                return returnUrl;   
            }
            return null;
        }
    }
}
