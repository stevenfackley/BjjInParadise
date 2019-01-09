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
        private readonly string _connectionString;

        public AccountService(BjjInParadiseContext context)
        {
            _context = context;
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public override IEnumerable<User> GetAll()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var result =  db.Query<User>("SELECT  * From ApplicationUsers");

                    return result;
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
                throw;
            }
        }

        public async Task<User> GetAsync(string id)
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

        public User Get(string id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var result =  db.Query<User>("SELECT TOP 1 * From ApplicationUsers where [AspNetUserId] = @AspNetUserId", new { AspNetUserId = id });

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
                    var result = db.Query<User>("SELECT TOP 1 * From ApplicationUsers where [UserId] = @UserId",  new { UserId = id });

                    return result.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
                throw;
            }
    
        }

        public  override async Task<User> UpdateAsync(User t)
        {
            if (t == null)
                return null;
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    t.ModifiedDate =DateTime.UtcNow;
                    
                    var updateQuery =
                            @"UPDATE [dbo].[ApplicationUsers]  
                    SET
                        [FirstName] = @FirstName
                        ,[LastName] = @LastName
                        ,[Street] = @Street
                        ,[City] = @City
                        ,[State] = @State
                        ,[ZipCode] = @ZipCode
                        ,[HomeGym] = @HomeGym
                        ,[Country] = @Country
                        ,[PhoneNumber] = @PhoneNumber

                        ,[ModifiedDate] = @ModifiedDate
WHERE [AspNetUserId] = @AspNetUserId
                   "
                        ;
                    var obj = new
                    {
                        t.FirstName,
                        t.LastName,
                        t.Street,
                        t.City,
                        t.State,
                        t.ZipCode,
                        t.HomeGym,
                        t.Country,
                        t.PhoneNumber,
                        t.ModifiedDate,
                        t.AspNetUserId
                    };
                    var result = await db.ExecuteAsync(updateQuery,obj);
                    return t;
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
                throw;
            }



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

        public override  async Task  DeleteAsync(User t)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    string deleteBooking = "DELETE FROM [dbo].[Booking] WHERE UserId = @UserId ";
                    await db.ExecuteAsync(deleteBooking, new
                    {
                        UserId = t.UserId
                    });

                  

                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
            }

            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                  
                    var deleteBooking = @"DELETE FROM [dbo].[ApplicationUsers] WHERE UserId = @UserId";
                   var rresult = await db.ExecuteAsync(deleteBooking, new
                    {
                        UserId = t.UserId
                    });
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
