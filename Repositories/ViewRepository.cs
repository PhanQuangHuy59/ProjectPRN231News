using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IViewRepository :  IRepository<View>
    {
        void DeleteListView(IEnumerable<View> viewList);
        Task<View> AddView(View view);
        Task<View> UpdateView(View view);
    }
    public class ViewRepository : RepositoryBase<View>, IViewRepository
    {
        public ViewRepository(FinalProjectPRN231Context context) : base(context)
        {
        }

        public void DeleteListView(IEnumerable<View> viewList)
        {
            dataContext.Views.RemoveRange(viewList);
            SaveAsync();
        }
        public async Task<View> AddView(View view)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response = await _context.Views.AddAsync(view);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }
        public async Task<View> UpdateView(View view)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response =  _context.Views.Update(view);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }
    }
}
