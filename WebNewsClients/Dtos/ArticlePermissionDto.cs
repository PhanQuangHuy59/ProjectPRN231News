using System.ComponentModel.DataAnnotations;

namespace WebNewsClients.Dtos
{
    public class AddArticlePermissionDto
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid ArticleId { get; set; }
        [Required]
        public int PermissionType { get; set; }
    }
    public class UpdateArticlePermissionDto
    {
        [Required]
        public long ApermissionId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid ArticleId { get; set; }
        [Required]
        public int PermissionType { get; set; }
    }
    public class ViewArticlePermissionDto
    {
        public long ApermissionId { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public Guid ArticleId { get; set; }
        public string ArticleName { get; set; }
        public int PermissionType { get; set; }
        public string PermissionTypeName { get; set; } // Thêm trường này để hiển thị tên của quyền
        public DateTime CreatedDate { get; set; }
    }


}
