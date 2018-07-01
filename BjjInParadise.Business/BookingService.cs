using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BjjInParadise.Core.Models;
using BjjInParadise.Data;
using Dapper;
using NLog;
using PayPal.Api;
using Transaction = System.Transactions.Transaction;

namespace BjjInParadise.Business
{
    public class BookingService : BaseCrudService<Booking>
    {
        private BjjInParadiseContext _context;
        private string _connectionString;
        private CampRoomOptionService _campRoomOptionService;
        private AccountService _accService;
        private CampService _campService;

        public BookingService(BjjInParadiseContext context, CampRoomOptionService service, AccountService accService, CampService campservice)
        {
            _context = context;
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            _campRoomOptionService = service;
            _accService = accService;
            _campService = campservice;
        }

        public override Task DeleteAsync(Booking t)
        {
            throw new NotImplementedException();
        }

        public override Booking Get(int? id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Booking> GetAll()
        {
            throw new NotImplementedException();
        }

        public override Task<Booking> UpdateAsync(Booking t)
        {
            throw new NotImplementedException();
        }

        protected override async Task<Booking> Add(Booking t)
        {
            try
            {
                t.BookingDate = t.ModifiedDate;
                if (t.AmountPaid == null)
                    t.AmountPaid = _campRoomOptionService.Get(t.CampRoomOptionId).CostPerPerson;

                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    string insertQuery = @"INSERT INTO [dbo].[Booking]
           ([BookingDate]
           ,[AmountPaid]
           ,[UserId]
           ,[CampId]
           ,[CampRoomOptionId]
           ,[CreatedDate]
           ,[ModifiedDate])
     VALUES
           (@BookingDate
           ,@AmountPaid
           ,@UserId
           ,@CampId
           ,@CampRoomOptionId
           ,@CreatedDate
           ,@ModifiedDate)
";

                    var result = await db.ExecuteAsync(insertQuery, t);
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
            }

            return t;

        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var result = await db.QueryAsync<Booking>("SELECT * From Booking where [UserId] = @UserId", new { UserId = userId });
                    var enumResult = result.ToList();
                    foreach (var booking in enumResult)
                    {
                        //Add foreign key table
                        var camp = await db.QueryAsync<Camp>("SELECT TOP 1 * From Camp where [CampId] = @CampId", new { CampId = booking.CampId });
                        booking.Camp = camp.FirstOrDefault();
                    }

                    return enumResult;
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
                throw;
            }
        }

        public Payment ProcessPayment(Booking result)
        {

            AddUser(result);
            AddCamp(result);

            return HandlePayPalProcessPayment(result);
        }

        private void AddUser(Booking booking)
        {
            var result = _accService.Get(booking.UserId);
            booking.User = result;
        }
        private void AddCamp(Booking booking)
        {
            var result = _campService.Get(booking.CampId);
            booking.Camp = result;
        }
        private CampRoomOption GetCampRoomOption(Booking booking)
        {
            var result = _campRoomOptionService.Get(booking.CampRoomOptionId);
            return result;
        }

        private  Payment HandlePayPalProcessPayment(Booking result)
        {
// Authenticate with PayPal
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken);
            var cro = GetCampRoomOption(result);
            var amt = new Amount
            {
                currency = "USD",
                total = cro.CostPerPerson.ToString(),
                details = new Details
                {
                    subtotal = cro.CostPerPerson.ToString()
                }
            };
            var itemList = new ItemList
            {
                items = new List<Item>
                {
                    new Item
                    {
                        name = cro.RoomType,
                        currency = "USD",
                        price = cro.CostPerPerson.ToString(),
                        quantity = "1"
                    }
                },
                shipping_address = new ShippingAddress
                {
                    city = result.User.City,
                    country_code = result.User.Country,
                    line1 = result.User.Street,
                    postal_code = result.User.ZipCode,
                    state = result.User.State,
                    recipient_name = result.User.FirstName + " " + result.User.LastName
                }
            };
            // A transaction defines the contract of a payment - what is the payment for and who is fulfilling it. 
            var transaction = new PayPal.Api.Transaction
            {
                amount = amt,
                description = result.Camp.CampName,
                item_list = itemList,
                invoice_number =
                    result.CampId + result.UserId.ToString() + new Random().Next(10000, 99999) //Common.GetRandomInvoiceNumber()
            };

            // A resource representing a Payer that funds a payment.
            var payer = new Payer
            {
                payment_method = "credit_card",
                funding_instruments = new List<FundingInstrument>
                {
                    new FundingInstrument
                    {
                        credit_card = new CreditCard
                        {
                            billing_address = new Address
                            {
                                city = result.User.City,
                                country_code = result.User.Country,
                                line1 = result.User.Street,
                                postal_code = result.User.ZipCode,
                                state = result.User.State,
                            },
                            cvv2 = result.CVC,
                            expire_month = int.Parse(result.Expiration.Substring(0, 2)),
                            expire_year = int.Parse(result.Expiration.Substring(result.Expiration.Length - 4)),
                            first_name = result.User.FirstName,
                            last_name = result.User.LastName,
                            number = result.CreditCard,
                            type = GetCreditCardType(result.CreditCard)
                        }
                    }
                },
                payer_info = new PayerInfo
                {
                    email = result.Email
                }
            };

            // A Payment resource; create one using the above types and intent as `sale` or `authorize`
            var payment = new Payment
            {
                intent = "sale",
                payer = payer,
                transactions = new List<PayPal.Api.Transaction> {transaction}
            };

            Payment createdPayment = new Payment();
            try
            {
                // Create a payment using a valid APIContext
                 createdPayment = payment.Create(apiContext);
       
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
            }
            return createdPayment;
        }

        private static string GetCreditCardType(string ccn)
        {
            var regVisa = new Regex("^4[0-9]{12}(?:[0-9]{3})?$");
            var regMaster = new Regex("^5[1-5][0-9]{14}$");
            Regex regExpress = new Regex("^3[47][0-9]{13}$");
            Regex regDiners = new Regex("^3(?:0[0-5]|[68][0-9])[0-9]{11}$");
            Regex regDiscover = new Regex("^6(?:011|5[0-9]{2})[0-9]{12}$");
            Regex regJCB = new Regex("^(?:2131|1800|35\\d{3})\\d{11}$");


            if (regVisa.IsMatch(ccn))
                return "visa";
            if (regMaster.IsMatch(ccn))
                return "mastercard";
            if (regExpress.IsMatch(ccn))
                return "amex";
            if (regDiners.IsMatch(ccn))
                return "diners";
            if (regDiscover.IsMatch(ccn))
                return "discover";
            if (regJCB.IsMatch(ccn))
                return "jcb";
            return "invalid";
        }
    }
}
