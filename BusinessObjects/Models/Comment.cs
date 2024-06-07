using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Comment
    {
        public Comment()
        {
            InverseReplyForNavigation = new HashSet<Comment>();
        }

        public long CommentId { get; set; }
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public long? Likes { get; set; }
        public long? Dislikes { get; set; }
        public long? ReplyFor { get; set; }
        public Guid? UserIdReply { get; set; }

        public virtual Article Article { get; set; } = null!;
        public virtual Comment? ReplyForNavigation { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual User? UserIdReplyNavigation { get; set; }
        public virtual ICollection<Comment> InverseReplyForNavigation { get; set; }
    }
}
