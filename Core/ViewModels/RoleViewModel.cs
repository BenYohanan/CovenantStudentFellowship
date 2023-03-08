using Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Core.ViewModels
{
    public class RoleViewModel
    {
        public string UserName { get; set; }
        public string SelectedRole { get; set; }
        public virtual IdentityRole Role { get; set; }
        public virtual ApplicationUserViewModel User { get; set; }
        public string Email { get; set; }
    }
}
