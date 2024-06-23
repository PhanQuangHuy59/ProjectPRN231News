using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using WebNewsAPIs.Dtos;
using WebNewsClients.Ultis;

namespace WebNewsClients.Controllers
{
    [Route("Article")]
    public class ArticleController : Controller
    {
        private readonly ILogger<ArticleController> _logger;
        private HttpClient _httpClient;
        private const int Items_Page = 12;
        private const int Items_Page_Search = 18;
        private const string GuidDefault = "00000000-0000-0000-0000-000000000000";
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
        public IActionResult ArticleOfCategory([FromRoute] Guid categoryArticleId)
        {
           

            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";

            var responseMessage =  _httpClient.GetAsync(urlOdataAllCategory).Result;
            responseMessage.EnsureSuccessStatusCode();

            var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
                .Result.data;
            //
            //
            string urlCallApiCategoryOfArticle = $"https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&$filter=CategoryId eq {categoryArticleId} & orderby=OrderLevel";
            var responseMessageCallApiCategoryOfArticle = _httpClient.GetAsync(urlCallApiCategoryOfArticle).Result;
            responseMessageCallApiCategoryOfArticle.EnsureSuccessStatusCode();
            var category = responseMessageCallApiCategoryOfArticle.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
                .Result.data.ToList();
            if (category.Count == 0)
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
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessage = _httpClient.GetAsync(urlOdataAllCategory).Result;
            responseMessage.EnsureSuccessStatusCode();
            var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
                .Result.data;
            //

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
                var responseMessageCallApiComment = _httpClient.GetAsync(urlCallApiComment).Result;
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
                ViewBag.UserLogin = userLogin;
                return View();
            }

            TempData["message"] = "Không tìm thấy bài báo nào! ";
            return RedirectToAction(actionName: "Error400", controllerName: "Home");
        }


        [HttpGet("LastNew.html")]
        public IActionResult LastNew(int currentPage = 1)
        {
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&$filter=ParentCategory eq null & orderby=OrderLevel";
            var responseMessage = _httpClient.GetAsync(urlOdataAllCategory).Result;
            responseMessage.EnsureSuccessStatusCode();
            var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>().Result.data.ToList();


            //Call api của last new của ngày hôm nay
            var now = new DateTime(2024, 6, 17);
            string urlOdataLastestnew = $"https://localhost:7251/odata/Articles?$expand=Categorty,AuthorNavigation&$orderby=CreatedDate desc&$filter=IsPublish eq true and StatusProcess eq 3 and year(CreatedDate) eq {now.Year} and month(CreatedDate) eq {now.Month} and day(CreatedDate) eq {now.Day}\r\n";
            var responseMessageLastNew = _httpClient.GetAsync(urlOdataLastestnew).Result;
            responseMessageLastNew.EnsureSuccessStatusCode();
            var listLastestArticle = responseMessageLastNew.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Article>>>().Result.data.ToList();

            int totalPage = (int)Math.Ceiling((decimal)listLastestArticle.Count / Items_Page);

            listLastestArticle = listLastestArticle.Skip((currentPage - 1) * Items_Page).Take(Items_Page).ToList();




            ViewBag.Category = listCategories;
            ViewBag.LastestNew = listLastestArticle;
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPage = totalPage;
            return View();
        }

        [HttpGet("Search.html")]
        public IActionResult SearchArticleView()
        {

            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessage = _httpClient.GetAsync(urlOdataAllCategory).Result;
            responseMessage.EnsureSuccessStatusCode();
            var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
                .Result.data;


            var listDayFilter = new List<object> {
                new {value ="all",title="Tất Cả" },
                new {value ="now",title="Ngày Hôm nay" },
              new {value ="day",title="1 Ngày Trước" },
              new {value ="week",title="1 Tuần Trước" },
              new {value ="month",title="1 Tháng Trước" },
              new {value ="year",title="1 Năm Trước" }
            };

            var tempListCategory = listCategories.ToList();
            tempListCategory.Insert(0, new CategoriesArticle { CategoryId = Guid.Parse(GuidDefault), CategoryName = "Tất Cả" });
            SelectList selectListTime = new SelectList(listDayFilter, "value", "title", "all");
            SelectList selectListCategory = new SelectList(tempListCategory, "CategoryId", "CategoryName", Guid.Parse(GuidDefault));

            ViewBag.Category = listCategories;
            ViewBag.ListTime = selectListTime;
            ViewBag.ListCategory = selectListCategory;
            ViewBag.KeySearch = "";
            ViewBag.CategoryId = Guid.Parse(GuidDefault);
            ViewBag.Time = "all";

            return View("SearchArticle");
        }



        [HttpGet("SearchWithContent")]
        public IActionResult SearchArticle(int currentPage = 1, Guid? categoryId = null, string time = "all", string keySearch = "")
        {
            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessage = _httpClient.GetAsync(urlOdataAllCategory).Result;
            responseMessage.EnsureSuccessStatusCode();
            var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
                .Result.data;

            //Call api
            //
            if (currentPage == int.MinValue)
            {
                currentPage = 1;
            }

            var listDayFilter = new List<object> {
                new {value ="all",title="Tất Cả" },
                new {value ="now",title="Ngày Hôm nay" },
                new {value ="day",title="1 Ngày Trước" },
                new {value ="week",title="1 Tuần Trước" },
               new {value ="month",title="1 Tháng Trước" },
               new {value ="year",title="1 Năm Trước" }
            };



            List<Article> listArticleSearch = new List<Article>();
            DateTime toDate = DateTime.Now.Date;
            DateTime? fromDate = toDate;


            if (time == "day")
            {
                fromDate = toDate.AddDays(-1);
            }
            else if (time == "month")
            {
                fromDate = toDate.AddMonths(-1);
            }
            else if (time == "week")
            {
                fromDate = toDate.AddDays(-7);
            }
            else if (time == "now")
            {
                fromDate = toDate;
            }
            else if (time == "year")
            {
                fromDate = toDate.AddYears(-1);
            }
            else if (time == "all")
            {
                fromDate = null;
            }
            

          
           
           
           

            // Tạo select list cho thẻ selelct 

            var tempListCategory = listCategories.ToList();
            tempListCategory.Insert(0,new CategoriesArticle { CategoryId = Guid.Parse(GuidDefault), CategoryName = "Tất Cả" });
            SelectList selectListTime = new SelectList(listDayFilter, "value", "title", time);
            SelectList selectListCategory = new SelectList(tempListCategory, nameof(CategoriesArticle.CategoryId), nameof(CategoriesArticle.CategoryName), categoryId);


            // Call ApiSearch

            //Call api của Category Root
            

            string urlSearch = $"https://localhost:7251/api/Articles/SearchArticle?categoryId={categoryId}&keySearch={keySearch}" +
                $"&from={fromDate}&to={toDate}&currentPage={currentPage}&size={Items_Page_Search}";
            var httpMessage = new HttpRequestMessage(HttpMethod.Get, urlSearch);
            var responseMessageArticleSearch = _httpClient.SendAsync(httpMessage).Result;
            responseMessageArticleSearch.EnsureSuccessStatusCode();
            var responseData = responseMessageArticleSearch.Content.ReadFromJsonAsync<SearchPaging<IEnumerable<ViewArticleDto>>>().Result;



            if (categoryId == null)
            {
                categoryId = Guid.Parse(GuidDefault);
            }

            ViewBag.Category = listCategories;
            ViewBag.ListTime = selectListTime;
            ViewBag.ListCategory = selectListCategory;
            // tra ve tham so search
            ViewBag.CurrentPage = currentPage;
            ViewBag.KeySearch = keySearch;
            ViewBag.CategoryId = categoryId;
            ViewBag.Time = time;
            ViewBag.TotalResultCount = responseData.total;
            ViewBag.TotalPage = (int)((decimal)responseData.total / Items_Page_Search);

            ViewBag.ListSearchArticle = responseData.result;





            return View();
        }

    }
}
