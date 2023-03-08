using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Logic.Helpers
{
    public class AdminHelper : IAdminHelper
    {
        private AppDbContext _context;
        private IGeneralConfiguration _generalConfiguration;
        private IUserHelper _userHelper;

        public AdminHelper(AppDbContext context, IUserHelper userHelper, IGeneralConfiguration generalConfiguration)
        {
            _context = context;
            _userHelper = userHelper;
            _generalConfiguration = generalConfiguration;
        }

        public List<EventViewModel> GetAllEvent()
        {
            var events = new List<EventViewModel>();
            var loggedInUser = Session.GetCurrentUser();
            if (loggedInUser != null)
            {
                var allEvent = _context.Events.Where(x => x.Id != 0 && x.Active && !x.Deleted && x.SchoolId == loggedInUser.SchoolId).Select(x => new EventViewModel
                {
                    Id = x.Id,
                    Note = x.Note,
                    Title = x.Title,
                    Date = x.Date.ToString("D"),
                    DateAdded = x.DateAdded.ToString("D")
                }).ToList();
                if (allEvent.Count > 0)
                {
                    return allEvent;
                }
            }
            return events;
        }

        public List<EventViewModel> GetHomePageEvent()
        {
            var events = new List<EventViewModel>();
            var allEvent = _context.Events.Where(x => x.Id != 0 && x.Active && !x.Deleted && x.SchoolId != null).Include(x => x.School).Select(x => new EventViewModel
            {
                Id = x.Id,
                Note = x.Note,
                Title = x.Title,
                Date = x.Date.ToString("D"),
                DateAdded = x.DateAdded.ToString("D"),
                SchoolName = x.School.SchoolCodeName
            }).ToList();
            if (allEvent.Count > 0)
            {
                return allEvent;
            }
            return events;
        }


        public string AddEvent(EventViewModel eventViewModel, Guid? schoolId)
        {
            var newEvent = new Event
            {
                Title = eventViewModel.Title,
                Note = eventViewModel.Note,
                Date = DateTime.Parse(eventViewModel.Date),
                DateAdded = DateTime.Now,
                SchoolId = schoolId,
                Active = true,
                Deleted = false,
            };
            _context.Events.Add(newEvent);
            _context.SaveChanges();
            return "Event Created Successfully";
        }

        public bool EditEvent(EventViewModel eventViewModel)
        {
            if (eventViewModel != null)
            {
                var myEvent = _context.Events.Where(c => c.Id == eventViewModel.Id && !c.Deleted && c.Active).FirstOrDefault();
                if (myEvent != null)
                {
                    myEvent.Id = eventViewModel.Id;
                    myEvent.Title = eventViewModel.Title;
                    myEvent.Note = eventViewModel.Note;
                    myEvent.Deleted = false;
                    myEvent.Active = true;
                    myEvent.Date = DateTime.Parse(eventViewModel.Date);
                    _context.Update(myEvent);
                    _context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool DeletEventById(int id)
        {
            if (id == 0)
            {
                return false;
            }
            var myEvent = _context.Events.Where(d => d.Id == id && d.Active && !d.Deleted).FirstOrDefault();
            if (myEvent != null)
            {
                myEvent.Deleted = true;
                _context.Events.Update(myEvent);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<GalleryViewModel> GetAllPicture()
        {
            var pictures = new List<GalleryViewModel>();
            var allPicture = _context.Galleries.Where(x => x.Id != Guid.Empty && x.Active && !x.Deleted && x.SchoolId != Guid.Empty).Include(x => x.School)
                .Select(x => new GalleryViewModel
                {
                    Id = x.Id,
                    Note = x.Note,
                    Title = x.Title,
                    DateAdded = x.DateAdded.ToString("D"),
                    ImageUrl = x.ImageUrl,
                    SchoolName = x.School.SchoolCodeName
                }).ToList();
            if (allPicture.Count > 0)
            {
                return allPicture;
            }

            return pictures;
        }

        public string AddPicture(GalleryViewModel galleryViewModel, string base64, Guid? schoolId)
        {
            if (galleryViewModel != null && base64 != null)
            {
                var gallery = new Gallery
                {
                    Title = galleryViewModel.Title,
                    Note = galleryViewModel.Note,
                    DateAdded = DateTime.Now,
                    SchoolId = schoolId,
                    ImageUrl = base64,
                    Active = true,
                    Deleted = false,
                };
                _context.Galleries.Add(gallery);
                _context.SaveChanges();
                return "Picture Uploaded Successfully";
            }
            return null;
        }

        public bool DeletPictureById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return false;
            }
            var photo = _context.Galleries.Where(d => d.Id == id && d.Active && !d.Deleted).FirstOrDefault();
            if (photo != null)
            {
                photo.Deleted = true;
                _context.Galleries.Remove(photo);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public string AddDocument(DocumentViewModel documentViewModel, string base64, Guid? schoolId)
        {
            if (documentViewModel != null && base64 != null)
            {
                var document = new Document
                {
                    DocumentList = documentViewModel.DocumentList,
                    Name = documentViewModel.Name,
                    DateAdded = DateTime.Now,
                    SchoolId = schoolId,
                    FileUrl = base64,
                    Active = true,
                    Deleted = false,
                };
                _context.Document.Add(document);
                _context.SaveChanges();
                return "Picture Uploaded Successfully";
            }
            return null;
        }

        public List<DocumentViewModel> GetAllDocument()
        {
            var documents = new List<DocumentViewModel>();
            var loggedInUser = Session.GetCurrentUser();
            var alldocument = _context.Document.Where(x => x.Id != Guid.Empty && x.Active && !x.Deleted && x.FileUrl != null && x.Name != null && x.SchoolId == loggedInUser.SchoolId).Select(x => new DocumentViewModel
            {
                Id = x.Id,
                DocumentList = x.DocumentList,
                FileUrl = x.FileUrl,
                Name = x.Name,
                DateAdded = x.DateAdded.ToString("D")
            }).ToList();
            if (alldocument.Count > 0)
            {
                return alldocument;
            }
            return documents;
        }

        public bool DeleteDocumentById(Guid documentId)
        {
            if (documentId != Guid.Empty)
            {
                var document = _context.Document.Where(d => d.Id == documentId && d.FileUrl != null).FirstOrDefault();
                if (document != null)
                {
                    document.Active = false;
                    document.Deleted = true;
                    _context.Document.Update(document);
                    _context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public async Task<JewishActivity> DisplayJewishActivities()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_generalConfiguration.HebrewCalApiUrl);

                var url = "/hebcal?" +
                    "v=" + _generalConfiguration.Version +
                    "&cfg=" + _generalConfiguration.JsonOutput +
                    "&maj=" + _generalConfiguration.Major +
                    "&min=" + _generalConfiguration.Minor +
                    "&mod=" + _generalConfiguration.MordenHoliday +
                    "&nx=" + _generalConfiguration.RoshChodesh +
                    "&year=" + _generalConfiguration.Year +
                    "&month=" + DateTime.Now.Month +
                    "&ss=" + _generalConfiguration.SpecialShabbath +
                    "&mf=" + _generalConfiguration.MinorFast +
                    "&c=" + _generalConfiguration.CandleLightingTimes +
                    "&geo=" + _generalConfiguration.Geo +
                    "&geonameid=" + _generalConfiguration.Geonameid +
                    "&M=" + _generalConfiguration.Havdalah +
                    "&s=" + _generalConfiguration.ParashatHaShavuah;

                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode == true)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var displayResponses = JsonConvert.DeserializeObject<JewishActivity>(result);
                    displayResponses.Items = displayResponses.Items.Where(d => d.Date.Date == DateTime.Now.Date).ToList();
                    return displayResponses;
                }
                return null;
            }
        }

        public string AddDues(DuesViewModel duesViewModel, Guid? schoolId)
        {
            if (duesViewModel != null)
            {
                var newDues = new Dues
                {
                    Name = duesViewModel.Name,
                    Amount = duesViewModel.Amount,
                    DateAdded = DateTime.Now,
                    SchoolId = schoolId,
                    Active = true,
                    Deleted = false,
                };
                _context.Dues.Add(newDues);
                _context.SaveChanges();
                return "Dues Added Successfully";
            }
            return null;
        }

        public List<DuesViewModel> GetAllDues()
        {
            var dues = new List<DuesViewModel>();
            var loggedInUser = Session.GetCurrentUser();
            var allDues = _context.Dues.Where(x => x.Id != Guid.Empty && x.Active && !x.Deleted && x.SchoolId == loggedInUser.SchoolId).Include(x => x.School).
                Select(x => new DuesViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Amount = x.Amount,
                    DateAdded = x.DateAdded.ToString("D")
                }).ToList();
            if (allDues.Count > 0)
            {
                return allDues;
            }
            return dues;
        }

        public bool DeletDuesById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return false;
            }
            var dues = _context.Dues.Where(d => d.Id == id && d.Active && !d.Deleted).FirstOrDefault();
            if (dues != null)
            {
                dues.Deleted = true;
                _context.Dues.Update(dues);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool EditDues(DuesViewModel duesViewModel)
        {
            if (duesViewModel != null)
            {
                var dues = _context.Dues.Where(c => c.Id == duesViewModel.Id && !c.Deleted && c.Active).FirstOrDefault();
                if (dues != null)
                {
                    dues.Id = duesViewModel.Id;
                    dues.Name = duesViewModel.Name;
                    dues.Amount = duesViewModel.Amount;
                    dues.Deleted = false;
                    dues.Active = true;

                    _context.Dues.Update(dues);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            return false;
        }

        public List<ApplicationUserViewModel> GetFilteredUser(ApplicationUserViewModel searchedUser)
        {
            if (searchedUser.FirstName != "")
            {
                var searchedByName = _context.ApplicationUser.Where(com => com.FirstName.ToLower().Contains(searchedUser.FirstName.ToLower()) && !com.Deactivated && com.UserName != null).ToList();
                var sortedData = new List<ApplicationUserViewModel>();
                foreach (var user in searchedByName)
                {
                    var applicationUser = new ApplicationUserViewModel()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        MiddleName = user.MiddleName,
                        SchoolAddress = user.SchoolAddress,
                        Email = user.Email,
                        DateOfBirth = user.DateOfBirth.ToString("D"),
                        Deactivated = false,
                        HomeAddress = user.HomeAddress,
                        NationalityId = user.NationalityId,
                        StateId = user.StateId,
                        PhoneNumber = user.PhoneNumber,
                        ReligionId = user.ReligionId,
                        GenderId = user.GenderId,
                        HomeSynagogue = user.HomeSynagogue,
                        Department = user.Department,
                        ProfilePicture = user.ProfilePicture,
                        ContactName = user.ContactName,
                        ContactPhoneNumber = user.ContactPhoneNumber,
                        ContactRelationship = user.ContactRelationship
                    };
                    sortedData.Add(applicationUser);
                }
                return sortedData;
            }
            return null;
        }

        public List<BlogViewModel> GetFilteredBlog(BlogViewModel searchedBlog)
        {
            if (searchedBlog.Title != "")
            {
                var searchedByName = _context.Blog.Where(com => com.Title.ToLower().Contains(searchedBlog.Title.ToLower()) && com.Active && !com.Deleted).Include(x => x.BlogCategory).ToList();
                var sortedData = new List<BlogViewModel>();
                foreach (var item in searchedByName)
                {
                    var detail = new BlogViewModel()
                    {
                        Title = item.Title,
                        BlogContent = item.BlogContent,
                        BlogStatus = item.BlogStatus.ToString(),
                        BlogCategoryId = item.BlogCategoryId,
                        BlogCategoryName = item.BlogCategory.Name,
                        Id = item.Id,
                        ImageUrl = item.ImageUrl,
                        BlogAutorName = item.AddedBy.FirstName,
                        AboutMe = item.AboutMe,
                        AddedById = item.AddedById
                    };
                    sortedData.Add(detail);
                }
                return sortedData;
            }


            if (searchedBlog.BlogCategoryId != null)
            {
                var searchedByCategory = _context.Blog.Where(com => com.BlogCategoryId == searchedBlog.BlogCategoryId && com.Active && !com.Deleted).ToList();
                var sortedData = new List<BlogViewModel>();
                foreach (var item in searchedByCategory)
                {
                    var detail = new BlogViewModel()
                    {
                        Title = item.Title,
                        BlogContent = item.BlogContent,
                        BlogStatus = item.BlogStatus.ToString(),
                        BlogCategoryId = item.BlogCategoryId,
                        BlogCategoryName = item.BlogCategory.Name,
                        Id = item.Id,
                        ImageUrl = item.ImageUrl,
                        BlogAutorName = item.AddedBy.FullName,
                        AboutMe = item.AboutMe,
                        AddedById = item.AddedById
                    };
                    sortedData.Add(detail);
                }
                return sortedData;
            }
            return null;
        }
        public List<ApplicationUserViewModel> GetAllUsers()
        {
            var loggedInUser = Session.GetCurrentUser();
            var applicationViewModel = new List<ApplicationUserViewModel>();
            var allUser = _context.ApplicationUser.Where(x => x.Id != null && x.SchoolId == loggedInUser.SchoolId && x.ReligionId != null && x.NationalityId != null && x.StateId != null && x.GenderId != null)
                .Include(x => x.Gender).Include(s => s.State).Include(x => x.School)
                    .Include(x => x.Religion).Include(x => x.Nationality)
                    .Select(x => new ApplicationUserViewModel
                    {
                        Id = x.Id,
                        FullName = x.FullName,
                        SchoolAddress = x.SchoolAddress,
                        Email = x.Email,
                        DateOfBirth = x.DateOfBirth.ToString("D"),
                        HomeAddress = x.HomeAddress,
                        Nationality = x.Nationality.Name,
                        State = x.State.Name,
                        PhoneNumber = x.PhoneNumber,
                        Religion = x.Religion.Name,
                        Gender = x.Gender.Name,
                        School = x.School.SchoolCodeName,
                        HomeSynagogue = x.HomeSynagogue,
                        Department = x.Department,
                        ProfilePicture = x.ProfilePicture,
                        ContactName = x.ContactName,
                        ContactPhoneNumber = x.ContactPhoneNumber,
                        ContactRelationship = x.ContactRelationship,
                        Deactivated = x.Deactivated
                    }).ToList();
            if (allUser.Count > 0 && allUser != null)
            {
                return allUser;
            }
            return applicationViewModel;
        }
        public bool DeactivatedUserById(string userId)
        {
            if (userId != null)
            {
                var user = _context.ApplicationUser.Where(d => d.Id == userId && !d.Deactivated).FirstOrDefault();
                if (user != null)
                {
                    user.Deactivated = true;
                    _context.ApplicationUser.Update(user);
                    _context.SaveChanges();
                    return true;
                }
            }
            return false;
        }
        public bool ReactivateUserById(string userId)
        {
            if (userId != null)
            {
                var user = _context.ApplicationUser.Where(d => d.Id == userId && d.Deactivated).FirstOrDefault();
                if (user != null)
                {
                    user.Deactivated = false;
                    _context.ApplicationUser.Update(user);
                    _context.SaveChanges();
                    return true;
                }
            }
            return false;
        }
        public List<SemesterManualViewModel> SemesterManuals()
        {
            var loggedInUser = Session.GetCurrentUser();
            var semesterManualList = new List<SemesterManualViewModel>();
            var semesterManuals = _context.SemesterManual.Where(x => x.Id != 0 && !x.Deleted && x.SchoolId == loggedInUser.SchoolId)
                .Include(x => x.Cordinator).Include(x => x.Speaker).OrderByDescending(x => x.Date)
                .Select(x => new SemesterManualViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    BibleVerse = x.BibleVerse,
                    Date = x.Date.ToString("D"),
                    SchoolId = x.SchoolId,
                    SpeakerId = x.SpeakerId,
                    SpeakerName = x.Speaker.FullName,
                    SpeakerProfilePicture = x.Speaker.ProfilePicture,
                    CordinatorId = x.CordinatorId,
                    CordinatorName = x.Cordinator.FullName,
                    CordinatorProfilePicture = x.Cordinator.ProfilePicture,
                }).ToList();
            if (semesterManuals != null && semesterManuals.Count > 0)
            {
                return semesterManuals;
            }
            return semesterManualList;
        }
        public bool AddSemesterManualActivity(SemesterManualViewModel semesterManualViewModel)
        {
            var loggedInUser = Session.GetCurrentUser();
            if (loggedInUser != null)
            {
                var manaul = new SemesterManual
                {
                    Name = semesterManualViewModel.Name,
                    BibleVerse = semesterManualViewModel.BibleVerse,
                    Date = DateTime.Parse(semesterManualViewModel.Date),
                    Active = true,
                    Deleted = false,
                    CordinatorId = semesterManualViewModel.CordinatorId,
                    SpeakerId = semesterManualViewModel.SpeakerId,
                    DateAdded = DateTime.Now,
                    SchoolId = loggedInUser.SchoolId,
                };
                _context.SemesterManual.Add(manaul);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public WeeklyActivityViewModel GetActivityForTheWeek()
        {
            var loggedInUser = Session.GetCurrentUser();
            var weeklyActivity = new WeeklyActivityViewModel();
            DateTime today = DateTime.Today;
            int currentDayOfWeek = (int)today.DayOfWeek;
            DateTime monday = today.AddDays(-currentDayOfWeek + 1);
            var dateRange = Enumerable.Range(0, 7).Select(days => monday.AddDays(days)).ToList();
            var activities = _context.SemesterManual.Where(x => x.Id != 0 && x.Active && loggedInUser.SchoolId == x.SchoolId).Include(x => x.Cordinator).Include(x => x.School).Include(x => x.Speaker).ToList();
            if (activities != null && activities.Count > 0)
            {
                var weeklyActivities = activities.Where(x => dateRange.Contains(x.Date))
                    .Select(x => new SemesterManualViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        BibleVerse = x.BibleVerse,
                        Date = x.Date.ToString("D"),
                        Day = x.Date.ToString("dddd"),
                        SchoolId = x.SchoolId,
                        SpeakerId = x.SpeakerId,
                        SpeakerName = x.Speaker.FullName,
                        SpeakerProfilePicture = x.Speaker.ProfilePicture,
                        CordinatorId = x.CordinatorId,
                        CordinatorName = x.Cordinator.FullName,
                        CordinatorProfilePicture = x.Cordinator.ProfilePicture,
                    }).ToList();
                if (weeklyActivities != null && weeklyActivities.Count > 0)
                {
                    foreach (var activity in weeklyActivities)
                    {
                        if (activity.Day == "Wednesday")
                        {
                            weeklyActivity.Wednesday = activity;
                        }
                        if (activity.Day == "Friday")
                        {
                            weeklyActivity.Friday = activity;
                        }
                        if (activity.Day == "Saturday")
                        {
                            weeklyActivity.Shabbath = activity;
                        }
                        if (activity.Day == "Sunday")
                        {
                            weeklyActivity.Sunday = activity;
                        }
                    }
                    return weeklyActivity;
                }
            }
            return weeklyActivity;
        }
    }
}
