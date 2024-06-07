using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class User
    {
        public User()
        {
            ArticleAuthorNavigations = new HashSet<Article>();
            ArticlePermissions = new HashSet<ArticlePermission>();
            ArticleProcessorNavigations = new HashSet<Article>();
            CommentUserIdReplyNavigations = new HashSet<Comment>();
            CommentUsers = new HashSet<Comment>();
            DropEmotions = new HashSet<DropEmotion>();
            FollowFollowIdByNavigations = new HashSet<Follow>();
            FollowFollowNavigations = new HashSet<Follow>();
            SaveArticles = new HashSet<SaveArticle>();
            Views = new HashSet<View>();
        }

        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? DisplayName { get; set; }
        public Guid Roleid { get; set; }
        public DateTime Createddate { get; set; }
        public DateTime? Updateddate { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Image { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Article> ArticleAuthorNavigations { get; set; }
        public virtual ICollection<ArticlePermission> ArticlePermissions { get; set; }
        public virtual ICollection<Article> ArticleProcessorNavigations { get; set; }
        public virtual ICollection<Comment> CommentUserIdReplyNavigations { get; set; }
        public virtual ICollection<Comment> CommentUsers { get; set; }
        public virtual ICollection<DropEmotion> DropEmotions { get; set; }
        public virtual ICollection<Follow> FollowFollowIdByNavigations { get; set; }
        public virtual ICollection<Follow> FollowFollowNavigations { get; set; }
        public virtual ICollection<SaveArticle> SaveArticles { get; set; }
        public virtual ICollection<View> Views { get; set; }
    }
}
