using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IArticlePermissionRepository
    {

    }
    public class ArticlePermissionRepository :  RepositoryBase<ArticlePermission>, IArticleRepository
    {
        public ArticlePermissionRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
       
    }
}
