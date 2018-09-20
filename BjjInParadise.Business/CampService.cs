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
    public class CampService : BaseCrudService<Camp>
    {
        private BjjInParadiseContext _context;
        private string _connectionString;
        private AccountService _accountService;

        public CampService(BjjInParadiseContext context, AccountService accountService)
        {
            _context = context;
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            _accountService = accountService;
        }


        public override IEnumerable<Camp> GetAll()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return db.Query<Camp>("SELECT * FROM Camp").ToList();
            }

        }

        public async Task<IEnumerable<Camp> >GetAllActiveAsync()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryAsync<Camp>(
                    "SELECT DISTINCT  C.* FROM Camp C INNER JOIN CampRoomOption CRO on C.CampId = CRO.CampId  where IsActive = 1 order by StartDate");

            }

        }

        public IEnumerable<Camp> GetAllActive()
        {
            //using (IDbConnection db = new SqlConnection(_connectionString))
            //{
            //    var lookup = new Dictionary<int, Camp>();
            //  var result=    db.Query<Camp, CampRoomOption, Camp>(
            //        "SELECT DISTINCT  C.* FROM Camp C INNER JOIN CampRoomOption CRO on C.CampId = CRO.CampId  where IsActive = 1 order by StartDate",
            //        (s, a) =>
            //        {
            //            Camp camp;
            //            if (!lookup.TryGetValue(s.CampId, out camp))
            //            {
            //                camp = s;
            //                camp.CampRoomOptions = new List<CampRoomOption>();
            //                lookup.Add(s.CampId, camp = s);
            //            }
   
            //            camp.CampRoomOptions.Add(a);
            //            return camp;

            //        }, splitOn: "CampRoomOptionId");
            //    return result;
            //}

         return _context.Camps.Where(x => x.IsActive).Include(x => x.CampRoomOptions).Include(x => x.Bookings)
                .OrderBy(x => x.StartDate);

           

        }
        protected override async Task<Camp> Add(Camp t)
        {
            
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                try
                {

                  
                    string insertQuery = @"INSERT INTO [dbo].[Camp]
                                                                   ([CampName]
                                                                   ,[StartDate]
                                                                   ,[EndDate]
                                                                   ,[IsActive]
                                                                   ,[CreatedDate]
                                                                   ,[ModifiedDate]
                                                                    ,[HtmlPageText])
                                                             VALUES
                                                                   (@CampName
                                                                   ,@StartDate
                                                                   ,@EndDate
                                                                   ,@IsActive
                                                                   ,@CreatedDate
                                                                   ,@ModifiedDate
                                                                    ,@HtmlPageText)";
                    var param = new
                    {
                        t.CampName,
                        t.StartDate,
                        t.EndDate,
                        t.IsActive,
                        t.CreatedDate,
                        t.ModifiedDate,
                        HtmlPageText = new DbString
                        {
                            Value = t.HtmlPageText,
                            IsFixedLength = false,
                            Length = -1,
                            IsAnsi = true
                        }
                    };
                    var result = await db.ExecuteAsync(insertQuery, param);
                    return t;
                }
                catch (Exception e)
                {
                    Log.Instance.Error(e);
                    throw;
                }
            }
        }

        public override Camp Get(int? id)
        {
            if (id == null)
                return null;

            try
            {
                var result = _context.Camps.Where(x => x.CampId == id).Include(x => x.CampRoomOptions)
                    .Include(x => x.Bookings).FirstOrDefault();


                return result;
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
            }

          
            return null;

        }

        public override async  Task<Camp> UpdateAsync(Camp t)
        {
            if (t == null)
            {
                return null;
            }
            var entity = _context.Camps.Find(t.CampId);
            if (entity == null)
            {
                return null;
            }

            try
            {
                UpdateModifiedDate(t);
                _context.Entry(entity).CurrentValues.SetValues(t);

                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
                throw;
            }
          

        }
        private IEnumerable<Booking> GetByCampId(int? id)
        {
            if (id == null)
                return null;

            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    string selectBooking = "SELECT * FROM [dbo].[Booking] WHERE CampId = @CampId ";
                    var result = db.Query<Booking>(selectBooking, new
                    {
                        CampId = id
                    });
                    return result;


                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
                return null;
            }

        }
        public override async Task DeleteAsync(Camp camp)
        {
            if (camp == null) return;
            var delCamp = new Camp {CampId = camp.CampId};
            _context.Camps.Attach(delCamp);
           _context.Camps.Remove(delCamp);
            await _context.SaveChangesAsync();
        }

        public async Task<Camp> GetNextCampAsync()
        {

            try
            {
                return await _context.Camps.Where(x => x.IsActive).Include(x => x.CampRoomOptions)
                    .Include(x => x.Bookings)
                    .OrderBy(x => x.StartDate).FirstOrDefaultAsync();

                //using (IDbConnection db = new SqlConnection(_connectionString))
                //{
                //    var result =
                //        await db.QueryAsync<Camp>("Select top 1 * From Camp where IsActive = 1 order by StartDate");

                //    return result.FirstOrDefault();
                //}

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool AreAnyCampsAvailableToBeBooked(Camp camp)
        {
            var cros = camp.CampRoomOptions.ToList();
            var bookings = camp.Bookings.ToList();

            var spotsAvailable = cros.Sum(x => x.SpotsAvailable);
            var bookedSpots = bookings.Count;
            var areAnyCampsAvailableToBeBooked = spotsAvailable > bookedSpots;//  camps.Any(x =>x.CampRoomOptions x.Bookings.Any());
            return areAnyCampsAvailableToBeBooked;
        }
    }
}
