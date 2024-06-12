
using System.ComponentModel.DataAnnotations;

namespace WebNewsClients.Dtos
{
    public class AddEmotionDto
    {
        [Required]
        [StringLength(255, ErrorMessage = "Emotion name cannot exceed 255 characters.")]
        public string NameEmotion { get; set; } = null!;

        [Required]
        [Url]
        public string Image { get; set; } = null!;
    }

    public class UpdateEmotionDto
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


    public class ViewEmotionDto
    {
        public Guid EmotionId { get; set; }
        public string NameEmotion { get; set; } = null!;
        public string Image { get; set; } = null!;
    }
}
