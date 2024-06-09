using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IViewRepository
    {

    }
    public class ViewRepository : RepositoryBase<View>, IViewRepository
    {
        public ViewRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
    }
}
