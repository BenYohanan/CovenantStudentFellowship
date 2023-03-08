using Core.DB;
using Core.Enums;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.EntityFrameworkCore;

namespace Logic.Helpers
{
    public class DropdownHelper : IDropdownHelper
    {
        private readonly AppDbContext _context;
        public DropdownHelper(AppDbContext context)
        {
            _context = context;
        }


        public async Task<List<Country>> GetCountry()
        {
            var listOfCountries = new List<Country>();
            var common = new Country()
            {
                Id = 0,
                Name = "-- Select --"
            };
            var country = await _context.Countries.Where(d => !d.Deleted && d.Id != 0).OrderBy(s => s.Id).ToListAsync();
            country.Insert(0, common);
            if (country.Any())
            {
                return country;
            }
            return listOfCountries;
        }

        public async Task<List<State>> GetState()
        {
            var states = new List<State>();
            var common = new State()
            {
                Id = 0,
                Name = "-- Select --"

            };
            var selectedBranches = await _context.States.OrderBy(x => x.Name).Where(x => x.Active && !x.Deleted).ToListAsync();
            if (selectedBranches != null)
            {
                selectedBranches.Insert(0, common);
                return selectedBranches;
            }
            return states;
        }

        public class DropdownEnumModel
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        public async Task<List<CommonDropdown>> GetDropdownByKey(DropdownEnums dropdownKey, bool deleteOption = false)
        {
            var drpdwns = new List<CommonDropdown>();
            var common = new CommonDropdown()
            {
                Id = 0,
                Name = "-- Select --"
            };
            var dropdowns = await _context.CommonDropdowns.Where(s => s.Deleted == deleteOption && s.DropdownKey == (int)dropdownKey).OrderBy(s => s.Name).ToListAsync();
            dropdowns.Insert(0, common);
            if (dropdowns.Any())
            {
                return dropdowns;
            }
            return drpdwns;
        }

        public List<DropdownEnumModel> GetDropDownEnumsList()
        {
            var common = new DropdownEnumModel()
            {
                Id = 0,
                Name = "-- Select --"

            };
            var enumList = ((DropdownEnums[])Enum.GetValues(typeof(DropdownEnums))).Select(c => new DropdownEnumModel() { Id = (int)c, Name = c.ToString() }).ToList();
            enumList.Insert(0, common);
            return enumList;
        }

        public List<DropdownEnumModel> GetDocumentFromEnums()
        {
            var common = new DropdownEnumModel()
            {
                Id = 0,
                Name = "-- Select --"

            };
            var enumList = ((DocumentList[])Enum.GetValues(typeof(DocumentList))).Select(c => new DropdownEnumModel() { Id = (int)c, Name = c.ToString() }).ToList();
            enumList.Insert(0, common);
            return enumList;
        }

        public async Task<bool> CreateDropdownsAsync(CommonDropdown commonDropdowns)
        {
            if (commonDropdowns != null && commonDropdowns.DropdownKey > 0 && commonDropdowns.Name != null)
            {
                var newCommonDropdowns = new CommonDropdown
                {
                    Name = commonDropdowns.Name,
                    DropdownKey = commonDropdowns.DropdownKey,
                    Active = true,
                    Deleted = false,
                    DateAdded = DateTime.Now,
                };

                var createdDropdowns = await _context.AddAsync(newCommonDropdowns);
                await _context.SaveChangesAsync();
                if (createdDropdowns.Entity.Id > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool EditDropdown(CommonDropdown commonDropdowns)
        {
            var dropdown = _context.CommonDropdowns.Where(c => c.Id == commonDropdowns.Id).FirstOrDefault();
            if (dropdown != null)
            {
                dropdown.Name = commonDropdowns.Name;
                _context.Update(dropdown);
                _context.SaveChanges();

                return true;
            }
            return false;
        }

        public DropdownEnumModel GetEnumById(int enumkeyId)
        {
            return ((DropdownEnums[])Enum.GetValues(typeof(DropdownEnums))).Select(c => new DropdownEnumModel() { Id = (int)c, Name = c.ToString() }).Where(x => x.Id == enumkeyId).FirstOrDefault();
        }

        public bool DeleteDropdownById(int id)
        {
            if (id == 0)
            {
                return false;
            }
            var dropdown = _context.CommonDropdowns.Where(d => d.Id == id && d.Active && !d.Deleted).FirstOrDefault();
            if (dropdown != null)
            {
                dropdown.Active = false;
                dropdown.Deleted = true;
                _context.CommonDropdowns.Update(dropdown);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<CommonDropdown> ListOfBlogCategories()
        {
            var categories = new List<CommonDropdown>();
            var blogCategories = _context.CommonDropdowns.Where(x => x.DropdownKey == 5 && x.Active && !x.Deleted).ToList();
            if (blogCategories != null && blogCategories.Count > 0)
            {
                return blogCategories;
            }
            return categories;
        }

        public List<CommonDropdown> DonationCurrency()
        {
            var common = new CommonDropdown()
            {
                Id = 0,
                Name = "-- Select --"
            };
            var donationCurrency = _context.CommonDropdowns.Where(x => x.DropdownKey == 6 && x.Active && !x.Deleted).ToList();
            donationCurrency.Insert(0, common);
            return donationCurrency;
        }

        public List<CommonDropdown> DonationCategory()
        {
            var common = new CommonDropdown()
            {
                Id = 0,
                Name = "-- Select --"
            };
            var donationCategory = _context.CommonDropdowns.Where(x => x.DropdownKey == 7 && x.Active && !x.Deleted).ToList();
            donationCategory.Insert(0, common);
            return donationCategory;
        }

        public List<ApplicationUserViewModel> GetUsers()
        {
            var loggedInuser = Session.GetCurrentUser();
            var common = new ApplicationUserViewModel()
            {
                Id = "",
                FullName = "-- Select --"
            };
            var users = _context.ApplicationUser.Where(x => x.Id != null && x.SchoolId == loggedInuser.SchoolId && !x.Deactivated).Select(x => new ApplicationUserViewModel
            {
                Id = x.Id,
                FullName = x.FullName,
            }).ToList();
            users.Insert(0, common);
            return users;
        }
        public List<SchoolViewModel> GetSchools()
        {
            var loggedInuser = Session.GetCurrentUser();
            var common = new SchoolViewModel()
            {
                Id = Guid.Empty,
                SchoolCodeName = "-- Select --"
            };
            var schools = _context.School.Where(x => x.Id != Guid.Empty && x.SchoolCodeName != null && !x.Deleted).Select(x => new SchoolViewModel
            {
                Id = x.Id,
                SchoolCodeName = x.SchoolCodeName,
            }).ToList();
            schools.Insert(0, common);
            return schools;
        }
    }
    public class drp
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}

