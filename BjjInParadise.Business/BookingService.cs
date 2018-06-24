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

        protected override Task<Booking> Add(Booking t)
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
    }
}
