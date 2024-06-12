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

    }
    public class SaveArticleRepository : RepositoryBase<SaveArticle>, ISaveArticleRepository
    {
        public SaveArticleRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
    }
}
