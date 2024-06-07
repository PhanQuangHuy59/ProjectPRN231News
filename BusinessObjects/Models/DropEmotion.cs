using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class DropEmotion
    {
        public long DropEmotionId { get; set; }
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }
        public DateTime DropDate { get; set; }
        public Guid EmotionId { get; set; }

        public virtual Article Article { get; set; } = null!;
        public virtual Emotion Emotion { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
