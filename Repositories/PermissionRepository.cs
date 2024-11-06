using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        public Task<Permission> AddPermission(Permission permission);
        public Task<Permission> UpdatePermission(Permission permission);
    }
    public class PermissionRepository : RepositoryBase<Permission>, IPermissionRepository
    {
        public PermissionRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
        public async Task<Permission> AddPermission(Permission permission)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response = await _context.Permissions.AddAsync(permission);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }
        public async Task<Permission> UpdatePermission(Permission permission)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response =  _context.Permissions.Update(permission);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }
    }
}
