namespace WebNewsAPIs.Dtos
{
    public class AddDropEmotionDto
    {
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }
        public DateTime DropDate { get; set; }
        public Guid EmotionId { get; set; }
    }
    public class UpdateDropEmotionDto
    {
        public long DropEmotionId { get; set; }
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }
        public DateTime DropDate { get; set; }
        public Guid EmotionId { get; set; }
    }
    public class ViewDropEmotionDto
    {
        public long DropEmotionId { get; set; }
        public Guid ArticleId { get; set; }
        public string ArticleName { get; set; } // Thêm trường này để hiển thị tiêu đề của bài viết
        public Guid UserId { get; set; }
        public string UserName { get; set; } // Thêm trường này để hiển thị tên của người dùng
        public DateTime DropDate { get; set; }
        public Guid EmotionId { get; set; }
        public string EmotionName { get; set; } // Thêm trường này để hiển thị tên của cảm xúc
    }



}
