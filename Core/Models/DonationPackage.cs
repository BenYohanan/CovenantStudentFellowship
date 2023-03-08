using Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class DonationPackage : BaseModel
    {
        [Key]
        public new Guid Id { get; set; }
        public double Amount { get; set; }
        public PaymentStatus? Status { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? TransactionDetails { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DonationInterval? Interval { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public int? CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public virtual CommonDropdown? Currency { get; set; }
        public int? StateId { get; set; }
        [ForeignKey("StateId")]
        public virtual State? State { get; set; }
        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual CommonDropdown? Category { get; set; }
        public bool Annonymous { get; set; }
    }
}
