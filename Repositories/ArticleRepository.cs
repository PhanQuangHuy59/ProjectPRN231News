using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IArticleRepository
    {

    }
    public class ArticleRepository :  RepositoryBase<Article>, IArticleRepository
    {
        public ArticleRepository(FinalProjectPRN231Context context) : base(context)
        {
        }

       
    }
}
