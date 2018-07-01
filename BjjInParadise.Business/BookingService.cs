using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BjjInParadise.Core.Models;
using BjjInParadise.Data;
using Dapper;
using PayPal.Api;
using Transaction = System.Transactions.Transaction;

namespace BjjInParadise.Business
{
    public class BookingService : BaseCrudService<Booking>
    {
        private BjjInParadiseContext _context;
        private string _connectionString;

        public BookingService(BjjInParadiseContext context)
        {
            _context = context;
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
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

        protected override Task<Booking> Add(Booking vm)
        {


            throw new NotImplementedException();

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

        public bool ProcessPayment(Booking result)
        {
            // Authenticate with PayPal
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken);



            // A transaction defines the contract of a payment - what is the payment for and who is fulfilling it. 
            var transaction = new PayPal.Api.Transaction
            {
                amount = new Amount
                {
                    currency = "USD",
                    total = result.AmountPaid.ToString(),
                    details = new Details
                    {
                        subtotal = result.AmountPaid.ToString()
                    }
                },
                description = result.Camp.CampName,
                item_list = new ItemList
                {
                    items = new List<Item>
                    {
                        new Item
                        {
                            name = result.CampRoomOption.RoomType,
                            currency = "USD",
                            price = result.AmountPaid.ToString(),
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
                },
                invoice_number = result.CampId + result.UserId.ToString() + new Random().Next(10000,99999) //Common.GetRandomInvoiceNumber()
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
                            expire_month =int.Parse(result.Expiration.Substring(0, 2)),
                            expire_year = int.Parse(result.Expiration.Substring(2, 2)),
                            first_name = result.User.FirstName,
                            last_name = result.User.LastName,
                            number = result.CreditCard,
                            type = "visa"
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
                transactions = new List<PayPal.Api.Transaction> { transaction }
            };

            try
            {
                // Create a payment using a valid APIContext
                var createdPayment = payment.Create(apiContext);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
           
        }

   
    }
}
