using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AccessDatas
{
    public abstract class RepositoryBase<T> : IDisposable, IRepository<T> where T : class
    {
        #region Properties
        protected FinalProjectPRN231Context dataContext;
        protected readonly DbSet<T> dbSet;


        protected FinalProjectPRN231Context FinalDbContext
        {
            get { return dataContext ?? (dataContext = new FinalProjectPRN231Context()); }
        }
        #endregion

        protected RepositoryBase(FinalProjectPRN231Context context)
        {
            dataContext = context ?? throw new ArgumentNullException(nameof(context));
            dbSet = dataContext.Set<T>();
        }

        #region Implementation
        public async virtual Task<T> AddAsync(T entity)
        {
            var entry = await dbSet.AddAsync(entity);
            SaveAsync();
            return entry.Entity;
        }

        public async virtual Task UpdateAsync(T entity)
        {
            dbSet.Attach(entity);
            dataContext.Entry(entity).State = EntityState.Modified;
            SaveAsync();
        }

        public async virtual Task<T> DeleteAsync(T entity)
        {
            T temp = dbSet.Remove(entity).Entity;
            SaveAsync();
            return temp;
        }
        public async virtual Task<T> DeleteAsync(int id)
        {
            var entity = await dbSet.FindAsync(id);
            T temp = dbSet.Remove(entity).Entity;
            SaveAsync();
            return temp;
        }
        public async virtual Task DeleteMultiAsync(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = dbSet.Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
                dbSet.Remove(obj);
            SaveAsync();
        }

        public async virtual Task<T> GetSingleByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async virtual Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> where, string includes)
        {
            return await dbSet.Include(includes)
                .Where(where).ToListAsync();
        }


        public async virtual Task<int> CountAsync(Expression<Func<T, bool>> where)
        {
            return await dbSet.CountAsync(where);
        }

        public IEnumerable<T> GetAll(string[] includes = null)
        {
            //HANDLE INCLUDES FOR ASSOCIATED OBJECTS IF APPLICABLE
            if (includes != null && includes.Count() > 0)
            {
                var query = dataContext.Set<T>().Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
                return query;
            }

            return dataContext.Set<T>();
        }

        public async Task<T> GetSingleByCondition(Expression<Func<T, bool>> expression, string[] includes = null)
        {
            if (includes != null && includes.Count() > 0)
            {

                var query = dataContext.Set<T>().Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
                return  query.FirstOrDefaultAsync(expression).Result;
            }
              
            return dataContext.Set<T>().FirstOrDefaultAsync(expression).Result;
        }

        public virtual IEnumerable<T> GetMulti(Expression<Func<T, bool>> predicate, string[] includes = null)
        {
            //HANDLE INCLUDES FOR ASSOCIATED OBJECTS IF APPLICABLE
            if (includes != null && includes.Count() > 0)
            {
                var query = dataContext.Set<T>().Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
                return query.Where<T>(predicate).AsQueryable<T>();
            }

            return dataContext.Set<T>().Where<T>(predicate).AsQueryable<T>();
        }

        public virtual IEnumerable<T> GetMultiPaging(Expression<Func<T, bool>> predicate, out int total, int index = 0, int size = 20, string[] includes = null)
        {
            int skipCount = index * size;
            IQueryable<T> _resetSet;

            //HANDLE INCLUDES FOR ASSOCIATED OBJECTS IF APPLICABLE
            if (includes != null && includes.Count() > 0)
            {
                var query = dataContext.Set<T>().Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
                _resetSet = predicate != null ? query.Where<T>(predicate).AsQueryable() : query.AsQueryable();
            }
            else
            {
                _resetSet = predicate != null ? dataContext.Set<T>().Where<T>(predicate).AsQueryable() : dataContext.Set<T>().AsQueryable();
            }

            _resetSet = skipCount == 0 ? _resetSet.Take(size) : _resetSet.Skip(skipCount).Take(size);
            total = _resetSet.Count();
            return _resetSet.AsQueryable();
        }

        public async Task<bool> CheckContainsAsync(Expression<Func<T, bool>> predicate)
        {
            return await dataContext.Set<T>().CountAsync<T>(predicate) > 0;
        }

        public async Task SaveAsync()
        {
            await dataContext.SaveChangesAsync();
        }
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dataContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        
        #endregion
    }
}
