
using System.ComponentModel.DataAnnotations;

namespace WebNewsClients.Dtos
{

    public class UpdateProcessStatusDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Process ID must be a positive integer.")]
        public int ProcessId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Process name cannot exceed 100 characters.")]
        public string NameProcess { get; set; } = null!;

        [Required]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Role for process must be a positive integer.")]
        public int RoleForProcess { get; set; }

    }

    public class AddProcessStatusDto
    {
        public int ProcessId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Process name cannot exceed 100 characters.")]
        public string NameProcess { get; set; } = null!;

        [Required]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Role for process must be a positive integer.")]
        public int RoleForProcess { get; set; }

    }
    public class ViewProcessStatusDto
    {
       
        [Key]
        public int ProcessId { get; set; }
        public string NameProcess { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int RoleForProcess { get; set; }

        
    }
}
