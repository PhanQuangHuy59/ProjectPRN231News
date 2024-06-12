using BusinessObjects.Models;
using System.ComponentModel.DataAnnotations;

namespace WebNewsAPIs.Dtos
{

    public class AddCategoriesArticleDto
    {

        [Required]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "Category name must be between 1 and 300 characters.")]
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public Guid? ParentCategoryId { get; set; }

        [Range(0, 1000, ErrorMessage = "Order level must be between 0 and 100.")]
        public int OrderLevel { get; set; }
    }
    public class UpdateCategoriesArticleDto
    {
        
        public Guid CategoryId { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "Category name must be between 1 and 300 characters.")]
        public string CategoryName { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid? ParentCategoryId { get; set; }

        [Range(0, 1000, ErrorMessage = "Order level must be between 0 and 100.")]
        public int OrderLevel { get; set; }
    }

    public class ViewCategoriesArticleDto
    {
       
        public Guid CategoryId { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "Category name must be between 1 and 300 characters.")]
        public string CategoryName { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }

        [Range(0, 1000, ErrorMessage = "Order level must be between 0 and 100.")]
        public int OrderLevel { get; set; }
        public virtual ICollection<ViewCategoriesArticleDto> InverseParentCategory { get; set; }
    }
}
