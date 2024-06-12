namespace WebNewsClients.Dtos
{
    public class AddViewDto
    {
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }
        public DateTime ViewDate { get; set; }
    }
    public class UpdateViewDto
    {
        public long ViewId { get; set; }
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }
        public DateTime ViewDate { get; set; }
    }
    public class ViewViewDto
    {
        public long ViewId { get; set; }
        public Guid ArticleId { get; set; }
        public string ArticleName { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public DateTime ViewDate { get; set; }
    }

}
