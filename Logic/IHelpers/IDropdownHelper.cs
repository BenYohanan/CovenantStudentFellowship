using Core.Enums;
using Core.Models;
using Core.ViewModels;
using static Logic.Helpers.DropdownHelper;

namespace Logic.IHelpers
{
    public interface IDropdownHelper
    {
        Task<List<Country>> GetCountry();
        Task<List<State>> GetState();
        Task<List<CommonDropdown>> GetDropdownByKey(DropdownEnums dropdownKey, bool deleteOption = false);
        Task<bool> CreateDropdownsAsync(CommonDropdown commonDropdowns);
        List<DropdownEnumModel> GetDropDownEnumsList();
        bool EditDropdown(CommonDropdown commonDropdowns);
        DropdownEnumModel GetEnumById(int enumkeyId);
        bool DeleteDropdownById(int id);
        List<DropdownEnumModel> GetDocumentFromEnums();
        List<CommonDropdown> ListOfBlogCategories();
        List<CommonDropdown> DonationCurrency();
        List<CommonDropdown> DonationCategory();
        List<ApplicationUserViewModel> GetUsers();
        List<SchoolViewModel> GetSchools();
    }
}
