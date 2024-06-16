using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class ArticlePermission
    {
        public long ApermissionId { get; set; }
        public Guid UserId { get; set; }
        public Guid ArticleId { get; set; }
        public int PermissionType { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Article Article { get; set; } = null!;
        public virtual Permission PermissionTypeNavigation { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
