using BusinessObjects.Models;
using System.ComponentModel.DataAnnotations;

namespace WebNewsAPIs.Dtos
{

    public class UserUpdateDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Display name cannot exceed 255 characters.")]
        public string? DisplayName { get; set; }

        [Required]
        public Guid RoleId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [EnumDataType(typeof(GenderType), ErrorMessage = "Invalid gender.")]
        public string? Gender { get; set; }

        public string? Address { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        public string? PhoneNumber { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? Image { get; set; }

        
    }
    public class UserCreateDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Display name cannot exceed 255 characters.")]
        public string? DisplayName { get; set; }

        [Required]
        public Guid RoleId { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [EnumDataType(typeof(GenderType), ErrorMessage = "Invalid gender.")]
        public string? Gender { get; set; }

        public string? Address { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        public string? PhoneNumber { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? Image { get; set; }

        
    }

    public class UserViewDto
    {
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
    }
}
