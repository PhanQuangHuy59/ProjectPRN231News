using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Models
{
    public partial class ProcessStatus
    {
        public ProcessStatus()
        {
            Articles = new HashSet<Article>();
        }
        [Key]
        public int ProcessId { get; set; }
        public string NameProcess { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int RoleForProcess { get; set; }

        public virtual ICollection<Article> Articles { get; set; }
    }
}
