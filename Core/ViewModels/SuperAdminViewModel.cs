namespace Core.ViewModels
{
    public class SuperAdminViewModel
    {
        public int UserCount { get; set; }
        public int BlogCount { get; set; }
        public int BlogSubscriberCount { get; set; }
        public int SchoolCount { get; set; }
        public string BlogTitle { get; set; }
        public string AuthorsName { get; set; }
        public string BlogStatus { get; set; }
        public string BlogAddedDate { get; set; }
        public string EventTitle { get; set; }
        public string EventDate { get; set; }
        public string EventNote { get; set; }
        public string SysytemAdminName { get; set; }
        public string SysytemAdminEmail { get; set; }
        public string SysytemAdminSchool { get; set; }
        public string SysytemAdminPhone { get; set; }
        public string DuesTitle { get; set; }
        public string DuesAmount { get; set; }
        public List<BlogViewModel> BlogViewModel { get; set; }
        public List<EventViewModel> Events { get; set; }
        public List<ApplicationUserViewModel> SystemAdmins { get; set; }

    }
}
