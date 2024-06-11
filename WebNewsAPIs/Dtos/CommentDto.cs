using BusinessObjects.Models;
using System.ComponentModel.DataAnnotations;

namespace WebNewsAPIs.Dtos
{
    public class CommentUpdateDto
    {
        [Key]
        public long CommentId { get; set; }

        [Required]
        public Guid ArticleId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Content { get; set; } = null!;


        [Range(0, long.MaxValue, ErrorMessage = "Likes must be a non-negative number.")]
        public long? Likes { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "Dislikes must be a non-negative number.")]
        public long? Dislikes { get; set; }

        public long? ReplyFor { get; set; }

        public Guid? UserIdReply { get; set; }
    }

    public class CommentCreateDto
    {
        [Key]
        public long CommentId { get; set; }

        [Required]
        public Guid ArticleId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters.")]
        public string Content { get; set; } = null!;

        [Range(0, long.MaxValue, ErrorMessage = "Likes must be a non-negative number.")]
        public long? Likes { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "Dislikes must be a non-negative number.")]
        public long? Dislikes { get; set; }

        public long? ReplyFor { get; set; }

        public Guid? UserIdReply { get; set; }
    }


public class CommentViewDto
    {
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
