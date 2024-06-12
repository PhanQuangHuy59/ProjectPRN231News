using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {

    }
    public class CommentRepository :  RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
       
    }
}
