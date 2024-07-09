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
        [UserLoginAuthorize ]
        public IActionResult PersonalInformationOfUser()
        {
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true & UserId eq {userLogin.UserId}";
            var response = _httpClient.GetAsync(urlRegister).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse = response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>().Result.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }
            ViewBag.InformationUser = userResponse[0];

            return View();
        }


        public IActionResult PersonalArticleComment(int? currentPage = 1)
        {
            //Lay thong tin tai khoan khi nguoi dung dang nhap
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true & UserId eq {userLogin.UserId}";
            var response = _httpClient.GetAsync(urlRegister).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse = response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>().Result.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }

            var urlListComment = $"https://localhost:7251/odata/Comments?$filter=UserId eq {userLogin.UserId}&$expand=Article,User&$orderby=CreateDate desc";
            var responseListComments = _httpClient.GetAsync(urlListComment).Result;
            if (!responseListComments.IsSuccessStatusCode)
            {
                TempData["err"] = "Đã xảy ra lỗi trong quá trình truy cập dữ liệu";
                return View();
            }
            var listComment = responseListComments.Content.ReadFromJsonAsync<OdataResponse<List<Comment>>>().Result.data.ToList();
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

        public IActionResult ArticleViewed(int? currentPage = 1)
        {

            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true & UserId eq {userLogin.UserId}";
            var response = _httpClient.GetAsync(urlRegister).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse = response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>().Result.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }

            //Lấy ra danh sách các bài báo mà người dùng đã xem gần nhất


            var urlListArticleView = $"https://localhost:7251/odata/Views?$expand=Article&filter=UserId eq {userLogin.UserId}&$orderby=ViewDate desc";
            var responseListViewed = _httpClient.GetAsync(urlListArticleView).Result;
            if (!responseListViewed.IsSuccessStatusCode)
            {
                TempData["err"] = "Đã xảy ra lỗi trong quá trình truy cập dữ liệu";
                return View();
            }
            var listArticleViewed = responseListViewed.Content.ReadFromJsonAsync<OdataResponse<List<View>>>()
                .Result.data.OrderByDescending(c => c.ViewDate.Date).ToList();


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


        public IActionResult BoardArticleOfUser(int? currentPage = 1)
        {

            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true & UserId eq {userLogin.UserId}";
            var response = _httpClient.GetAsync(urlRegister).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse = response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>().Result.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }

            //Lấy ra danh sách các bài báo mà người dùng đã xem gần nhất


            var urlListArticleView = $"https://localhost:7251/api/Articles/GetBoardArticleOfUser?userId={userLogin.UserId}";
            var responseListViewed = _httpClient.GetAsync(urlListArticleView).Result;
            if (!responseListViewed.IsSuccessStatusCode)
            {
                TempData["err"] = "Đã xảy ra lỗi trong quá trình truy cập dữ liệu";
                return View();
            }
            var listArticleViewed = responseListViewed.Content.ReadFromJsonAsync<List<ViewArticleDto>>()
                .Result.OrderByDescending(c => c.CreatedDate.Date).ToList();


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
        public IActionResult ArticleSaved(int? currentPage = 1)
        {
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true & UserId eq {userLogin.UserId}";
            var response = _httpClient.GetAsync(urlRegister).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse = response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>().Result.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }

            //Lấy ra danh sách các bài báo mà người dùng đã xem gần nhất


            var urlListArticleView = $"https://localhost:7251/odata/SaveArticles?$expand=Article&filter=UserId eq {userLogin.UserId}&$orderby=SaveDate desc";
            var responseListViewed = _httpClient.GetAsync(urlListArticleView).Result;
            if (!responseListViewed.IsSuccessStatusCode)
            {
                TempData["err"] = "Đã xảy ra lỗi trong quá trình truy cập dữ liệu";
                return View();
            }
            var listArticleViewed = responseListViewed.Content.ReadFromJsonAsync<OdataResponse<List<SaveArticle>>>()
                .Result.data.OrderByDescending(c => c.SaveDate.Date).ToList();


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
        public IActionResult GetAllArticleOfUser(int? processId, Guid? categoryId, string keySearch = "", int? currentPage = 1)
        {
            string guid_Default = "00000000-0000-0000-0000-000000000000";
            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessage = _httpClient.GetAsync(urlOdataAllCategory).Result;
            responseMessage.EnsureSuccessStatusCode();
            var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
                .Result.data;


            //Call api của Process Status
            string urlOdataAllProessStatus = "https://localhost:7251/odata/ProcessStatuss";
            var responseMessageProessStatus = _httpClient.GetAsync(urlOdataAllProessStatus).Result;
            responseMessageProessStatus.EnsureSuccessStatusCode();
            var listProessStatus = responseMessageProessStatus.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<ProcessStatus>>>()
                .Result.data;



            //lấy thông tin của người dùng khi đăng nhập
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            string urlRegister = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true & UserId eq {userLogin.UserId}";
            var response = _httpClient.GetAsync(urlRegister).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse = response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>().Result.data;
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
            string urlSearch = $"https://localhost:7251/api/Articles/SearchArticleOfUser?categoryId={categoryId}&keySearch={keySearch}&authorId={userLogin.UserId}" +
                $"&currentPage={currentPage}&size={Page_Item}&processId={(processId == int.MinValue ? null : processId)}";

            var httpMessage = new HttpRequestMessage(HttpMethod.Get, urlSearch);
            var responseMessageArticleSearch = _httpClient.SendAsync(httpMessage).Result;
            responseMessageArticleSearch.EnsureSuccessStatusCode();
            var responseData = responseMessageArticleSearch.Content.ReadFromJsonAsync<SearchPaging<IEnumerable<ViewArticleDto>>>().Result;

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
        public IActionResult AddArticle()
        {
            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessage = _httpClient.GetAsync(urlOdataAllCategory).Result;
            responseMessage.EnsureSuccessStatusCode();
            var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
                .Result.data;
            SelectList selectListCategory = new SelectList(listCategories, nameof(CategoriesArticle.CategoryId), nameof(CategoriesArticle.CategoryName));
            //
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true & UserId eq {userLogin.UserId}";
            var response = _httpClient.GetAsync(urlRegister).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse = response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>().Result.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }


            ViewBag.InformationUser = userResponse[0];
            ViewBag.SelectListCategory = selectListCategory;
            AddArticleDto add = new AddArticleDto();
            return View(add);
        }
        [HttpPost("AddNewArticle.html")]
        [ArticlesAuthorize]
        public IActionResult AddArticlePost(AddArticleDto? addArticle)
        {
            TempData.Clear();
            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessage = _httpClient.GetAsync(urlOdataAllCategory).Result;
            responseMessage.EnsureSuccessStatusCode();
            var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
                .Result.data;
            SelectList selectListCategory = new SelectList(listCategories, nameof(CategoriesArticle.CategoryId), nameof(CategoriesArticle.CategoryName));
            //
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister1 = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true & UserId eq {userLogin.UserId}";
            var response1 = _httpClient.GetAsync(urlRegister1).Result;
            if (!response1.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse = response1.Content.ReadFromJsonAsync<OdataResponse<List<User>>>().Result.data;
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
            string urlRegister = "https://localhost:7251/api/Articles/AddNewArticle";
            var request = new HttpRequestMessage(HttpMethod.Post, urlRegister);

            request.Content = new StringContent(JsonConvert.SerializeObject(addArticle), Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", "Bearer " + token);
            var response = _httpClient.SendAsync(request).Result;


            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = response.Content.ReadAsStringAsync().Result;
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
        public IActionResult EditArticle(Guid? articleId)
        {
            TempData.Clear();
            // Thong tin user
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister1 = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true & UserId eq {userLogin.UserId}";
            var response1 = _httpClient.GetAsync(urlRegister1).Result;
            if (!response1.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse = response1.Content.ReadFromJsonAsync<OdataResponse<List<User>>>().Result.data;
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
            string urlGetArticle = $"https://localhost:7251/api/Articles/GetArticleById/{articleId}";
            var responseMessage = _httpClient.GetAsync(urlGetArticle).Result;

            if (!responseMessage.IsSuccessStatusCode)
            {
                var error = responseMessage.Content.ReadAsStringAsync().Result;
                TempData["error"] = error;
                return RedirectToAction("GetAllArticleOfUser", "PersonalInformation");
            }
            var editArticle = responseMessage.Content.ReadFromJsonAsync<ViewArticleDto>()
                .Result;
            if(editArticle == null)
            {
                var error = "Không thể tìm thấy được Article của bạn.";
                TempData["error"] = error;
                return RedirectToAction("GetAllArticleOfUser", "PersonalInformation");
            }
            var updateForView = _mapper.Map<UpdateArticleDto>(editArticle);
           // Dánh sách các thể loại
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessageCategory = _httpClient.GetAsync(urlOdataAllCategory).Result;
            responseMessageCategory.EnsureSuccessStatusCode();
            var listCategories = responseMessageCategory.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
                .Result.data;
            SelectList selectListCategory = new SelectList(listCategories, nameof(CategoriesArticle.CategoryId), nameof(CategoriesArticle.CategoryName), editArticle.CategortyId);
            //

            ViewBag.SelectListCategory = selectListCategory;
            ViewBag.InformationUser = userResponse[0];
            return View(updateForView);
        }



        [HttpPost("EditArticle.html")]
        [ArticlesAuthorize]
        public IActionResult EditArticlePost(UpdateArticleDto? updateArticleDto)
        {
            TempData.Clear();
            // Thong tin user
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister1 = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true & UserId eq {userLogin.UserId}";
            var response1 = _httpClient.GetAsync(urlRegister1).Result;
            if (!response1.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse = response1.Content.ReadFromJsonAsync<OdataResponse<List<User>>>().Result.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }
            // Dánh sách các thể loại
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessageCategory = _httpClient.GetAsync(urlOdataAllCategory).Result;
            responseMessageCategory.EnsureSuccessStatusCode();
            var listCategories = responseMessageCategory.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
                .Result.data;
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
            string urlEidtArticle = $"https://localhost:7251/api/Articles/UpdateArticle/{updateArticleDto.ArticleId}";
            var request = new HttpRequestMessage(HttpMethod.Put, urlEidtArticle);
            request.Content = new StringContent(JsonConvert.SerializeObject(updateArticleDto), Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", "Bearer " + token);
            var response = _httpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = response.Content.ReadAsStringAsync().Result;

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


    }
}
