using Core.Enums;

namespace Core.ViewModels
{
    public class DocumentViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FileUrl { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string DateAdded { get; set; }
        public DocumentList? DocumentList { get; set; }
    }
}
