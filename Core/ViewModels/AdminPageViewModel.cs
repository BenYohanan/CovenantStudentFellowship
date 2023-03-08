namespace Core.ViewModels
{
    public class AdminPageViewModel
    {
        public virtual WeeklyActivityViewModel WeeklyActivityViewModel { get; set; }
        public virtual List<BlogViewModel> BlogViewModel { get; set; }
        public virtual List<EventViewModel> Events { get; set; }
        public virtual List<DuesViewModel> DuesViewModel { get; set; }
        public virtual List<DocumentViewModel> DocumentViewModel { get; set; }
        public int ActiveUserCount { get; set; }
        public int EventCount { get; set; }
        public int BlogCount { get; set; }
    }
}
