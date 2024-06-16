using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Permission
    {
        public Permission()
        {
            ArticlePermissions = new HashSet<ArticlePermission>();
        }

        public int PermissionId { get; set; }
        public string PermisstionName { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<ArticlePermission> ArticlePermissions { get; set; }
    }
}
