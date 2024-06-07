using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class ProcessStatus
    {
        public ProcessStatus()
        {
            Articles = new HashSet<Article>();
        }

        public int ProcessId { get; set; }
        public string NameProcess { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int RoleForProcess { get; set; }

        public virtual ICollection<Article> Articles { get; set; }
    }
}
