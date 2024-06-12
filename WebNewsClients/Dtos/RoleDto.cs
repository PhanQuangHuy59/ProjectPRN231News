
using System.ComponentModel.DataAnnotations;

namespace WebNewsClients.Dtos
{
    public class AddRoleDto
    {
        [Required]
        [StringLength(300, ErrorMessage = "Role name cannot exceed 300 characters.")]
        public string Rolename { get; set; } = null!;

        public string? Description { get; set; }
    }
    public class UpdateRoleDto
    {

        [Required]
        public Guid RoleId { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Role name cannot exceed 255 characters.")]
        public string Rolename { get; set; } = null!;

        public string? Description { get; set; }
    }

    public class ViewRoleDto
    {
        public Guid RoleId { get; set; }
        public string Rolename { get; set; } = null!;
        public DateTime Createddate { get; set; }
        public DateTime? Updateddate { get; set; }
        public string? Description { get; set; }
    }

}
