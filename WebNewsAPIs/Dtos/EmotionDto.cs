using BusinessObjects.Models;
using System.ComponentModel.DataAnnotations;

namespace WebNewsAPIs.Dtos
{
    public class EmotionUpdateDto
    {
        [Required]
        public Guid EmotionId { get; set; }
        [Required]
        [StringLength(255, ErrorMessage = "Emotion name cannot exceed 255 characters.")]
        public string NameEmotion { get; set; } = null!;

        [Required]
        [Url]
        public string Image { get; set; } = null!;
    }
    public class EmotionCreateDto
    {
        [Required]
        [StringLength(255, ErrorMessage = "Emotion name cannot exceed 255 characters.")]
        public string NameEmotion { get; set; } = null!;

        [Required]
        [Url]
        public string Image { get; set; } = null!;
    }

public class EmotionViewDto
    {
        public Guid EmotionId { get; set; }
        public string NameEmotion { get; set; } = null!;
        public string Image { get; set; } = null!;
    }
}
