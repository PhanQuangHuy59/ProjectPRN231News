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
        [HttpGet("{slug}.html")]
        public IActionResult ArticleDetail([FromRoute]string slug)
        {
			//Call api của Category
			string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&$filter=ParentCategory eq null & orderby=OrderLevel";

			var responseMessage = _httpClient.GetAsync(urlOdataAllCategory).Result;
			responseMessage.EnsureSuccessStatusCode();
            var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>().Result.data;


            //Call api tim article theo slug
			string urlOdataOfArticleBySlug= $"https://localhost:7251/odata/Articles?$expand=Categorty,AuthorNavigation&$top=1&$filter=IsPublish eq true and StatusProcess eq 3 and Slug eq '{slug}'";
			var responseMessageArticle = _httpClient.GetAsync(urlOdataOfArticleBySlug).Result;
			responseMessageArticle.EnsureSuccessStatusCode();
			var article = responseMessageArticle.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Article>>>()
                .Result.data.ToList();

            if(article.Count > 0)
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
				ViewBag.Category = listCategories;
				ViewBag.Article = article1;
				ViewBag.ListArticle = articles;
				ViewBag.CategoryOfArticle = category[0];

                return View();
			}

            TempData["message"] = "Không tìm thấy bài báo nào! ";
            return RedirectToAction(actionName: "Error400", controllerName: "Home");
        }
        public IActionResult ArticleOfCategory(Guid categoryArticleId)
        {
            return View();
        }
    }
}
