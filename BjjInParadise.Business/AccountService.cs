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
    public class AccountService : BaseCrudService<User>
    {
        private readonly BjjInParadiseContext _context;
        private string _connectionString;

        public AccountService(BjjInParadiseContext context)
        {
            _context = context;
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public override IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<User> Get(string id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var result = await db.QueryAsync<User>("SELECT TOP 1 * From ApplicationUsers where [AspNetUserId] = @AspNetUserId", new { AspNetUserId = id});

                    return result.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
                throw;
            }
         
        }

        public override User Get(int? id)
        {
            if (id == null)
                return null;
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var result = db.Query<User>("SELECT TOP 1 * From ApplicationUsers where [UserId] = {0}", id);

                    return result.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
                throw;
            }
    
        }

        public override Task<User> UpdateAsync(User t)
        {
            throw new NotImplementedException();
        }

        protected override async Task<User> Add(User user)
        {
           

            try
            {
                _context.ApplicationUsers.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
            }

            return user;
        }

        public override Task  DeleteAsync(User t)
        {
            throw new NotImplementedException();
        }
    }
}
