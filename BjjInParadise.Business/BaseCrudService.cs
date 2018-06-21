using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BjjInParadise.Core.Models;

namespace BjjInParadise.Business
{
    public abstract class BaseCrudService<T> where T : BaseModel

    {
        public   async Task<T> AddAsync(T t)
        {
            UpdateCreatedAndModifiedDate(t);

            return  await Add(t);
        }

        public abstract IEnumerable<T> GetAll();
        public abstract T Get(int? id);
        public abstract Task<T> UpdateAsync(T t);
        protected abstract Task<T> Add(T t);

        public abstract Task DeleteAsync(T t);
        protected T UpdateModifiedDate(T t)
        {
            t.ModifiedDate = DateTime.UtcNow;
            
            return t;
        }
        protected  T UpdateCreatedDate(T t)
        {
            t.CreatedDate = DateTime.UtcNow;
            return t;
        }
        /// <summary>
        /// Business process to update any new entity's updated and created dates
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected T UpdateCreatedAndModifiedDate(T t)
        {
            UpdateCreatedDate(t);
            UpdateModifiedDate(t);
            return t;
        }

       
    }
}
