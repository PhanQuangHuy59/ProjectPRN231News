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

    }
    public class FollowRepository : RepositoryBase<Follow>, IFollowRepository
    {
        public FollowRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
    }
}
