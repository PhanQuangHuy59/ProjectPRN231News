using BusinessObjects.Models;
using System.ComponentModel.DataAnnotations;

namespace WebNewsAPIs.Dtos
{
    public class RoleUpdateDto
    {

        [Required]
        public Guid RoleId { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Role name cannot exceed 255 characters.")]
        public string Rolename { get; set; } = null!;

        [Required]
        public DateTime Createddate { get; set; }

        public DateTime? Updateddate { get; set; }

        public string? Description { get; set; }
    }
    public class RoleCreateDto
    {
        [Required]
        [StringLength(300, ErrorMessage = "Role name cannot exceed 300 characters.")]
        public string Rolename { get; set; } = null!;

        public string? Description { get; set; }
    }
    public class RoleViewDto
    {
        public RoleViewDto()
        {
            Users = new HashSet<User>();
        }

        public Guid RoleId { get; set; }
        public string Rolename { get; set; } = null!;
        public DateTime Createddate { get; set; }
        public DateTime? Updateddate { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
