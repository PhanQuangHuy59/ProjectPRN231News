using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OData.Edm;
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
        public async Task<IActionResult> ArticleOfCategory([FromRoute] Guid categoryArticleId)
        {


            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:8080/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory & $orderby=OrderLevel";

            var responseMessage = await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessage.EnsureSuccessStatusCode();

            var listCategories1 = await responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
            var listCategories = listCategories1.data;
            //
            //
            string urlCallApiCategoryOfArticle = $"https://localhost:8080/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory & $filter=CategoryId eq {categoryArticleId} & orderby=OrderLevel";
            var responseMessageCallApiCategoryOfArticle = await _httpClient.GetAsync(urlCallApiCategoryOfArticle);
            responseMessageCallApiCategoryOfArticle.EnsureSuccessStatusCode();
            var category2 = await responseMessageCallApiCategoryOfArticle.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();

            var category = category2.data.ToList();
            if (category.Count == 0)
            {
                TempData["message"] = "Đường dẫn truy câp không hợp lệ ! ";
                return RedirectToAction(actionName: "Error400", controllerName: "Home");
            }
            var category1 = category[0];

            //Call api tim cacs article theo loai cua bai bao
            string urlCallApiArticles = $"https://localhost:8080/odata/Articles?$expand=Categorty,AuthorNavigation,Comments & $top=10 & $orderby=CreatedDate desc & $filter=IsPublish eq true and StatusProcess eq 3 and CategortyId eq {categoryArticleId}";
            var responseMessageCallApiArticles = await _httpClient.GetAsync(urlCallApiArticles);
            responseMessageCallApiArticles.EnsureSuccessStatusCode();
            var articles1 = await responseMessageCallApiArticles.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Article>>>();
            var articles = articles1.data.ToList();


            //


            //tra ve view

            ViewBag.Category = listCategories;
            ViewBag.CategoryDetail = category1;
            ViewBag.Articles = articles;
            return View();
        }

        [HttpGet("{slug}.html")]
        public async Task<IActionResult> ArticleDetail([FromRoute] string slug)
        {
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:8080/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessage = await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessage.EnsureSuccessStatusCode();
            var listCategories1 = await responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
            var listCategories = listCategories1.data;
            //

            //Call api tim article theo slug
            string urlOdataOfArticleBySlug = $"https://localhost:8080/odata/Articles?$expand=Categorty,AuthorNavigation&$top=1&$filter=IsPublish eq true and StatusProcess eq 3 and Slug eq '{slug}'";
            var responseMessageArticle = await _httpClient.GetAsync(urlOdataOfArticleBySlug);
            responseMessageArticle.EnsureSuccessStatusCode();
            var article3 = await responseMessageArticle.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Article>>>();
            var article = article3.data.ToList();

            if (article.Count > 0)
            {
                var article1 = article[0];
                //Call api tim cacs article theo loai cua bai bao
                string urlCallApiArticles = $"https://localhost:8080/odata/Articles?$expand=Categorty,AuthorNavigation&$top=10&$orderby=CreatedDate desc &$filter=IsPublish eq true and StatusProcess eq 3 and CategortyId eq {article1.CategortyId}";
                var responseMessageCallApiArticles = await _httpClient.GetAsync(urlCallApiArticles);
                responseMessageCallApiArticles.EnsureSuccessStatusCode();
                var articles2 = await responseMessageCallApiArticles.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Article>>>();
                var articles = articles2.data.ToList();

                string urlCallApiCategoryOfArticle = $"https://localhost:8080/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&$filter=CategoryId eq {article1.CategortyId} & orderby=OrderLevel";
                var responseMessageCallApiCategoryOfArticle = await _httpClient.GetAsync(urlCallApiCategoryOfArticle);
                responseMessageCallApiCategoryOfArticle.EnsureSuccessStatusCode();
                var category1 = await responseMessageCallApiCategoryOfArticle.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
                var category = category1.data.ToList();

                // Call api comment of Article 
                string urlCallApiComment = $"https://localhost:8080/odata/Comments?$expand=ReplyForNavigation,User,UserIdReplyNavigation,InverseReplyForNavigation($expand=User,InverseReplyForNavigation)&$filter=ArticleId eq {article1.ArticleId}";
                var responseMessageCallApiComment = await _httpClient.GetAsync(urlCallApiComment);
                responseMessageCallApiComment.EnsureSuccessStatusCode();
                var articlesComment1 = await responseMessageCallApiComment.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Comment>>>();
                var articlesComment = articlesComment1.data.ToList();

                // Lay tat ca emotion
                var urlEmotions = "https://localhost:8080/odata/Emotions";
                var responseMessaEmotion = await _httpClient.GetAsync(urlEmotions);
                responseMessaEmotion.EnsureSuccessStatusCode();
                var responseEmotion1 = await responseMessaEmotion.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Emotion>>>();
                var responseEmotion = responseEmotion1.data.ToList();
                //get all drop emotion of article
                var urlDropEmotionOfArticle = $"https://localhost:8080/odata/DropEmotions?$filter=ArticleId eq {article1.ArticleId}";
                var responseMessaDropEmotionOfArticle = await _httpClient.GetAsync(urlDropEmotionOfArticle);
                responseMessaDropEmotionOfArticle.EnsureSuccessStatusCode();
                var responseDropEmotion1 = await responseMessaDropEmotionOfArticle.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<DropEmotion>>>();
                var responseDropEmotion = responseDropEmotion1.data.ToList();

                // lấy ra người dung đã drop emotion cho emotion nào
                Dictionary<Guid, bool> checkDropEmotion = new Dictionary<Guid, bool>();
                Dictionary<Guid, int> quantityEachEmotion = new Dictionary<Guid, int>();


                bool checkSave = false;
                if (userLogin != null)
                {
                    // get article save of user of this article
                    var urlArticleSave = $"https://localhost:8080/odata/SaveArticles?$filter=UserId eq {userLogin.UserId} and ArticleId eq {article1.ArticleId}";
                    var responseMessagArticleSaveOfUser = await _httpClient.GetAsync(urlArticleSave);
                    responseMessagArticleSaveOfUser.EnsureSuccessStatusCode();
                    var articleSave1 = await responseMessagArticleSaveOfUser.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<SaveArticle>>>();
                    var articleSave = articleSave1.data.ToList();

                    if (articleSave.Count != 0)
                    {
                        checkSave = true;
                    }

                    //lấy ra người dùng đã thả emotion nào  
                    foreach (var emotion in responseEmotion)
                    {
                        // kiem tra xem nguoi dung co tha cam xuc cho loai cam xuc nay k
                        var check = responseDropEmotion.FirstOrDefault(c => c.UserId.Equals(userLogin.UserId)
                        && c.ArticleId.Equals(article1.ArticleId)
                        && c.EmotionId.Equals(emotion.EmotionId));
                        // lay ra so luong thả emotion của mot emotion
                        checkDropEmotion.Add(emotion.EmotionId, (check != null));
                    }
                    ViewBag.CheckDropEmotion = checkDropEmotion;
                }

                foreach (var emotion in responseEmotion)
                {
                    // lay ra so luong thả emotion của mot emotion
                    var numberDropEmotionOfEmotion = responseDropEmotion.Where(c =>
                    c.ArticleId.Equals(article1.ArticleId)
                    && c.EmotionId.Equals(emotion.EmotionId)).Count();
                    quantityEachEmotion.Add(emotion.EmotionId, numberDropEmotionOfEmotion);

                }


                //Call api tim cacs article theo loai cua bai bao
                string urlCallApiArticlesForRecomment = $"https://localhost:8080/api/Articles/GetAllArticleOfAllCategory?categoryId={category[0].CategoryId}";
                var responseMessageCallApiArticlesForRecomment = await _httpClient.GetAsync(urlCallApiArticlesForRecomment);
                responseMessageCallApiArticlesForRecomment.EnsureSuccessStatusCode();
                var articlesRecomment1 = await responseMessageCallApiArticlesForRecomment.Content.ReadFromJsonAsync<List<ViewArticleDto>>();
                var articlesRecomment = articlesRecomment1.ToList();

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
                ViewBag.CheckSave = checkSave;
                ViewBag.Emotions = responseEmotion;
                ViewBag.QuantityDropEmotion = quantityEachEmotion;
                return View();
            }

            TempData["message"] = "Không tìm thấy bài báo nào! ";
            return RedirectToAction(actionName: "Error400", controllerName: "Home");
        }


        [HttpGet("LastNew.html")]
        public async Task<IActionResult> LastNew(int currentPage = 1)
        {
            string urlOdataAllCategory = "https://localhost:8080/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&$filter=ParentCategory eq null & orderby=OrderLevel";
            var responseMessage = await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessage.EnsureSuccessStatusCode();
            var listCategories1 = await responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
            var listCategories = listCategories1.data.ToList();


            //Call api của last new của ngày hôm nay
            var now = new DateTime(2024, 6, 17);
            string urlOdataLastestnew = $"https://localhost:8080/odata/Articles?$expand=Categorty,AuthorNavigation,Comments&$orderby=CreatedDate desc&$filter=IsPublish eq true and StatusProcess eq 3 and year(CreatedDate) eq {now.Year} and month(CreatedDate) eq {now.Month} and day(CreatedDate) eq {now.Day}\r\n";
            var responseMessageLastNew = await _httpClient.GetAsync(urlOdataLastestnew);
            responseMessageLastNew.EnsureSuccessStatusCode();
            var listLastestArticle1 = await responseMessageLastNew.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Article>>>();
            var listLastestArticle = listLastestArticle1.data.ToList();

            int totalPage = (int)Math.Ceiling((decimal)listLastestArticle.Count / Items_Page);

            listLastestArticle = listLastestArticle.Skip((currentPage - 1) * Items_Page).Take(Items_Page).ToList();




            ViewBag.Category = listCategories;
            ViewBag.LastestNew = listLastestArticle;
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPage = totalPage;
            return View();
        }

        [HttpGet("Search.html")]
        public async Task<IActionResult> SearchArticleView()
        {

            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:8080/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessage =await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessage.EnsureSuccessStatusCode();
            var listCategories1 =await responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
             var listCategories = listCategories1.data;


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
        public async Task<IActionResult> SearchArticle(int currentPage = 1, Guid? categoryId = null, string time = "all", string keySearch = "")
        {
            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:8080/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessage =await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessage.EnsureSuccessStatusCode();
            var listCategories1 =await responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
             var listCategories = listCategories1.data;

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
            tempListCategory.Insert(0, new CategoriesArticle { CategoryId = Guid.Parse(GuidDefault), CategoryName = "Tất Cả" });
            SelectList selectListTime = new SelectList(listDayFilter, "value", "title", time);
            SelectList selectListCategory = new SelectList(tempListCategory, nameof(CategoriesArticle.CategoryId), nameof(CategoriesArticle.CategoryName), categoryId);


            // Call ApiSearch

            //Call api của Category Root


            string urlSearch = $"https://localhost:8080/api/Articles/SearchArticle?categoryId={categoryId}&keySearch={keySearch}" +
                $"&from={fromDate}&to={toDate}&currentPage={currentPage}&size={Items_Page_Search}";
            var httpMessage = new HttpRequestMessage(HttpMethod.Get, urlSearch);
            var responseMessageArticleSearch =await _httpClient.SendAsync(httpMessage);
            responseMessageArticleSearch.EnsureSuccessStatusCode();
            var responseData =await responseMessageArticleSearch.Content.ReadFromJsonAsync<SearchPaging<IEnumerable<ViewArticleDto>>>();



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
            ViewBag.TotalPage = (int)(Math.Ceiling((decimal)responseData.total / Items_Page_Search));

            ViewBag.ListSearchArticle = responseData.result;





            return View();
        }

    }
}
