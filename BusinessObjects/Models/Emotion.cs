using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Emotion
    {
        public Emotion()
        {
            DropEmotions = new HashSet<DropEmotion>();
        }

        public Guid EmotionId { get; set; }
        public string NameEmotion { get; set; } = null!;
        public string Image { get; set; } = null!;

        public virtual ICollection<DropEmotion> DropEmotions { get; set; }
    }
}
