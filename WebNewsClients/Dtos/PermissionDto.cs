using System.ComponentModel.DataAnnotations;

namespace WebNewsClients.Dtos
{
    public class AddPermissionDto
    {
        [Required]
        [StringLength(300, ErrorMessage = "Permission name cannot exceed 300 characters.")]
        public string PermissionName { get; set; } = null!;

        public string? Description { get; set; } = null!;


    }
    public class UpdatePermissionDto
    {
        public int PermissionId { get; set; }
        [Required]
        [StringLength(300, ErrorMessage = "Permission name cannot exceed 300 characters.")]
        public string PermisstionName { get; set; } = null!;
        public string? Description { get; set; } = null!;
    }


    public class ViewPermissionDto
    {
        public int PermissionId { get; set; }
        public string PermisstionName { get; set; } = null!;
        public string? Description { get; set; }
    }

}
