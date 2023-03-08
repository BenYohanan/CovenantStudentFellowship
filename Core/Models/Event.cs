namespace Core.Models
{
    public class Event : BaseModel
    {
        public string? Note { get; set; }
        public string? Title { get; set; }
        public DateTime Date { get; set; }
    }
}
