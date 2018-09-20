using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BjjInParadise.Core.Models;
using BjjInParadise.Data;
using Dapper;

namespace BjjInParadise.Business
{
    public class CampRoomOptionService : BaseCrudService<CampRoomOption>
    {
        private BjjInParadiseContext _context;
        private string _connectionString;
        private CampService _campService;

        public CampRoomOptionService(BjjInParadiseContext context, CampService campService)
        {
            _context = context;
            _campService = campService;
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public override async Task DeleteAsync(CampRoomOption t)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    string deleteBooking = "DELETE FROM [dbo].[CampRoomOption] WHERE CampRoomOptionId = @CampRoomOptionId ";
                    await db.ExecuteAsync(deleteBooking, new
                    {
                       t.CampRoomOptionId
                    });



                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
            }
        }

        public override  CampRoomOption Get(int? id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var result =  db.Query<CampRoomOption>("SELECT TOP 1 * From CampRoomOption where [CampRoomOptionId] = @CampRoomOptionId", new { CampRoomOptionId = id });
                    var retVal = result.FirstOrDefault();
                    if (retVal != null)
                    {
                        retVal.Camp = _campService.Get(retVal.CampId);
                    }

                    return retVal;
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
                throw;
            }
        }
        public async Task<IEnumerable<Booking>> GetBookingsByCampIdAsync(int campId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var result = await db.QueryAsync<Booking>("SELECT * From Booking where [CampId] = @CampId", new { CampId = campId });
                    var enumResult = result.ToList();
                    foreach (var booking in enumResult)
                    {
                        //Add foreign key table
                        var camp = await db.QueryAsync<Camp>("SELECT TOP 1 * From Camp where [CampId] = @CampId", new { CampId = campId });
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

        public IEnumerable<Booking> GetBookingsByCampId(int campId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var result =  db.Query<Booking>("SELECT * From Booking where [CampId] = @CampId", new { CampId = campId });
                    var enumResult = result.ToList();
                    foreach (var booking in enumResult)
                    {
                        //Add foreign key table
                        var camp =  db.Query<Camp>("SELECT TOP 1 * From Camp where [CampId] = @CampId", new { CampId = campId });
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
        public async Task< IEnumerable<CampRoomOption>> GetActiveOptionsByCampIdAsync(int id)
        {
            try
            {
             

                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var result = await db.QueryAsync<CampRoomOption>(@"SELECT cro.*	, BookedSpots = COUNT(B.BookingId)
            From CampRoomOption cro
            INNER JOIN Camp C
            On cro.CampId = C.CampId 
            left outer JOIN Booking B
            On B.CampId = C.CampId AND cro.CampRoomOptionId = B.CampRoomOptionId
          where cro.[CampId] = @CampId
				Group By cro.SpotsAvailable, cro.CampRoomOptionId, cro.CampId, cro.RoomType, cro.CostPerPerson, cro.RoomType,  cro.CreatedDate, cro.ModifiedDate
			Having cro.SpotsAvailable > COUNT(B.BookingId)
                                                           ",
                        new { CampId = id });


                    var camp = _campService.Get(id);
                    var retVal = new List<CampRoomOption>();
                  
                    foreach (var campRoomOption in result)
                    {
                        campRoomOption.Camp = camp;
                        retVal.Add(campRoomOption);
                    }

                    return retVal;
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
                throw;
            }
        }
        public IEnumerable<CampRoomOption> GetActiveOptionsByCampId(int id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var result =  db.Query<CampRoomOption>("SELECT * From CampRoomOption where [CampId] = @CampId",
                        new { CampId = id }).ToList();
                    

                    var camp = _campService.Get(id);
                    var retVal = new List<CampRoomOption>();
                    var bookings = GetBookingsByCampId(id);

                    foreach (var campRoomOption in result)
                    {
                        var bookedCount = bookings.Count(x => x.CampRoomOptionId == campRoomOption.CampRoomOptionId);
                        campRoomOption.BookedSpots = bookedCount;
                        campRoomOption.Camp = camp;
                        retVal.Add(campRoomOption);
                    }

                    return retVal;
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
                throw;
            }
        }
        public override IEnumerable<CampRoomOption> GetAll()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var result = db.Query<CampRoomOption>("SELECT  * From CampRoomOption");

                    return result;
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
                throw;
            }
        }

        public override async Task<CampRoomOption> UpdateAsync(CampRoomOption t)
        {
            if (t == null)
                return null;
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    t.ModifiedDate = DateTime.UtcNow;

                    var updateQuery =
                            @"UPDATE [dbo].[CampRoomOption]  
                    SET
                        [RoomType] = @RoomType,
                        [CostPerPerson] = @CostPerPerson,
                        [SpotsAvailable] = @SpotsAvailable
                        WHERE [CampRoomOptionId] = @CampRoomOptionId
                   "
                        ;
                    var obj = new
                    {

                        t.RoomType,
                        t.CostPerPerson,
                        t.SpotsAvailable,
                        t.CampRoomOptionId

                    };
                    var result = await db.ExecuteAsync(updateQuery, obj);
                    return t;
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
                throw;
            }
        }

        protected override async Task<CampRoomOption> Add(CampRoomOption t)
        {
            try
            {
                _context.CampRoomOptions.Add(t);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
            }

            return t;
        }
    }
}
