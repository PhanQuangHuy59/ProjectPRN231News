using BusinessObjects.Models;
using System.ComponentModel.DataAnnotations;

namespace WebNewsAPIs.Dtos
{

    public class ProcessStatusUpdateDto
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

    public class ProcessStatusCreateDto
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
    public class ProcessStatusViewDto
    {
        public ProcessStatusViewDto()
        {
            Articles = new HashSet<Article>();
        }
        [Key]
        public int ProcessId { get; set; }
        public string NameProcess { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int RoleForProcess { get; set; }

        public virtual ICollection<Article> Articles { get; set; }
    }
}
