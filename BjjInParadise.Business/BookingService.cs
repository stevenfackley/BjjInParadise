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
           return _context.Bookings.Find(id);
        }

        public override IEnumerable<Booking> GetAll()
        {
            throw new NotImplementedException();
        }

        public override Task<Booking> UpdateAsync(Booking t)
        {
            throw new NotImplementedException();
        }
        public Booking AddNew(Booking t)
        {
            UpdateCreatedAndModifiedDate(t);
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
           ,[ModifiedDate]
,[BrainTreeTransactionId])
     VALUES
           (@BookingDate
           ,@AmountPaid
           ,@UserId
           ,@CampId
           ,@CampRoomOptionId
           ,@CreatedDate
           ,@ModifiedDate
           ,@BrainTreeTransactionId)
";

                    var result =  db.Execute(insertQuery, t);
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
            }

            return t;
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
    
      

   
    }
}
