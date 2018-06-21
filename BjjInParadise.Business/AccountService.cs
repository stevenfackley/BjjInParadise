using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BjjInParadise.Core.Models;
using BjjInParadise.Data;

namespace BjjInParadise.Business
{
    public class AccountService : BaseCrudService<User>
    {
        private readonly BjjInParadiseContext _context;

        public AccountService(BjjInParadiseContext context)
        {
            _context = context;

        }

        public override IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public override User Get(int? id)
        {
            if (id == null)
                return null;
            return _context.ApplicationUsers.Find(id);
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
