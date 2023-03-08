using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Document : BaseModel
    {
        [Key]
        public new Guid Id { get; set; }
        public string? FileUrl { get; set; }
        public DocumentList? DocumentList { get; set; }
    }
}
