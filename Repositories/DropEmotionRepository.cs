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

    }
    public class DropEmotionRepository :  RepositoryBase<DropEmotion>, IDropEmotionRepository
    {
        public DropEmotionRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
       
    }
}
