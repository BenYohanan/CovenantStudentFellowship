using Core.DB;
using Core.Enums;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Logic.Helpers
{
    public class SuperAdminHelper : ISuperAdminHelper
    {
        private AppDbContext _context;
        private IUserHelper _userHelper;
        private IEmailHelper _emailHelper;
        private UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SuperAdminHelper(AppDbContext context,
            UserManager<ApplicationUser> userManager, IUserHelper userHelper, IEmailHelper emailHelper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _userHelper = userHelper;
            _userHelper = userHelper;
            _emailHelper = emailHelper;
            _webHostEnvironment= webHostEnvironment;
        }

        public async Task<ApplicationUser> UpdateSchoolAdminAndAddToRole(SchoolViewModel schoolViewModel, Guid schoolId)
        {
            if (schoolViewModel != null)
            {
                var applicationUser = new ApplicationUser()
                {
                    UserName = schoolViewModel.Email,
                    FirstName = schoolViewModel.FirstName,
                    LastName = schoolViewModel.LastName,
                    MiddleName = schoolViewModel.MiddleName,
                    Email = schoolViewModel.Email,
                    DateOfBirth = DateTime.Parse(schoolViewModel.DateOfBirth),
                    Deactivated = false,
                    PhoneNumber = schoolViewModel.SchoolAdinPhoneNumber,
                    GenderId = schoolViewModel.GenderId,
                    SchoolId = schoolId,
                    VerificationStatus = VerificationStatus.InComplete,
                    IsSchoolAdmin = true,
                    DateRegistered = DateTime.Now,
                };
                applicationUser.DateRegistered = DateTime.Now;
                var result = await _userManager.CreateAsync(applicationUser, "123456").ConfigureAwait(false);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(applicationUser, "SchoolAdmin").ConfigureAwait(false);
                    return applicationUser;
                }
            }
            return null;
        }
        public List<SchoolViewModel> AllRegisteredSchool()
        {
            var schoolViewModel = new List<SchoolViewModel>();
            var allSchols = _context.School.Where(x => x.Id != Guid.Empty && x.Active && !x.Deleted)
                .Include(x => x.State).ToList();
            foreach (var school in allSchols)
            {
                var schoolAdmin = GetSchoolAdminBySchoolId(school.Id);
                var schoolDetails = new SchoolViewModel()
                {
                    Id = school.Id,
                    SchoolCodeName = school.SchoolCodeName,
                    Address = school.Address,
                    SchoolName = school.SchoolName,
                    State = school.State?.Name,
                    AdminDepartment = schoolAdmin?.Department,
                    AdminName = schoolAdmin?.FullName,
                    PhoneNumber = schoolAdmin?.PhoneNumber,
                    ProfilePicture = schoolAdmin?.ProfilePicture,

                };
                schoolViewModel.Add(schoolDetails);
            }
            return schoolViewModel;
        }
        public async Task<ApplicationUser> AddSchool(SchoolViewModel schoolViewModel)
        {
            if (schoolViewModel != null)
            {
                    var schoolModel = new School
                    {
                        SchoolCodeName = schoolViewModel.SchoolCodeName,
                        Address = schoolViewModel.Address,
                        SchoolName = schoolViewModel.SchoolName,
                        StateId = schoolViewModel.StateId,
                        Active = true,
                        Deleted = false,
                        DateAdded = DateTime.Now,
                    };
                    _context.School.Add(schoolModel);
                    _context.SaveChanges();
                    var schoolAdmin =await UpdateSchoolAdminAndAddToRole(schoolViewModel, schoolModel.Id).ConfigureAwait(false);
                    if (schoolAdmin != null)
                    {
                        return schoolAdmin;
                    }
                
            }
            return null;
        }
        public bool EditSchool(SchoolViewModel schoolViewModel)
        {
            if (schoolViewModel != null)
            {
                var getUserByFullName = _userHelper.FindByName(schoolViewModel.AdminName);
                if (getUserByFullName != null)
                {
                    var school = _context.School.Where(c => c.Id == schoolViewModel.Id && !c.Deleted && c.Active).FirstOrDefault();
                    if (school != null)
                    {
                        school.Id = schoolViewModel.Id;
                        school.SchoolCodeName = schoolViewModel.SchoolCodeName;
                        school.SchoolName = schoolViewModel.SchoolName;
                        school.StateId = schoolViewModel.StateId;
                        school.Address = schoolViewModel.Address;
                        school.Deleted = false;
                        school.Active = true;


                        _context.Update(school);
                        _context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }
        public bool DeleteSchoolById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return false;
            }
            var school = _context.School.Where(d => d.Id == id && d.Active && !d.Deleted).FirstOrDefault();
            if (school != null)
            {
                school.Deleted = true;
                _context.School.Update(school);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public async Task<List<ApplicationUserViewModel>> SystemAdmins()
        {
            var users = new List<ApplicationUserViewModel>();
            var systemAdmin = await _userManager.GetUsersInRoleAsync("SchoolAdmin").ConfigureAwait(false);
            var admins = systemAdmin?.Select(x => new ApplicationUserViewModel
            {
                Id = x.Id,
                ProfilePicture = x.ProfilePicture,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                FullName = x.FullName,
            }).ToList();
            if (systemAdmin.Any())
            {
                return admins;
            }
            return users;
        }

        public PictureViewModel HomePagePictures()
        {
            var pictures = new PictureViewModel();
            var homepagePicture = _context.HomePageImages.Where(u => u.Id != 0 && u.ImageUrl != null && u.Active && !u.Deleted && u.HomePageImageId != null).Include(u => u.HomePagePicture).ToList();
            if (homepagePicture != null && homepagePicture.Count > 0)
            {
                var aboutPictureUrl = homepagePicture.Where(u => u.Id != 0 && u.ImageUrl != null && u.HomePagePicture.Name.ToLower().Contains("about")).FirstOrDefault();
                if (aboutPictureUrl != null)
                {
                    pictures.AboutUrl = aboutPictureUrl.ImageUrl;
                }
                var mainPictureUrl = homepagePicture.Where(u => u.Id != 0 && u.ImageUrl != null && u.HomePagePicture.Name.ToLower().Contains("main")).FirstOrDefault();
                if (mainPictureUrl != null)
                {
                    pictures.MainUrl = mainPictureUrl.ImageUrl;
                }
                var eventPictureUrl = homepagePicture.Where(u => u.Id != 0 && u.ImageUrl != null && u.HomePagePicture.Name.ToLower().Contains("event")).FirstOrDefault();
                if (eventPictureUrl != null)
                {
                    pictures.EventUrl = eventPictureUrl.ImageUrl;
                }
                return pictures;
            }
            return pictures;
        }
        public List<HomePageImageViewModel> GetHomePagePictures()
        {
            var homepagePictures = new List<HomePageImageViewModel>();
            var allPictures = _context.HomePageImages.Where(x => x.Id != 0 && x.Active && !x.Deleted && x.HomePageImageId != null)
                .Include(x => x.HomePagePicture).Include(x => x.School)
                .Select(x => new HomePageImageViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImageUrl = x.ImageUrl,
                    PictureName = x.HomePagePicture.Name,
                    DateAdded = x.DateAdded.ToString("D"),
                    SchoolName = x.School.SchoolCodeName,
                }).ToList();
            if (allPictures != null && allPictures.Count > 0)
            {
                return allPictures;
            }
            return homepagePictures;
        }
        public string AddHomePagePicture(int pictureType, string base64)
        {
            var loggedInUser = Session.GetCurrentUser();
            var picture = new HomePageImage
            {
                HomePageImageId = pictureType,
                DateAdded = DateTime.Now,
                ImageUrl = base64,
                Active = true,
                Deleted = false,
                SchoolId = loggedInUser.SchoolId,
            };
            _context.HomePageImages.Add(picture);
            _context.SaveChanges();
            return "Picture Uploaded Successfully";
        }
        public bool EditHomePagePicture(HomePageImageViewModel homePagePictureViewModel, string imageUrl)
        {
            if (homePagePictureViewModel != null)
            {
                var picture = _context.HomePageImages.Where(c => c.Id == homePagePictureViewModel.Id && !c.Deleted && c.Active && c.ImageUrl != null && c.HomePageImageId != null).Include(c => c.HomePagePicture).FirstOrDefault();
                if (picture != null)
                {
                    picture.ImageUrl = imageUrl;
                    _context.Update(picture);
                    _context.SaveChanges();
                    return true;
                }
            }
            return false;
        }
        public bool DeletHomepagePictureById(int id)
        {
            if (id == 0)
            {
                return false;
            }
            var homepagePicture = _context.HomePageImages.Where(d => d.Id == id && d.Active && !d.Deleted).FirstOrDefault();
            if (homepagePicture != null)
            {
                homepagePicture.Deleted = true;
                _context.HomePageImages.Remove(homepagePicture);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public List<RoleViewModel> GetRoleList()
        {
            var roleViewModel = new List<RoleViewModel>();
            var allroles = _context.UserRoles.Where(x => x.UserId != null && x.RoleId != null).ToList();
            if (allroles.Count > 0)
            {
                foreach (var role in allroles)
                {
                    var user =_userHelper.FindUserById(role.UserId);
                    var userRole = _context.Roles.Where(r => r.Id == role.RoleId).FirstOrDefault();
                    if (user != null && userRole != null)
                    {
                        var roleModel = new RoleViewModel
                        {
                            Role = userRole,
                            User = user,
                        };
                        roleViewModel.Add(roleModel);
                    }
                    else
                    {
                        continue;
                    }
                }
                return roleViewModel;
            }
            return roleViewModel;
        }
        public ApplicationUser GetSchoolAdminBySchoolId(Guid schoolId)
        {
            if (schoolId != Guid.Empty)
            {
                var admin = _context.ApplicationUser.Where(x => x.Id != null && x.SchoolId == schoolId && !x.Deactivated && x.UserName != null && x.IsSchoolAdmin).FirstOrDefault();
                if (admin != null)
                {
                    return admin;
                }
            }
            return null;
        }

        public int AllUserCount()
        {
            var allUsers = _context.ApplicationUser.Where(x => x.Id != null && !x.Deactivated).ToList().Count();
            {
                return allUsers;
            }

        }
    }
}
