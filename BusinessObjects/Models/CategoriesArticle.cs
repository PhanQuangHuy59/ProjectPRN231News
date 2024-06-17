using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BusinessObjects.Models
{
    public partial class CategoriesArticle
    {
        public CategoriesArticle()
        {
            Articles = new HashSet<Article>();
            InverseParentCategory = new HashSet<CategoriesArticle>();
        }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public int OrderLevel { get; set; }

        public virtual CategoriesArticle? ParentCategory { get; set; }
        public virtual ICollection<Article> Articles { get; set; }
        public virtual ICollection<CategoriesArticle> InverseParentCategory { get; set; }
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			var categoryArticle = (CategoriesArticle)obj;
			return this.CategoryId == categoryArticle.CategoryId
				&& this.CategoryName == categoryArticle.CategoryName
				&& this.Description == categoryArticle.Description;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
