using BusinessObjects.Models;
using Repositories;

namespace WebNewsAPIs.Services
{

	public class ArticleService
	{
		private readonly ICategoriesArticleRepository _category;
		private readonly IArticleRepository _articleRepository;
		public ArticleService(ICategoriesArticleRepository category, IArticleRepository articleRepository)
		{
			_category = category;
			_articleRepository = articleRepository;
		}

		public Dictionary<string, List<Article>> GetAllArticlesOfRootCategory() {
			string[] includesCategory = new string[]
			{
				nameof(CategoriesArticle.ParentCategory), 
			};
			string[] includesArticle = new string[]
			{
				nameof(Article.AuthorNavigation),
				nameof(Article.Categorty),
				nameof(Article.Comments)
			};
			var rootCategoryArticle = _category.GetMulti(c => true, includesCategory).ToList();
			var allArticle = _articleRepository.GetMulti(c => c.IsPublish == true && c.StatusProcess == 3, includesArticle)
				.OrderBy(c => c.CreatedDate).OrderBy(c => c.ViewArticles)
				.ToList();

			Dictionary<string, List<Article>> articleOfRooCategory = new Dictionary<string, List<Article>>();	

			foreach (var article in allArticle)
			{
				var cateofArticle = rootCategoryArticle.FirstOrDefault(c => c.CategoryId == article.CategortyId);
				if (cateofArticle != null)
				{
					var root = getRootOfCategory(cateofArticle).CategoryName;
					if (articleOfRooCategory.ContainsKey(root))
					{
						articleOfRooCategory[root].Add(article);
					}
					else
					{
						var newListArticle = new List<Article>();
						newListArticle.Add(article);
						articleOfRooCategory[root] = newListArticle;
					}
				}

			}


			return articleOfRooCategory;
		}

		public CategoriesArticle getRootOfCategory(CategoriesArticle category)
		{
			while(category.ParentCategory != null)
			{
				category = category.ParentCategory;
			}
			return category;
		}

		
	}
}
