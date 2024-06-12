namespace WebNewsAPIs.Dtos
{
    public class AddFollowDto
    {
        public Guid FollowId { get; set; }
        public Guid FollowIdBy { get; set; }
        public DateTime FollowDate { get; set; }
    }
    public class UpdateFollowDto
    {
        public Guid FollowId { get; set; }
        public Guid FollowIdBy { get; set; }
        public DateTime FollowDate { get; set; }

    }
    public class ViewFollowDto
    {
        public Guid FollowId { get; set; }
        public string FollowIdName { get; set; }
        public Guid FollowIdBy { get; set; }
        public string FollowIdByName { get; set; } // Thêm trường này để hiển thị tên của người theo dõi
        public DateTime FollowDate { get; set; }
    }
}
