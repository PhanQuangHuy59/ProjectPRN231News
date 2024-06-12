﻿using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IEmotionRepository :IRepository<Emotion>
    {

    }
    public class EmotionRepository :  RepositoryBase<Emotion>, IEmotionRepository
    {
        public EmotionRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
       
    }
}
