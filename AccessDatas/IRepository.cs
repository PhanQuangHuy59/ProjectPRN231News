using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AccessDatas
{
    public interface IRepository<T> : IDisposable where T : class
    {
        // Marks an entity as new
        Task<T> AddAsync(T entity);

        // Marks an entity as modified
        Task UpdateAsync(T entity);

        // Marks an entity to be removed
        Task<T> DeleteAsync(T entity);

        Task<T> DeleteAsync(int id);

        //Delete multi records
        Task DeleteMultiAsync(Expression<Func<T, bool>> where);

        // Get an entity by int id
        Task<T> GetSingleByIdAsync(int id);

        Task<T> GetSingleByCondition(Expression<Func<T, bool>> expression, string[] includes = null);

        IEnumerable<T> GetAll(string[] includes = null);

        IEnumerable<T> GetMulti(Expression<Func<T, bool>> predicate, string[] includes = null);

        IEnumerable<T> GetMultiPaging(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50, string[] includes = null);

        Task<int> CountAsync(Expression<Func<T, bool>> where);

        Task<bool> CheckContainsAsync(Expression<Func<T, bool>> predicate);
        void Detached(T entityDetached);
        Task SaveAsync();
    }
}
