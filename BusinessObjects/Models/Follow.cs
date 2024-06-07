using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Follow
    {
        public Guid FollowId { get; set; }
        public Guid FollowIdBy { get; set; }
        public DateTime FollowDate { get; set; }

        public virtual User FollowIdByNavigation { get; set; } = null!;
        public virtual User FollowNavigation { get; set; } = null!;
    }
}
