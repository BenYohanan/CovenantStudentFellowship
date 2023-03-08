using Core.Models;
using Core.ViewModels;

namespace Logic.IHelpers
{
    public interface ISuperAdminHelper
    {
        Task<ApplicationUser> UpdateSchoolAdminAndAddToRole(SchoolViewModel schoolViewModel, Guid schoolId);
        List<SchoolViewModel> AllRegisteredSchool();
        Task<ApplicationUser> AddSchool(SchoolViewModel schoolViewModel);
        bool EditSchool(SchoolViewModel schoolViewModel);
        bool DeleteSchoolById(Guid id);
        Task<List<ApplicationUserViewModel>> SystemAdmins();

        PictureViewModel HomePagePictures();
        List<HomePageImageViewModel> GetHomePagePictures();
        string AddHomePagePicture(int pictureType, string base64);
        bool EditHomePagePicture(HomePageImageViewModel homePagePictureViewModel, string imageUrl);
        bool DeletHomepagePictureById(int id);
        List<RoleViewModel> GetRoleList();
        int AllUserCount();
    }
}
