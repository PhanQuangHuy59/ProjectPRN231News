using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class SaveArticle
    {
        public long SaveId { get; set; }
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }
        public DateTime SaveDate { get; set; }

        public virtual Article Article { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
