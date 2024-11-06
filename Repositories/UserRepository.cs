using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> AddUser(User user);
         User UpdateUser(User user);
    }
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
        public async Task<User> AddUser(User user)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response = await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }

        public User UpdateUser(User user)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response =  _context.Users.Update(user);
                 _context.SaveChanges();
                return response.Entity;
            }
        }
    }
}
