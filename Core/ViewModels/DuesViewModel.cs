namespace Core.ViewModels
{
    public class DuesViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double? Amount { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string DateAdded { get; set; }
    }
}
