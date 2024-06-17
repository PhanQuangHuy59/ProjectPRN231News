using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
	public interface ICategoriesArticleRepository : IRepository<CategoriesArticle>
	{
		public List<Guid> getAllParentIDChildrentID(Guid parentId);
		public CategoriesArticle getRootOfCategory(Guid categoryID);
	}
	public class CategoriesArticleRepository : RepositoryBase<CategoriesArticle>, ICategoriesArticleRepository
	{
		public CategoriesArticleRepository(FinalProjectPRN231Context context) : base(context)
		{
		}

		public List<Guid> getAllParentIDChildrentID(Guid parentId)
		{
			string[] includes = new string[]
			{
				nameof(CategoriesArticle.ParentCategory),
				nameof(CategoriesArticle.InverseParentCategory)
			};
			var category = GetSingleByCondition(c => c.CategoryId == parentId, includes).Result;
			var listId = new List<Guid>();
			var getParent = category;
			var getChildren = category;
			//get parent
			while(getParent.ParentCategory != null)
			{
				getParent = getParent.ParentCategory;
				listId.Add(getParent.CategoryId);
			}
			getChildrenId(getChildren.InverseParentCategory, ref listId);

			return listId;
		}
		public void getChildrenId(ICollection<CategoriesArticle> list, ref List<Guid> listId)
		{
			foreach(var category in list)
			{
				listId.Add(category.CategoryId);
				if(category.ParentCategory != null)
				{
					getChildrenId(list, ref listId);
				}
			}
		}

		public CategoriesArticle getRootOfCategory(Guid categoryID)
		{
			string[] includes = new string[]
			{
				nameof(CategoriesArticle.ParentCategory),
			};

			var category = GetSingleByCondition(c => c.CategoryId.Equals(categoryID), includes).Result;
			if(category == null)
			{
				return null;
			}
			//get parent
			while (category.ParentCategory != null)
			{
				category = category.ParentCategory;
			}
			return category;
		}
	}
}
