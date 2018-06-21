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

        public CampService(BjjInParadiseContext context)
        {
            _context = context;

        }


        public override IEnumerable<Camp> GetAll()
        {
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                return db.Query<Camp>("Select * From Camp").ToList();
            }

        }

        protected override async Task<Camp> Add(Camp t)
        {

            try
            {
                _context.Camps.Add(t);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
                throw;
            }

            return t;
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
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var result = await db.QueryAsync<Camp>("Select top 1 * From Camp where IsActive = 1 order by StartDate");

                return result.FirstOrDefault();
            }

          
        }
    }
}
