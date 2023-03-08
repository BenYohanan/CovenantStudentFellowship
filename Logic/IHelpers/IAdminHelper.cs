using Core.Models;
using Core.ViewModels;

namespace Logic.IHelpers
{
    public interface IAdminHelper
    {
        List<EventViewModel> GetAllEvent();

        string AddEvent(EventViewModel eventViewModel, Guid? schoolId);

        bool DeletEventById(int id);

        bool EditEvent(EventViewModel eventViewModel);

        List<GalleryViewModel> GetAllPicture();

        string AddPicture(GalleryViewModel galleryViewModel, string base64, Guid? schoolId);

        bool DeletPictureById(Guid id);

        string AddDocument(DocumentViewModel documentViewModel, string base64, Guid? schoolId);

        List<DocumentViewModel> GetAllDocument();

        bool DeleteDocumentById(Guid documentId);

        Task<JewishActivity> DisplayJewishActivities();

        string AddDues(DuesViewModel duesViewModel, Guid? schoolId);

        List<DuesViewModel> GetAllDues();

        bool EditDues(DuesViewModel duesViewModel);

        bool DeletDuesById(Guid id);

        List<ApplicationUserViewModel> GetFilteredUser(ApplicationUserViewModel searchedUser);

        List<BlogViewModel> GetFilteredBlog(BlogViewModel searchedBlog);
        List<ApplicationUserViewModel> GetAllUsers();
        bool DeactivatedUserById(string userId);
        bool ReactivateUserById(string userId);
        bool AddSemesterManualActivity(SemesterManualViewModel semesterManualViewModel);
        List<SemesterManualViewModel> SemesterManuals();
        List<EventViewModel> GetHomePageEvent();
        WeeklyActivityViewModel GetActivityForTheWeek();
    }
}
