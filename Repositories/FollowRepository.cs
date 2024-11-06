using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IFollowRepository : IRepository<Follow>
    {
        Task<Follow> AddFollow(Follow follow);
        Task<Follow> UpdateFollow(Follow follow);
    }
    public class FollowRepository : RepositoryBase<Follow>, IFollowRepository
    {
        public FollowRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
        public async Task<Follow> AddFollow(Follow follow)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response = await _context.Follows.AddAsync(follow);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }
        public async Task<Follow> UpdateFollow(Follow follow)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response =  _context.Follows.Update(follow);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }
    }
}
