using BusinessObjects.Models;
using System.ComponentModel.DataAnnotations;

namespace WebNewsAPIs.Dtos
{
    public class PermissionUpdateDto
    {
        public int PermissionId { get; set; }
        [Required]
        [StringLength(300, ErrorMessage = "Permission name cannot exceed 300 characters.")]
        public string PermisstionName { get; set; } = null!;
        public string? Description { get; set; } = null!;
    }

    public class PermissionCreateDto
    {
        [Required]
        [StringLength(300, ErrorMessage = "Permission name cannot exceed 300 characters.")]
        public string PermissionName { get; set; } = null!;

        public string? Description { get; set; } = null!;


    }
    public class PermissionViewDto
    {
        public PermissionViewDto()
        {
            ArticlePermissions = new HashSet<ArticlePermission>();
        }

        public int PermissionId { get; set; }
        public string PermisstionName { get; set; } = null!;
        public string? Description { get; set; } = null!;

        public virtual ICollection<ArticlePermission> ArticlePermissions { get; set; }
    }
}
