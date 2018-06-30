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

        public IEnumerable<CampRoomOption> GetByCampId(int? id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var result = db.Query<CampRoomOption>("SELECT * From CampRoomOption where [CampId] = @CampId",
                        new {CampId = id}).ToList();
                    ;

                    var camp = _campService.Get(id);

                    foreach (var campRoomOption in result)
                    {
                        campRoomOption.Camp = camp;
                    }

                    return result;
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
