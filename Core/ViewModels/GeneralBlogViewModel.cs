using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class GeneralBlogViewModel
    {
        public List<BlogViewModel> BlogViewModels { get; set; }
        public List<CommonDropdown> BlogCategories { get; set; }
        public List<BlogCommentViewModel> BlogCommentViewModel { get; set; }
        public BlogViewModel BlogViewModel { get; set; }

    }
}
