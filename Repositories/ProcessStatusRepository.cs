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
        Task<ProcessStatus> AddProcessStatus(ProcessStatus process);
        Task<ProcessStatus> UpdateProcessStatus(ProcessStatus process);
    }
    public class ProcessStatusRepository : RepositoryBase<ProcessStatus>, IProcessStatusRepository
    {
        public ProcessStatusRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
        public async Task<ProcessStatus> AddProcessStatus(ProcessStatus process)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response = await _context.ProcessStatuses.AddAsync(process);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }

        public async Task<ProcessStatus> UpdateProcessStatus(ProcessStatus process)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response =  _context.ProcessStatuses.Update(process);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }
    }
}
