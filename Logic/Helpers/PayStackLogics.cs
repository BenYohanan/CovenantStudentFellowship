using Core.DB;
using Core.Enums;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using System.Linq.Expressions;
using System.Net;
using static Core.Configurations.PaystackRespsonse;

namespace Logic.Helpers
{
    public class PaystackLogic : IPaystackLogic
    {
        private RestClient client;
        protected RestRequest request;
        public static string RestUrl = "https://api.paystack.co/";
        static string ApiEndPoint = "";
        private AppDbContext _context;
        private IGeneralConfiguration _generalConfiguration;
        private IEmailService _emailService;

        public PaystackLogic(AppDbContext contex, IGeneralConfiguration generalConfiguration, IEmailService emailService)
        {
            client = new RestClient(RestUrl);
            _context = contex;
            _generalConfiguration = generalConfiguration;
            _emailService = emailService;
        }

        public PaystackRepsonse MakeDonationPlanPayment(DonationPackage donationPackage)
        {
            PaystackRepsonse paystackRepsonse = null;
            try
            {
                long milliseconds = DateTime.Now.Ticks;
                string testid = milliseconds.ToString();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ApiEndPoint = "/transaction/initialize";
                request = new RestRequest(ApiEndPoint, Method.Post);
                request.AddHeader("accept", "application/json");
                request.AddHeader("Authorization", "Bearer " + _generalConfiguration.PayStakApiKey);
                request.AddParameter("reference", donationPackage.InvoiceNumber);
                request.AddParameter("amount", donationPackage.Amount * 100);

                List<CustomeField> myCustomfields = new List<CustomeField>();
                CustomeField nameCustomeField = new CustomeField();
                nameCustomeField.display_name = "Email";
                nameCustomeField.variable_name = "Email";
                nameCustomeField.value = donationPackage.Email;
                myCustomfields.Add(nameCustomeField);


                Dictionary<string, List<CustomeField>> metadata = new Dictionary<string, List<CustomeField>>();
                metadata.Add("custom_fields", myCustomfields);
                var serializedMetadata = JsonConvert.SerializeObject(metadata);
                request.AddParameter("metadata", serializedMetadata);
                request.AddParameter("email", donationPackage.Email);

                var serializedRequest = JsonConvert.SerializeObject(request, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                var result = client.ExecuteAsync(request).Result;
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    paystackRepsonse = JsonConvert.DeserializeObject<PaystackRepsonse>(result.Content);
                }
                return paystackRepsonse;
            }
            catch (Exception ex)
            {
                var toEmail = _generalConfiguration.DeveloperEmail;
                var subject = "Paystack Response Exception on CSF";
                _emailService.SendEmail(toEmail, subject, ex.Message);
                throw;
            }
        }

        public async Task<PaystackRepsonse> VerifyPayment(DonationPackage payment)
        {
            PaystackRepsonse paystackRepsonse = null;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ApiEndPoint = "/transaction/verify/" + payment.InvoiceNumber;
                request = new RestRequest(ApiEndPoint, Method.Get);
                request.AddHeader("accept", "application/json");
                request.AddHeader("Authorization", "Bearer " + _generalConfiguration.PayStakApiKey);
                var result = client.ExecuteAsync(request).Result;
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    paystackRepsonse = JsonConvert.DeserializeObject<PaystackRepsonse>(result.Content);
                    await UpdateResponse(paystackRepsonse);
                }
                return paystackRepsonse;
            }
            catch (Exception ex)
            {
                var toEmail = _generalConfiguration.DeveloperEmail;
                var subject = "Paystack Main Payment Verification Exception on CSF";
                _emailService.SendEmail(toEmail, subject, ex.Message);
                throw;
            }
        }

        public async Task<bool> UpdateResponse(PaystackRepsonse PaystackRepsonse)
        {
            try
            {
                Expression<Func<Paystack, bool>> selector = p => p.DonationPackage.InvoiceNumber == PaystackRepsonse.data.reference;
                Paystack _paystackEntity = _context.Paystack.Where(selector).Include(s => s.DonationPackage).OrderBy(s => s.amount).LastOrDefault();
                if (_paystackEntity != null)
                {
                    _paystackEntity.amount = PaystackRepsonse.data.amount;
                    _paystackEntity.bank = PaystackRepsonse.data.authorization.bank;
                    _paystackEntity.brand = PaystackRepsonse.data.authorization.brand;
                    _paystackEntity.card_type = PaystackRepsonse.data.authorization.card_type;
                    _paystackEntity.channel = PaystackRepsonse.data.channel;
                    _paystackEntity.country_code = PaystackRepsonse.data.authorization.country_code;
                    _paystackEntity.currency = PaystackRepsonse.data.currency;
                    _paystackEntity.domain = PaystackRepsonse.data.domain;
                    _paystackEntity.exp_month = PaystackRepsonse.data.authorization.exp_month;
                    _paystackEntity.exp_year = PaystackRepsonse.data.authorization.exp_year;
                    _paystackEntity.fees = PaystackRepsonse.data.fees.ToString();
                    _paystackEntity.gateway_response = PaystackRepsonse.data.gateway_response;
                    _paystackEntity.ip_address = PaystackRepsonse.data.ip_address;
                    _paystackEntity.last4 = PaystackRepsonse.data.authorization.last4;
                    _paystackEntity.message = PaystackRepsonse.message;
                    _paystackEntity.reference = PaystackRepsonse.data.reference;
                    _paystackEntity.reusable = PaystackRepsonse.data.authorization.reusable;
                    _paystackEntity.signature = PaystackRepsonse.data.authorization.signature;
                    _paystackEntity.status = PaystackRepsonse.data.status;
                    _paystackEntity.transaction_date = PaystackRepsonse.data.transaction_date;

                    _context.Update(_paystackEntity);

                    _context.SaveChanges();

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                var toEmail = _generalConfiguration.DeveloperEmail;
                var subject = "Paystack Updating Response Exception on CSF";
                _emailService.SendEmail(toEmail, subject, ex.Message);
                throw;
            }
        }

        public Paystack GetBy(string reference)
        {
            return _context.Paystack.Where(a => a.DonationPackage.InvoiceNumber == reference).LastOrDefault();
        }

        public DonationPackage ValidatePayment(string reference)
        {
            try
            {
                var details = _context.Paystack.Where(a => a.DonationPackage.InvoiceNumber == reference).LastOrDefault();
                if (details != null && details.status != null && details.status.Contains("success") && (details.gateway_response.Contains("Approved") || details.gateway_response.Contains("Transaction Successful") || details.gateway_response.Contains("Successful") || details.gateway_response.Contains("Payment successful") || details.gateway_response.Contains("success")) && details.domain == "live")
                {
                    return details.DonationPackage;
                }
                return null;
            }
            catch (Exception ex)
            {
                var toEmail = _generalConfiguration.DeveloperEmail;
                var subject = "Paystack validating payment Step2 Exception on CSF";
                _emailService.SendEmail(toEmail, subject, ex.Message);
                throw;
            }
        }

        public DateTime ConvertToDate(string date)
        {
            DateTime newDate = new DateTime();
            string[] dateSplit = date.Split('-');
            newDate = new DateTime(Convert.ToInt32(dateSplit[0]), Convert.ToInt32(dateSplit[1]), Convert.ToInt32(dateSplit[2]));
            return newDate;
        }

        public async Task<PaystackRepsonse> GeneratePaymentParameters(DonationViewModel userDetails)
        {
            try
            {
                if (userDetails != null)
                {
                    var packagePayment = new DonationPackage();
                    packagePayment.PaymentDate = DateTime.Now;
                    packagePayment.Status = PaymentStatus.Pending;
                    packagePayment.InvoiceNumber = GenerateNumber();
                    packagePayment.Amount = userDetails.Amount;
                    packagePayment.Email = userDetails.Email;
                    packagePayment.FullName = userDetails.FullName;
                    packagePayment.CategoryId = userDetails.CategoryId;
                    packagePayment.CurrencyId = userDetails.CurrencyId;
                    packagePayment.StateId = userDetails.StateId;


                    var newPackagePayment = await _context.AddAsync(packagePayment);
                    await _context.SaveChangesAsync();
                    if (newPackagePayment.Entity.Id != Guid.Empty)
                    {
                        var paystackResponse = MakeDonationPlanPayment(newPackagePayment.Entity);
                        if (paystackResponse != null)
                        {
                            Paystack paystack = new Paystack();
                            paystack.DonationPackage = newPackagePayment.Entity;
                            paystack.authorization_url = paystackResponse.data.authorization_url;
                            paystack.access_code = paystackResponse.data.access_code;
                            paystack.DonationPackageId = newPackagePayment.Entity.Id;
                            _context.Paystacks.Add(paystack);
                            await _context.SaveChangesAsync();
                            return paystackResponse;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                var toEmail = _generalConfiguration.DeveloperEmail;
                var subject = "Generate Payment Parameters Method Exception on CSF";
                _emailService.SendEmail(toEmail, subject, ex.Message);
                throw;
            }
        }

        private string GenerateNumber()
        {
            return DateTime.Now.ToString().ToLower().Replace("am", "").Replace("pm", "").Replace(":", "").Replace("/", "").Replace(" ", "");
        }

        public async Task<bool> VerifyPayment(string invoiceNumber)
        {
            var donationPackage = _context.DonationPackage.Where(s => s.InvoiceNumber == invoiceNumber).FirstOrDefault();
            var response = await VerifyPayment(donationPackage);
            if (response.status)
            {
                return true;
            }
            return false;
        }
    }
}
