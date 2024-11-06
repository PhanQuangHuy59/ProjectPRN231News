using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ISaveArticleRepository : IRepository<SaveArticle>
    {
        Task<SaveArticle> AddSaveArticle(SaveArticle dropEmotion);
        Task<SaveArticle> UdpatetSaveArticle(SaveArticle saveArticle);
    }
    public class SaveArticleRepository : RepositoryBase<SaveArticle>, ISaveArticleRepository
    {
        public SaveArticleRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
        public async Task<SaveArticle> AddSaveArticle(SaveArticle saveArticle)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response = await _context.SaveArticles.AddAsync(saveArticle);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }
        public async Task<SaveArticle> UdpatetSaveArticle(SaveArticle saveArticle)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response =  _context.SaveArticles.Update(saveArticle);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }

    }
}
