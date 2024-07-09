using BusinessObjects.Models;
using System.ComponentModel.DataAnnotations;

namespace WebNewsAPIs.Dtos
{
    public class AddArticleDto
    {
        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;

        [Required]
        public Guid Author { get; set; }       


        [Required]
        public bool IsPublish { get; set; }

        public int StatusProcess { get; set; }

        [Required]
        public Guid CategortyId { get; set; }

        [Required, MaxLength(300)]
        public string ShortDescription { get; set; } = null!;


        public string? CoverImage { get; set; } = null!;


        [Url]
        public string? LinkAudio { get; set; }
    }
    public class UpdateArticleDto
    {
        [Required]
        public Guid ArticleId { get; set; }
        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;

        [Required]
        public Guid Author { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public DateTime? PublishDate { get; set; }

        [Required]
        public bool IsPublish { get; set; }

        public int StatusProcess { get; set; }

        [Required]
        public Guid CategortyId { get; set; }

        [Required, StringLength(300)]
        public string ShortDescription { get; set; } = null!;


        [Required]
        public string? CoverImage { get; set; } = null!;

        [Required]
        public Guid Processor { get; set; }

        [Url]
        public string? LinkAudio { get; set; }
		public int? ViewArticles { get; set; }
	}
    public class ViewArticleDto
    {
        public Guid ArticleId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public Guid Author { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public bool IsPublish { get; set; }
        public int StatusProcess { get; set; }
        public Guid CategortyId { get; set; }
        public string CategortyName { get; set; }
        public string ShortDescription { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string CoverImage { get; set; } = null!;
        public Guid Processor { get; set; }
        public string ProcessorName { get; set; } = null!;
        public string? LinkAudio { get; set; }
		public int? ViewArticles { get; set; }
		public virtual ICollection<ViewCommentDto> Comments { get; set; }
		
	}

}
