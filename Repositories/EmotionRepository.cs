using AccessDatas;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IEmotionRepository :IRepository<Emotion>
    {
        Task<Emotion> AddEmotion(Emotion emotion);
        Task<Emotion> UpdateEmotion(Emotion emotion);
        Task<Emotion> DeleteEmotion(Guid emotionId);
    }
    public class EmotionRepository :  RepositoryBase<Emotion>, IEmotionRepository
    {
        public EmotionRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
        public async Task<Emotion> AddEmotion(Emotion emotion)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response = await _context.Emotions.AddAsync(emotion);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }

        public async Task<Emotion> DeleteEmotion(Guid emotionId)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response = await _context.Emotions.FirstOrDefaultAsync(c => c.EmotionId.Equals(emotionId));
                if(response != null)
                {
                    var dropEmotion =  _context.DropEmotions.Where(c => c.EmotionId.Equals(emotionId));
                    if(dropEmotion.Any())
                    {
                        _context.DropEmotions.RemoveRange(dropEmotion);
                        await _context.SaveChangesAsync();
                    }
                    _context.Emotions.Remove(response);
                    await _context.SaveChangesAsync();
                    return response;
                }
               
                return null;
            }
        }

        public async Task<Emotion> UpdateEmotion(Emotion emotion)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
               
                var response =  _context.Emotions.Update(emotion);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }

    }
}
