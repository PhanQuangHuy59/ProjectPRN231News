using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using WebNewsAPIs.Dtos;

namespace WebNewsClients.Controllers
{
	[Route("Article")]
	public class ArticleController : Controller
	{
		private readonly ILogger<ArticleController> _logger;
		private HttpClient _httpClient;
		public ArticleController(HttpClient client, ILogger<ArticleController> logger)
		{
			this._logger = logger;
			this._httpClient = client;
		}
		
		public IActionResult Index()
		{
			return View();
		}
		[Route("ArticleOfCategory/{categoryArticleId}.html")]
		public IActionResult ArticleOfCategory([FromRoute]Guid categoryArticleId)
		{
            //Call api của Category
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&$filter=ParentCategory eq null & orderby=OrderLevel";

            var responseMessage = _httpClient.GetAsync(urlOdataAllCategory).Result;
            responseMessage.EnsureSuccessStatusCode();
            var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>().Result.data;
            //
            string urlCallApiCategoryOfArticle = $"https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&$filter=CategoryId eq {categoryArticleId} & orderby=OrderLevel";
            var responseMessageCallApiCategoryOfArticle = _httpClient.GetAsync(urlCallApiCategoryOfArticle).Result;
            responseMessageCallApiCategoryOfArticle.EnsureSuccessStatusCode();
            var category = responseMessageCallApiCategoryOfArticle.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
				.Result.data.ToList();
			if(category.Count == 0)
			{
                TempData["message"] = "Đường dẫn truy câp không hợp lệ ! ";
                return RedirectToAction(actionName: "Error400", controllerName: "Home");
            }
			var category1 = category[0];

            //Call api tim cacs article theo loai cua bai bao
            string urlCallApiArticles = $"https://localhost:7251/odata/Articles?$expand=Categorty,AuthorNavigation&$top=10&$orderby=CreatedDate desc &$filter=IsPublish eq true and StatusProcess eq 3 and CategortyId eq {categoryArticleId}";
            var responseMessageCallApiArticles = _httpClient.GetAsync(urlCallApiArticles).Result;
            responseMessageCallApiArticles.EnsureSuccessStatusCode();
            var articles = responseMessageCallApiArticles.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Article>>>()
                .Result.data.ToList();

			//


            //tra ve view
            ViewBag.Category = listCategories;
			ViewBag.CategoryDetail = category1;
			ViewBag.Articles = articles;
            return View();
		}

		[HttpGet("{slug}.html")]
		public IActionResult ArticleDetail([FromRoute] string slug)
		{
			//Call api của Category
			string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&$filter=ParentCategory eq null & orderby=OrderLevel";

			var responseMessage = _httpClient.GetAsync(urlOdataAllCategory).Result;
			responseMessage.EnsureSuccessStatusCode();
			var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>().Result.data;


			//Call api tim article theo slug
			string urlOdataOfArticleBySlug = $"https://localhost:7251/odata/Articles?$expand=Categorty,AuthorNavigation&$top=1&$filter=IsPublish eq true and StatusProcess eq 3 and Slug eq '{slug}'";
			var responseMessageArticle = _httpClient.GetAsync(urlOdataOfArticleBySlug).Result;
			responseMessageArticle.EnsureSuccessStatusCode();
			var article = responseMessageArticle.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Article>>>()
				.Result.data.ToList();

			if (article.Count > 0)
			{
				var article1 = article[0];
				//Call api tim cacs article theo loai cua bai bao
				string urlCallApiArticles = $"https://localhost:7251/odata/Articles?$expand=Categorty,AuthorNavigation&$top=10&$orderby=CreatedDate desc &$filter=IsPublish eq true and StatusProcess eq 3 and CategortyId eq {article1.CategortyId}";
				var responseMessageCallApiArticles = _httpClient.GetAsync(urlCallApiArticles).Result;
				responseMessageCallApiArticles.EnsureSuccessStatusCode();
				var articles = responseMessageCallApiArticles.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Article>>>()
					.Result.data.ToList();

				string urlCallApiCategoryOfArticle = $"https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&$filter=CategoryId eq {article1.CategortyId} & orderby=OrderLevel";
				var responseMessageCallApiCategoryOfArticle = _httpClient.GetAsync(urlCallApiCategoryOfArticle).Result;
				responseMessageCallApiCategoryOfArticle.EnsureSuccessStatusCode();
				var category = responseMessageCallApiCategoryOfArticle.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
					.Result.data.ToList();
				// Call api comment of Article 
				string urlCallApiComment = $"https://localhost:7251/odata/Comments?$expand=ReplyForNavigation,User,UserIdReplyNavigation,InverseReplyForNavigation($expand=User,InverseReplyForNavigation)&$filter=ArticleId eq {article1.ArticleId}";
				var responseMessageCallApiComment =  _httpClient.GetAsync(urlCallApiComment).Result;
				responseMessageCallApiComment.EnsureSuccessStatusCode();
				var articlesComment = responseMessageCallApiComment.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Comment>>>()
					.Result.data.ToList();



				//Call api tim cacs article theo loai cua bai bao
				string urlCallApiArticlesForRecomment = $"https://localhost:7251/api/Articles/GetAllArticleOfAllCategory?categoryId={category[0].CategoryId}";
				var responseMessageCallApiArticlesForRecomment = _httpClient.GetAsync(urlCallApiArticlesForRecomment).Result;
				responseMessageCallApiArticlesForRecomment.EnsureSuccessStatusCode();
				var articlesRecomment = responseMessageCallApiArticlesForRecomment.Content.ReadFromJsonAsync<List<ViewArticleDto>>()
					.Result.ToList();

				List<ViewArticleDto> listArticlesRecommend = new List<ViewArticleDto>();
				List<int> checkExist = new List<int>();
				int count = 0;
				while (count < 3)
				{
					int positionRandom = (int)new Random().NextInt64(0, articlesRecomment.Count - 1);
					if (!checkExist.Contains(positionRandom))
					{
						listArticlesRecommend.Add(articlesRecomment[positionRandom]);
						checkExist.Add(positionRandom);
						count++;
					}
				}


				ViewBag.Category = listCategories;
				ViewBag.Article = article1;
				ViewBag.ListArticle = articles;
				ViewBag.ListArticleRecomend = listArticlesRecommend;
				ViewBag.CategoryOfArticle = category[0];
				ViewBag.Comments = articlesComment;
				return View();
			}

			TempData["message"] = "Không tìm thấy bài báo nào! ";
			return RedirectToAction(actionName: "Error400", controllerName: "Home");
		}
	}
}
