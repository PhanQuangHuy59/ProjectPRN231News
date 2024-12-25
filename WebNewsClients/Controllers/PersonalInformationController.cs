using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using WebNewsAPIs.Common;
using WebNewsAPIs.Dtos;
using WebNewsClients.Ultis;
using System.Text;
using AutoMapper;
using NuGet.Common;

namespace WebNewsClients.Controllers
{
    public class PersonalInformationController : Controller
    {
        private HttpClient _httpClient;
        private IMapper _mapper;

        private static int Page_Item = 15;
        public PersonalInformationController(HttpClient httpClient,
            IMapper mapper)
        {
            _httpClient = httpClient;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("PersonalInformation.html")]
        [UserLoginAuthorize]
        public async Task<IActionResult> PersonalInformationOfUser()
        {
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:8080/odata/Users?$expand=Role&$filter=IsConfirm eq true and UserId eq {userLogin.UserId}";
            var response = await _httpClient.GetAsync(urlRegister);
            if (!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse1 = await response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>();
            var userResponse = userResponse1.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }
            ViewBag.InformationUser = userResponse[0];

            return View();
        }


        public async Task<IActionResult> PersonalArticleComment(int? currentPage = 1)
        {
            //Lay thong tin tai khoan khi nguoi dung dang nhap
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:8080/odata/Users?$expand=Role&$filter=IsConfirm eq true and UserId eq {userLogin.UserId}";
            var response = await _httpClient.GetAsync(urlRegister);
            if (!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse1 = await response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>();
            var userResponse = userResponse1.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }

            var urlListComment = $"https://localhost:8080/odata/Comments?$filter=UserId eq {userLogin.UserId}&$expand=Article,User&$orderby=CreateDate desc";
            var responseListComments = await _httpClient.GetAsync(urlListComment);
            if (!responseListComments.IsSuccessStatusCode)
            {
                TempData["err"] = "Đã xảy ra lỗi trong quá trình truy cập dữ liệu";
                return View();
            }
            var listComment1 = await responseListComments.Content.ReadFromJsonAsync<OdataResponse<List<Comment>>>();
            var listComment = listComment1.data.ToList();
            int totalPage = (int)(Math.Ceiling((decimal)listComment.Count / Page_Item));
            var listResponse = listComment.Skip((currentPage.Value - 1) * Page_Item).Take(Page_Item).ToList();

            //Tra ve phia view
            ViewBag.InformationUser = userResponse[0];
            ViewBag.Comments = listResponse;
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPage = totalPage;
            ViewBag.TotalResult = listComment.Count;
            return View();
        }

        public async Task<IActionResult> ArticleViewed(int? currentPage = 1)
        {

            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:8080/odata/Users?$expand=Role&$filter=IsConfirm eq true and UserId eq {userLogin.UserId}";
            var response = await _httpClient.GetAsync(urlRegister);
            if (!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse1 = await response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>();
            var userResponse = userResponse1.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }

            //Lấy ra danh sách các bài báo mà người dùng đã xem gần nhất


            var urlListArticleView = $"https://localhost:8080/odata/Views?$expand=Article&filter=UserId eq {userLogin.UserId}&$orderby=ViewDate desc";
            var responseListViewed = await _httpClient.GetAsync(urlListArticleView);
            if (!responseListViewed.IsSuccessStatusCode)
            {
                TempData["err"] = "Đã xảy ra lỗi trong quá trình truy cập dữ liệu";
                return View();
            }
            var listArticleViewed1 = await responseListViewed.Content.ReadFromJsonAsync<OdataResponse<List<View>>>();
            var listArticleViewed = listArticleViewed1.data.OrderByDescending(c => c.ViewDate.Date).ToList();


            int totalPage = (int)(Math.Ceiling((decimal)listArticleViewed.Count / Page_Item));
            var listResponse = listArticleViewed.Skip((currentPage.Value - 1) * Page_Item).Take(Page_Item).ToList();

            //Tra ve phia view




            //Tra ve phia view
            ViewBag.InformationUser = userResponse[0];
            ViewBag.ArticleViews = listResponse;
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPage = totalPage;
            ViewBag.TotalResult = listArticleViewed.Count;

            return View();
        }


        public async Task<IActionResult> BoardArticleOfUser(int? currentPage = 1)
        {

            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:8080/odata/Users?$expand=Role&$filter=IsConfirm eq true and UserId eq {userLogin.UserId}";
            var response = await _httpClient.GetAsync(urlRegister);
            if (!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse1 = await response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>();
            var userResponse = userResponse1.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }

            //Lấy ra danh sách các bài báo mà người dùng đã xem gần nhất


            var urlListArticleView = $"https://localhost:8080/api/Articles/GetBoardArticleOfUser?userId={userLogin.UserId}";
            var responseListViewed = await _httpClient.GetAsync(urlListArticleView);
            if (!responseListViewed.IsSuccessStatusCode)
            {
                TempData["err"] = "Đã xảy ra lỗi trong quá trình truy cập dữ liệu";
                return View();
            }
            var listArticleViewed1 = await responseListViewed.Content.ReadFromJsonAsync<List<ViewArticleDto>>();
            var listArticleViewed = listArticleViewed1.OrderByDescending(c => c.CreatedDate.Date).ToList();


            int totalPage = (int)(Math.Ceiling((decimal)listArticleViewed.Count / Page_Item));
            var listResponse = listArticleViewed.Skip((currentPage.Value - 1) * Page_Item).Take(Page_Item).ToList();

            //Tra ve phia view

            //Tra ve phia view
            ViewBag.InformationUser = userResponse[0];
            ViewBag.ArticleViews = listResponse;
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPage = totalPage;
            ViewBag.TotalResult = listArticleViewed.Count;

            return View();
        }
        public async Task<IActionResult> ArticleSaved(int? currentPage = 1)
        {
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:8080/odata/Users?$expand=Role&$filter=IsConfirm eq true and UserId eq {userLogin.UserId}";
            var response = await _httpClient.GetAsync(urlRegister);
            if (!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse1 = await response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>();
            var userResponse = userResponse1.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }

            //Lấy ra danh sách các bài báo mà người dùng đã xem gần nhất


            var urlListArticleView = $"https://localhost:8080/odata/SaveArticles?$expand=Article&filter=UserId eq {userLogin.UserId}&$orderby=SaveDate desc";
            var responseListViewed = await _httpClient.GetAsync(urlListArticleView);
            if (!responseListViewed.IsSuccessStatusCode)
            {
                TempData["err"] = "Đã xảy ra lỗi trong quá trình truy cập dữ liệu";
                return View();
            }
            var listArticleViewed1 = await responseListViewed.Content.ReadFromJsonAsync<OdataResponse<List<SaveArticle>>>();
            var listArticleViewed = listArticleViewed1.data.OrderByDescending(c => c.SaveDate.Date).ToList();


            int totalPage = (int)(Math.Ceiling((decimal)listArticleViewed.Count / Page_Item));
            var listResponse = listArticleViewed.Skip((currentPage.Value - 1) * Page_Item).Take(Page_Item).ToList();

            //Tra ve phia view
            ViewBag.InformationUser = userResponse[0];
            ViewBag.ArticleSaves = listResponse;
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPage = totalPage;
            ViewBag.TotalResult = listArticleViewed.Count;


            return View();
        }
        // sẽ thực hiện các chức năng liên quan đến quản lý các bài viêt nếu người đó là Article

        [HttpGet("ArticleOfUser.html")]
        [ArticlesAuthorize]
        public async Task<IActionResult> GetAllArticleOfUser(int? processId, Guid? categoryId, string keySearch = "", int? currentPage = 1)
        {
            string guid_Default = "00000000-0000-0000-0000-000000000000";
            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:8080/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessage = await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessage.EnsureSuccessStatusCode();
            var listCategories1 = await responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
            var listCategories = listCategories1.data;


            //Call api của Process Status
            string urlOdataAllProessStatus = "https://localhost:8080/odata/ProcessStatuss";
            var responseMessageProessStatus = await _httpClient.GetAsync(urlOdataAllProessStatus);
            responseMessageProessStatus.EnsureSuccessStatusCode();
            var listProessStatus1 = await responseMessageProessStatus.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<ProcessStatus>>>();
            var listProessStatus = listProessStatus1.data;



            //lấy thông tin của người dùng khi đăng nhập
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            string urlRegister = $"https://localhost:8080/odata/Users?$expand=Role&$filter=IsConfirm eq true and UserId eq {userLogin.UserId}";
            var response = await _httpClient.GetAsync(urlRegister);
            if (!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse1 = await response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>();
            var userResponse = userResponse1.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }


            if (categoryId == null)
            {
                categoryId = Guid.Parse(guid_Default);
            }
            if (processId == null)
            {
                processId = int.MinValue;
            }

            // Các bài báo của author
            string urlSearch = $"https://localhost:8080/api/Articles/SearchArticleOfUser?categoryId={categoryId}&keySearch={keySearch}&authorId={userLogin.UserId}" +
                $"&currentPage={currentPage}&size={Page_Item}&processId={(processId == int.MinValue ? null : processId)}";

            var httpMessage = new HttpRequestMessage(HttpMethod.Get, urlSearch);
            var responseMessageArticleSearch = await _httpClient.SendAsync(httpMessage);
            responseMessageArticleSearch.EnsureSuccessStatusCode();
            var responseData = await responseMessageArticleSearch.Content.ReadFromJsonAsync<SearchPaging<IEnumerable<ViewArticleDto>>>();

            //Hiện thị danh sach category để filter
            var tempListCategory = listCategories.ToList();
            tempListCategory.Insert(0, new CategoriesArticle { CategoryId = Guid.Parse(guid_Default), CategoryName = "Tất Cả" });
            SelectList selectListCategory = new SelectList(tempListCategory, nameof(CategoriesArticle.CategoryId), nameof(CategoriesArticle.CategoryName), categoryId);
            //Hiện thị danh sach category để filter
            var tempListProcessStatus = listProessStatus.ToList();
            tempListProcessStatus.Insert(0, new ProcessStatus { ProcessId = int.MinValue, NameProcess = "Tất Cả" });
            SelectList selectListProcessStatus = new SelectList(tempListProcessStatus, nameof(ProcessStatus.ProcessId), nameof(ProcessStatus.NameProcess), processId);

            // phan trang 
            ViewBag.CurrentPage = currentPage;
            ViewBag.KeySearch = keySearch;
            ViewBag.CategoryId = categoryId;
            ViewBag.ProcessId = processId;
            ViewBag.TotalResultCount = responseData.total;
            ViewBag.TotalPage = (int)(Math.Ceiling((decimal)responseData.total / Page_Item));
            ViewBag.SelectListCategory = selectListCategory;
            ViewBag.SelectListProcessStatus = selectListProcessStatus;
            ViewBag.InformationUser = userResponse[0];
            //
            ViewBag.ArticleOfAuthor = responseData.result;

            return View();
        }
        // 
        [HttpGet("AddNewArticle.html")]
        [ArticlesAuthorize]
        public async Task<IActionResult> AddArticle()
        {
            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:8080/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessage = await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessage.EnsureSuccessStatusCode();
            var listCategories1 = await responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
            var listCategories = listCategories1.data;
            SelectList selectListCategory = new SelectList(listCategories, nameof(CategoriesArticle.CategoryId), nameof(CategoriesArticle.CategoryName));
            //
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:8080/odata/Users?$expand=Role&$filter=IsConfirm eq true and UserId eq {userLogin.UserId}";
            var response = await _httpClient.GetAsync(urlRegister);
            if (!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse1 = await response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>();
            var userResponse = userResponse1.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }


            ViewBag.InformationUser = userResponse[0];
            ViewBag.SelectListCategory = selectListCategory;
            AddArticleDto add = new AddArticleDto();
            add.CoverImage = "/images/anh_banner_default.jpg";
            add.StatusProcess = 1;
            return View(add);
        }
        [HttpPost("AddNewArticle.html")]
        [ArticlesAuthorize]
        public async Task<IActionResult> AddArticlePost(AddArticleDto? addArticle)
        {
            TempData.Clear();
            addArticle.StatusProcess = 1;
            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:8080/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessage = await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessage.EnsureSuccessStatusCode();
            var listCategories1 = await responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
            var listCategories = listCategories1.data;
            SelectList selectListCategory = new SelectList(listCategories, nameof(CategoriesArticle.CategoryId), nameof(CategoriesArticle.CategoryName));
            //
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister1 = $"https://localhost:8080/odata/Users?$expand=Role&$filter=IsConfirm eq true and UserId eq {userLogin.UserId}";
            var response1 = await _httpClient.GetAsync(urlRegister1);
            if (!response1.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse1 = await response1.Content.ReadFromJsonAsync<OdataResponse<List<User>>>();
            var userResponse = userResponse1.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }

            string token = HttpContext.Request.Cookies[SaveKeySystem.Authentication];
            if (!ModelState.IsValid)
            {
                var errorMessages = new List<string>();
                foreach (var err in ModelState.Keys)
                {
                    var state = ModelState[err];
                    if (state != null && state.Errors.Count > 0)
                    {
                        errorMessages.Add($"{err}: {state.Errors[0].ErrorMessage}");
                    }
                }
                ModelState.AddModelError(string.Empty, string.Join("\n", errorMessages));
                TempData["err"] = "Dữ liệu có vẻ không hợp lệ";
                ViewBag.InformationUser = userResponse[0];
                ViewBag.SelectListCategory = selectListCategory;
                return View("AddArticle", addArticle);
            }
            // call api them bài báo
            string urlRegister = "https://localhost:8080/api/Articles/AddNewArticle";
            var request = new HttpRequestMessage(HttpMethod.Post, urlRegister);

            request.Content = new StringContent(JsonConvert.SerializeObject(addArticle), Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", "Bearer " + token);
            var response = await _httpClient.SendAsync(request);


            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ViewBag.InformationUser = userResponse[0];
                ViewBag.SelectListCategory = selectListCategory;
                TempData["err"] = $"Không thể tạo bài báo hãy thử lại \n {errorMessage}";
                return View("AddArticle", addArticle);
            }

            TempData["success"] = "Đã tạo bài báo thành công Hãy chờ đề Quản trị viên chấp nhận bài báo của bạn";

            ViewBag.InformationUser = userResponse[0];
            ViewBag.SelectListCategory = selectListCategory;
            return View("AddArticle", addArticle);
        }



        [HttpGet("EditArticle/{articleId}.html")]
        [ArticlesAuthorize]
        public async Task<IActionResult> EditArticle(Guid? articleId)
        {
            TempData.Clear();
            // Thong tin user
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:8080/odata/Users?$expand=Role&$filter=IsConfirm eq true and UserId eq {userLogin.UserId}";
            var response1 = await _httpClient.GetAsync(urlRegister);
            if (!response1.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse1 = await response1.Content.ReadFromJsonAsync<OdataResponse<List<User>>>();
            var userResponse = userResponse1.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }



            if (articleId == null)
            {
                return RedirectToAction("GetAllArticleOfUser");
            }
            // get article
            string urlGetArticle = $"https://localhost:8080/api/Articles/GetArticleById/{articleId}";
            var responseMessage = await _httpClient.GetAsync(urlGetArticle);

            if (!responseMessage.IsSuccessStatusCode)
            {
                var error = await responseMessage.Content.ReadAsStringAsync();
                TempData["error"] = "Đã xảy ra lỗi" + error;
                return RedirectToAction("GetAllArticleOfUser", "PersonalInformation");
            }
            var editArticle = await responseMessage.Content.ReadFromJsonAsync<ViewArticleDto>()
                ;
            if (editArticle == null)
            {
                var error = "Không thể tìm thấy được Article của bạn.";
                TempData["error"] = "Đã xảy ra lỗi" + error;
                return RedirectToAction("GetAllArticleOfUser", "PersonalInformation");
            }
            var updateForView = _mapper.Map<UpdateArticleDto>(editArticle);
            // Dánh sách các thể loại
            string urlOdataAllCategory = "https://localhost:8080/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessageCategory = await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessageCategory.EnsureSuccessStatusCode();
            var listCategories1 = await responseMessageCategory.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
            var listCategories = listCategories1.data;
            SelectList selectListCategory = new SelectList(listCategories, nameof(CategoriesArticle.CategoryId), nameof(CategoriesArticle.CategoryName), editArticle.CategortyId);
            //

            ViewBag.SelectListCategory = selectListCategory;
            ViewBag.InformationUser = userResponse[0];
            return View(updateForView);
        }



        [HttpPost("EditArticle.html")]
        [ArticlesAuthorize]
        public async Task<IActionResult> EditArticlePost(UpdateArticleDto? updateArticleDto)
        {
            TempData.Clear();
            // Thong tin user
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:8080/odata/Users?$expand=Role&$filter=IsConfirm eq true and UserId eq {userLogin.UserId}";
            var response1 = await _httpClient.GetAsync(urlRegister);
            if (!response1.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse1 = await response1.Content.ReadFromJsonAsync<OdataResponse<List<User>>>();
            var userResponse = userResponse1.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }
            // Dánh sách các thể loại
            string urlOdataAllCategory = "https://localhost:8080/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessageCategory = await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessageCategory.EnsureSuccessStatusCode();
            var listCategories1 = await responseMessageCategory.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
            var listCategories = listCategories1.data;
            SelectList selectListCategory = new SelectList(listCategories, nameof(CategoriesArticle.CategoryId), nameof(CategoriesArticle.CategoryName), updateArticleDto.CategortyId);
            //

            string token = HttpContext.Request.Cookies[SaveKeySystem.Authentication];
            if (!ModelState.IsValid)
            {
                var errorMessages = new List<string>();
                foreach (var err in ModelState.Keys)
                {
                    var state = ModelState[err];
                    if (state != null && state.Errors.Count > 0)
                    {
                        errorMessages.Add($"{err}: {state.Errors[0].ErrorMessage}");
                    }
                }
                ModelState.AddModelError(string.Empty, string.Join("\n", errorMessages));
                TempData["err"] = "Dữ liệu có vẻ không hợp lệ";
                ViewBag.SelectListCategory = selectListCategory;
                ViewBag.InformationUser = userResponse[0];
                return View("EditArticle", updateArticleDto);
            }
            // call api them bài báo
            string urlEidtArticle = $"https://localhost:8080/api/Articles/UpdateArticle/{updateArticleDto.ArticleId}";
            var request = new HttpRequestMessage(HttpMethod.Put, urlEidtArticle);
            request.Content = new StringContent(JsonConvert.SerializeObject(updateArticleDto), Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", "Bearer " + token);
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                TempData["err"] = $"Không thể cập nhật bài báo hãy thử lại \n {errorMessage}";
                ViewBag.SelectListCategory = selectListCategory;
                ViewBag.InformationUser = userResponse[0];
                return View("EditArticle", updateArticleDto);
            }

            ViewBag.SelectListCategory = selectListCategory;
            ViewBag.InformationUser = userResponse[0];
            TempData["success"] = "Đã tạo cập nhật bài báo thành công.";
            return View("EditArticle", updateArticleDto);
        }

        [HttpGet("ConfirmDeleteArticle.html")]
        [ArticlesAuthorize]
        public async Task<IActionResult> ConfirmDeleteArticle(Guid? articleId)
        {

            TempData.Clear();
            // Thong tin user
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:8080/odata/Users?$expand=Role&$filter=IsConfirm eq true and UserId eq {userLogin.UserId}";
            var response1 = await _httpClient.GetAsync(urlRegister);
            if (!response1.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse1 = await response1.Content.ReadFromJsonAsync<OdataResponse<List<User>>>();
            var userResponse = userResponse1.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }



            if (articleId == null)
            {
                return RedirectToAction("GetAllArticleOfUser");
            }
            // get article
            string urlGetArticle = $"https://localhost:8080/api/Articles/GetArticleById/{articleId}";
            var responseMessage = await _httpClient.GetAsync(urlGetArticle);

            if (!responseMessage.IsSuccessStatusCode)
            {
                var error = await responseMessage.Content.ReadAsStringAsync();
                TempData["error"] = "Đã xảy ra lỗi" + error;
                return RedirectToAction("GetAllArticleOfUser", "PersonalInformation");
            }
            var editArticle = await responseMessage.Content.ReadFromJsonAsync<ViewArticleDto>()
                ;
            if (editArticle == null)
            {
                var error = "Không thể tìm thấy được Article của bạn.";
                TempData["error"] = "Đã xảy ra lỗi" + error;
                return RedirectToAction("GetAllArticleOfUser", "PersonalInformation");
            }
            // Dánh sách các thể loại

            ViewBag.InformationUser = userResponse[0];
            return View(editArticle);
        }
        [HttpPost("DeleteArticle.html")]
        [ArticlesAuthorize]
        public async Task<IActionResult> DeleteArticlePost(Guid? articleId)
        {
            string token = HttpContext.Request.Cookies[SaveKeySystem.Authentication];
            string urlDeleteArticle = $"https://localhost:8080/api/Articles/DeleteArticle/{articleId.Value}";
            var request = new HttpRequestMessage(HttpMethod.Delete, urlDeleteArticle);
            request.Headers.Add("Authorization", "Bearer " + token);
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                TempData["success"] = "Đã xảy ra lỗi trong quá trình xóa";
                return RedirectToAction("GetAllArticleOfUser");
            }
            TempData["success"] = "Đã xóa thành công bài viết.";
            return RedirectToAction("GetAllArticleOfUser");
        }



    }
}
