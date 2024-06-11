using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Article
    {
        public Article()
        {
            ArticlePermissions = new HashSet<ArticlePermission>();
            Comments = new HashSet<Comment>();
            DropEmotions = new HashSet<DropEmotion>();
            SaveArticles = new HashSet<SaveArticle>();
            Views = new HashSet<View>();
        }

        public Guid ArticleId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public Guid Author { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public bool IsPublish { get; set; }
        public int StatusProcess { get; set; }
        public Guid CategortyId { get; set; }
        public string ShortDescription { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string CoverImage { get; set; } = null!;
        public Guid Processor { get; set; }
        public string? LinkAudio { get; set; }

        public virtual User AuthorNavigation { get; set; } = null!;
        public virtual CategoriesArticle Categorty { get; set; } = null!;
        public virtual User ProcessorNavigation { get; set; } = null!;
        public virtual ProcessStatus StatusProcessNavigation { get; set; } = null!;
        public virtual ICollection<ArticlePermission> ArticlePermissions { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<DropEmotion> DropEmotions { get; set; }
        public virtual ICollection<SaveArticle> SaveArticles { get; set; }
        public virtual ICollection<View> Views { get; set; }
    }
}
