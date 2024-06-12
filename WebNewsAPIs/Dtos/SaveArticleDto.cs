namespace WebNewsAPIs.Dtos
{
    public class AddSaveArticleDto
    {
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }
    }
    public class UpdateSaveArticleDto
    {
        public long SaveId { get; set; }
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }
    }
    public class ViewSaveArticleDto
    {
        public long SaveId { get; set; }
        public Guid ArticleId { get; set; }
        public string ArticleName { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public DateTime SaveDate { get; set; }
    }


}
