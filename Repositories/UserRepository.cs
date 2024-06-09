using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IUserRepository
    {

    }
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
    }
}
