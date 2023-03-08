using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Models.JewishActivity;

namespace Core.ViewModels
{
    public class HomePageViewModel
    {
        public virtual PictureViewModel PictureViewModel { get; set; }
        public virtual List<EventViewModel> Events { get; set; }
        public virtual List<BlogViewModel> Blog { get; set; }
        public virtual List<GalleryViewModel> Gallery { get; set; }
        public JewishActivity JewishActivity { get; set; }
        public List<Item> Item { get; set; }
    }
}
