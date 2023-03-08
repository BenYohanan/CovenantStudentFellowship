using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Configurations.PaystackRespsonse;

namespace Logic.IHelpers
{
    public interface IPaystackLogic
    {
        PaystackRepsonse MakeDonationPlanPayment(DonationPackage donationPackage);
        Task<PaystackRepsonse> VerifyPayment(DonationPackage payment);
        Task<bool> UpdateResponse(PaystackRepsonse PaystackRepsonse);
        Paystack GetBy(string reference);
        DonationPackage ValidatePayment(string reference);
        DateTime ConvertToDate(string date);
        Task<PaystackRepsonse> GeneratePaymentParameters(DonationViewModel userDetails);
        Task<bool> VerifyPayment(string invoiceNumber);
    }
}
