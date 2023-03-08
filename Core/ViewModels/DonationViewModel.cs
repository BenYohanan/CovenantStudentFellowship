using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class DonationViewModel
    {
        public Guid Id { get; set; }

        public double Amount { get; set; }

        public PaymentStatus Status { get; set; }

        public string InvoiceNumber { get; set; }

        public string TransactionDetails { get; set; }

        public DateTime PaymentDate { get; set; }

        public DonationInterval Interval { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public int? CurrencyId { get; set; }

        public int? StateId { get; set; }

        public int? CategoryId { get; set; }

        public bool Annonymous { get; set; }

        public bool Deleted { get; set; }
    }
}
