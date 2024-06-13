using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ICategoriesArticleRepository : IRepository<CategoriesArticle>
    {
        
    }
    public class CategoriesArticleRepository :  RepositoryBase<CategoriesArticle>, ICategoriesArticleRepository
    {
        public CategoriesArticleRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
       
    }
}
