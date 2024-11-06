using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IDropEmotionRepository : IRepository<DropEmotion>
    {
        Task<DropEmotion> AddDropEmotion(DropEmotion dropEmotion);
        Task<DropEmotion> UpdateDropEmotion(DropEmotion dropEmotion);

    }
    public class DropEmotionRepository : RepositoryBase<DropEmotion>, IDropEmotionRepository
    {
        public DropEmotionRepository(FinalProjectPRN231Context context) : base(context)
        {
        }

        public async Task<DropEmotion> AddDropEmotion(DropEmotion dropEmotion)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response = await _context.DropEmotions.AddAsync(dropEmotion);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }
        public async Task<DropEmotion> UpdateDropEmotion(DropEmotion dropEmotion)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response =  _context.DropEmotions.Update(dropEmotion);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }
    }
}
