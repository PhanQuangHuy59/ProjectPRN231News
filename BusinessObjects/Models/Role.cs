using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public Guid RoleId { get; set; }
        public string Rolename { get; set; } = null!;
        public DateTime Createddate { get; set; }
        public DateTime? Updateddate { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
