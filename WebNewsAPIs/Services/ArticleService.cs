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

        public Dictionary<string, List<Article>> GetAllArticlesOfRootCategory()
        {
            string[] includesCategory = new string[]
            {
                nameof(CategoriesArticle.InverseParentCategory)
            };
            string[] includesArticle = new string[]
            {
                nameof(Article.AuthorNavigation),
                nameof(Article.Categorty),
                nameof(Article.Comments)
            };
            var rootCategoryArticle = _category.GetMulti(c => true, includesCategory).ToList();
            var allArticle = _articleRepository.GetMulti(c => c.IsPublish  && c.StatusProcess == 3, includesArticle)
                .OrderByDescending(c => c.ViewArticles).ThenByDescending(c => c.CreatedDate)
                .ToList();

            Dictionary<string, List<Article>> articleOfRooCategory = new Dictionary<string, List<Article>>();

            foreach (var article in allArticle)
            {

                var root = getRootOfCategory(rootCategoryArticle, article.Categorty).CategoryName;
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


            return articleOfRooCategory;
        }

        public CategoriesArticle getRootOfCategory(List<CategoriesArticle> listCategoryArticles, CategoriesArticle category)
        {
            if (category == null || category.ParentCategoryId == null)
            {
                return category;
            }

            var parentCategory = listCategoryArticles.FirstOrDefault(c => c.CategoryId == category.ParentCategoryId);
            return getRootOfCategory(listCategoryArticles, parentCategory);
        }


    }
}
