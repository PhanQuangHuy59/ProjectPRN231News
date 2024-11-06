using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IArticlePermissionRepository : IRepository<ArticlePermission>
    {
        Task<ArticlePermission> AddArticlePermission(ArticlePermission articlePermission);
        Task<ArticlePermission> UpdateArticlePermission(ArticlePermission articlePermission);
    }
    public class ArticlePermissionRepository :  RepositoryBase<ArticlePermission>, IArticlePermissionRepository
    {
        public ArticlePermissionRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
        public async Task<ArticlePermission> AddArticlePermission(ArticlePermission articlePermission)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response = await _context.ArticlePermissions.AddAsync(articlePermission);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }
        public async Task<ArticlePermission> UpdateArticlePermission(ArticlePermission articlePermission)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response =  _context.ArticlePermissions.Update(articlePermission);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }
    }
}
