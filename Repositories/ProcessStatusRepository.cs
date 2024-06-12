using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IProcessStatusRepository : IRepository<ProcessStatus>
    {

    }
    public class ProcessStatusRepository : RepositoryBase<ProcessStatus>, IProcessStatusRepository
    {
        public ProcessStatusRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
    }
}
