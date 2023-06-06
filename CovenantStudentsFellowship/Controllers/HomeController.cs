using Core.ViewModels;
using CovenantStudentsFellowship.Models;
using Logic.IHelpers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CovenantStudentsFellowship.Controllers
{
    public class HomeController : Controller
    {
        private IBlogHelper _blogHelper;
        private IUserHelper _userHelper;
        private IAdminHelper _adminHelper;
        private readonly ISuperAdminHelper _superAdminHelper;

        public HomeController(IBlogHelper blogHelper,
           IUserHelper userHelper, IAdminHelper adminHelper, ISuperAdminHelper superAdminHelper)
        {
            _userHelper = userHelper;
            _adminHelper = adminHelper;
            _blogHelper = blogHelper;
            _superAdminHelper = superAdminHelper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var events = _adminHelper
                .GetHomePageEvent()?
                .OrderByDescending(x => x.DateAdded)
                .Take(7)
                .ToList();
            var blogs = _blogHelper.
                GetAllBlog()?
                .OrderByDescending(x => x.DateCreated)
                .Take(5)
                .ToList();
            var topPictures = _adminHelper
                .GetAllPicture()?
                .OrderByDescending(x => x.DateAdded)
                .Take(5)
                .ToList();
            var homePagePictures = _superAdminHelper
                .HomePagePictures();
            //var jewishActivity = _adminHelper.DisplayJewishActivities().Result;
            //var jewishActivityToBeDisplayedOnHomePage = jewishActivity.Items.ToList();
            var homePageViewModel = new HomePageViewModel()
            {
                PictureViewModel = homePagePictures,
                Gallery = topPictures,
                Blog = blogs,
                Events = events,
                //Item = jewishActivityToBeDisplayedOnHomePage,
            };
            return View(homePageViewModel);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}