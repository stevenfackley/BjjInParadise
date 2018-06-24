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

        public CampService(BjjInParadiseContext context)
        {
            _context = context;
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
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
                return await db.QueryAsync<Camp>("SELECT  * FROM Camp where IsActive = 1 order by StartDate");

            }

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

                    var result = await db.ExecuteAsync(insertQuery, t);
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
            var camp =  _context.Camps.Find(id);
            return camp;
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


                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var result =
                        await db.QueryAsync<Camp>("Select top 1 * From Camp where IsActive = 1 order by StartDate");

                    return result.FirstOrDefault();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
