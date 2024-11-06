using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role> AddRole(Role role);
        Task<Role> UpdateRole(Role role);
    }
    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        public RoleRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
        public async Task<Role> AddRole(Role role)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response = await _context.Roles.AddAsync(role);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }

        public async Task<Role> UpdateRole(Role role)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response =  _context.Roles.Update(role);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }
    }
}
