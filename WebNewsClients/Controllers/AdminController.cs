using AutoMapper;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using NuGet.Common;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using WebNewsAPIs.Common;
using WebNewsAPIs.Dtos;
using WebNewsClients.Ultis;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebNewsClients.Controllers
{
    [AdminAuthorize]
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private HttpClient _httpClient;
        private const int Item_Page = 20;
        private Guid GuidDefault = Guid.Parse("00000000-0000-0000-0000-000000000000");
        private IMapper _mapper;

        public AdminController(ILogger<HomeController> logger, HttpClient httpClient, IMapper mapper)
        {
            _logger = logger;
            _httpClient = httpClient;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            string urlArticleProcessApproval = "https://localhost:7251/odata/Articles?$filter=StatusProcess eq 3";
            string urlArticleNeedApproval = "https://localhost:7251/odata/Articles?$filter=StatusProcess eq 1";
            string urlCustomerInSystem = "https://localhost:7251/odata/Users?$filter=RoleId eq 179D557C-D9CC-4504-B869-A4319792F631";
            string urlArticleInSystem = "https://localhost:7251/odata/Users?$filter=RoleId eq 90293A39-475C-44BC-8506-DAEA10E80384";
            string urlUserNewest = "https://localhost:7251/odata/Users?$expand=Role&$orderBy=Createddate desc&top=20";
            string urlArticleNewest = "https://localhost:7251/odata/Articles?$expand=Categorty,AuthorNavigation&$orderBy=CreatedDate & $top=15";
            string urlCommentNewest = "https://localhost:7251/odata/Comments?$expand=User&$top=20&$orderBy=CreateDate desc";
            string urlRole = "https://localhost:7251/odata/Roles";


            var OdatauserInSystem = await _httpClient.GetFromJsonAsync<OdataResponse<List<BusinessObjects.Models.User>>>(urlCustomerInSystem);
            var OdataarticlerInSystem = await _httpClient.GetFromJsonAsync<OdataResponse<List<BusinessObjects.Models.User>>>(urlArticleInSystem);
            var OdataarticleApproval = await _httpClient.GetFromJsonAsync<OdataResponse<List<Article>>>(urlArticleProcessApproval);
            var OdataarticleNeedApproval = await _httpClient.GetFromJsonAsync<OdataResponse<List<Article>>>(urlArticleNeedApproval);

            var OdatauserNewest = await _httpClient.GetFromJsonAsync<OdataResponse<List<BusinessObjects.Models.User>>>(urlUserNewest);
            var OdataaticleNewest = await _httpClient.GetFromJsonAsync<OdataResponse<List<Article>>>(urlArticleNewest);
            var OdatacommentNewest = await _httpClient.GetFromJsonAsync<OdataResponse<List<Comment>>>(urlCommentNewest);
            var Odatarole = await _httpClient.GetFromJsonAsync<OdataResponse<List<Role>>>(urlRole);

            ViewBag.UserInSystem = OdatauserInSystem.data.Count;
            ViewBag.ArticlerInSystem = OdataarticlerInSystem.data.Count;
            ViewBag.ArticleApproval = OdataarticleApproval.data.Count;
            ViewBag.ArticleNeedApproval = OdataarticleNeedApproval.data.Count;
            ViewBag.UserNewest = OdatauserNewest.data;
            ViewBag.ArticleNewest = OdataaticleNewest.data;
            ViewBag.CommentNewest = OdatacommentNewest.data;
            ViewBag.SelectListRoles = new SelectList(Odatarole.data, "RoleId", "Rolename");

            return View();
        }
        #region Quan ly thong tin user

        [HttpGet("AddNewAdmin.html")]
        public IActionResult AdminAdd()
        {
            TempData.Clear();
            AddUserDto addUser = new AddUserDto();
            return View(addUser);
        }
        [HttpPost("AddNewAdmin.html")]
        //[AdminAuthorize]
        public IActionResult AddNewAdmin([FromForm] AddUserDto user)
        {
            TempData.Clear();
            if (!ModelState.IsValid)
            {
                TempData["err"] = "Dữ liệu có vẻ không hợp lệ";
                return View("AdminAdd");
            }
            string token = HttpContext.Request.Cookies[SaveKeySystem.Authentication];
            string urlRegister = "https://localhost:7251/api/Users/AddAdmin";
            var request = new HttpRequestMessage(HttpMethod.Post, urlRegister);
            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", "Bearer " + token);


            var response = _httpClient.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = response.Content.ReadAsStringAsync().Result;
                TempData["err"] = $"Không thể tạo tài khoản hãy thử lại \n {errorMessage}";

                return View("UserAdd", user);
            }
            TempData["success"] = "Thêm quản lý Thành Công";
            return View("AdminAdd", user);
        }


        [HttpGet("UserManagement.html")]
        public async Task<IActionResult> UserList(string? keySearch, Guid? roleId, int currentPage = 1)
        {
            string urlOdataRole = "https://localhost:7251/odata/Roles";
            var respondMessageRole = await _httpClient.GetAsync(urlOdataRole);
            if (!respondMessageRole.IsSuccessStatusCode)
            {
                TempData["err"] = "Đã xảy ra lỗi trong quá trình lấy dữ liệu.";
                return RedirectToAction("Index");
            }
            var odataResponse = await respondMessageRole.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Role>>>();
            var listOdataRole = odataResponse.data.ToList();
            listOdataRole.Insert(0, new Role { RoleId = GuidDefault, Rolename = "Tất cả" });
            SelectList selectListRole = new SelectList(listOdataRole, nameof(Role.RoleId), nameof(Role.Rolename), roleId);
            ViewBag.SelectListRole = selectListRole;

            keySearch = keySearch ?? string.Empty;

            var tempRoleId = roleId.Equals(GuidDefault) ? null : roleId;
            roleId = roleId ?? GuidDefault;


            string urlSearchUser = $"https://localhost:7251/api/Users/SearchUser?currentPage={currentPage}&keySearch={keySearch}&roleId={tempRoleId}&size={Item_Page}";
            var respondMessage = await _httpClient.GetAsync(urlSearchUser);
            if (!respondMessage.IsSuccessStatusCode)
            {
                TempData["err"] = "Đã xảy ra lỗi trong quá trình lấy dữ liệu.";
                return RedirectToAction("Index");
            }
            var listUser = await respondMessage.Content.ReadFromJsonAsync<SearchPaging<IEnumerable<ViewUserDto>>>();
            var listUserResponse = listUser.result.OrderByDescending(c => c.Createddate).ToList();
            var totalResult = listUser.total;

            var totalPage = (int)Math.Ceiling((decimal)totalResult / Item_Page);


            ViewBag.TotalPage = totalPage;
            ViewBag.CurrentPage = currentPage;
            ViewBag.KeySearch = keySearch;
            ViewBag.RoleId = roleId;
            ViewBag.ListUser = listUserResponse;
            ViewBag.TotalResultCount = totalResult;



            return View("UserList");
        }

        public async Task<IActionResult> LockOrActive(Guid? userId, bool isLock)
        {
            TempData.Clear();
            if (userId == null)
            {
                TempData["err"] = "Cung cấp không đủ thông tin";
            }
            string token = HttpContext.Request.Cookies[SaveKeySystem.Authentication];
            string urlRegister = $"https://localhost:7251/api/Users/LockOrActive?userId={userId}&isLock={isLock}";
            var request = new HttpRequestMessage(HttpMethod.Put, urlRegister);
            request.Headers.Add("Authorization", "Bearer " + token);


            var response = _httpClient.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = response.Content.ReadAsStringAsync().Result;
                TempData["err"] = $"Đã xảy ra lỗi trong quá trình cập nhật dữ liệu " + errorMessage;
                return RedirectToAction("UserList");
            }
            TempData["success"] = "Đã cập nhật thành công";
            return RedirectToAction("UserList");
        }

        [HttpGet("AddNewUser.html")]
        public async Task<IActionResult> UserAdd()
        {
            TempData.Clear();
            string urlOdataRole = "https://localhost:7251/odata/Roles";
            var respondMessageRole = await _httpClient.GetAsync(urlOdataRole);
            if (!respondMessageRole.IsSuccessStatusCode)
            {
                TempData["err"] = "Đã xảy ra lỗi trong quá trình lấy dữ liệu.";
                return RedirectToAction("Index");
            }
            var odataResponse = await respondMessageRole.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Role>>>();
            var listOdataRole = odataResponse.data.ToList();
            for (int i = 0; i < listOdataRole.Count; i++)
            {
                if (listOdataRole[i].Rolename.ToUpper().Equals("ADMIN"))
                {
                    listOdataRole.RemoveAt(i);
                }
            }

            SelectList selectListRole = new SelectList(listOdataRole, nameof(Role.RoleId), nameof(Role.Rolename));
            ViewBag.SelectListRole = selectListRole;
            AddUserDto addUser = new AddUserDto();
            return View(addUser);
        }
        [HttpPost("AddNewUser.html")]
        public async Task<IActionResult> UserAddPost([FromForm] AddUserDto user)
        {
            TempData.Clear();

            string urlOdataRole = "https://localhost:7251/odata/Roles";
            var respondMessageRole = await _httpClient.GetAsync(urlOdataRole);
            if (!respondMessageRole.IsSuccessStatusCode)
            {
                TempData["err"] = "Đã xảy ra lỗi trong quá trình lấy dữ liệu.";
                return RedirectToAction("Index");
            }
            var odataResponse = await respondMessageRole.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Role>>>();
            var listOdataRole = odataResponse.data.ToList();
            for (int i = 0; i < listOdataRole.Count; i++)
            {
                if (listOdataRole[i].Rolename.ToUpper().Equals("ADMIN"))
                {
                    listOdataRole.RemoveAt(i);
                }
            }

            SelectList selectListRole = new SelectList(listOdataRole, nameof(Role.RoleId), nameof(Role.Rolename));
            ViewBag.SelectListRole = selectListRole;


            if (!ModelState.IsValid)
            {
                TempData["err"] = "Dữ liệu có vẻ không hợp lệ";
                return View("UserAdd");
            }

            string token = HttpContext.Request.Cookies[SaveKeySystem.Authentication];
            string urlRegister = "https://localhost:7251/api/Users/AddAdmin";
            var request = new HttpRequestMessage(HttpMethod.Post, urlRegister);
            request.Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", "Bearer " + token);


            var response = _httpClient.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = response.Content.ReadAsStringAsync().Result;
                TempData["err"] = $"Không thể tạo tài khoản hãy thử lại \n {errorMessage}";

                return View("UserAdd", user);
            }
            TempData["success"] = "Thêm người dùng Thành Công";
            return View("UserAdd", user);


        }
        #endregion

        #region Quan ly thong tin article
        [HttpGet("ManagerArticle.html")]
        public async Task<IActionResult> ArticleList(int currentPage = 1, Guid? categoryId = null, Guid? authorId = null, int processId = 0, int statusPublish = -1, string time = "all", string keySearch = "")
        {
            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessage = await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessage.EnsureSuccessStatusCode();
            var listCategories1 = await responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
            var listCategories = listCategories1.data.ToList();

            //Call api của All Author
            string urlOdataAllAuthor = "https://localhost:7251/odata/Users?$filter=RoleId eq 90293A39-475C-44BC-8506-DAEA10E80384";
            var responseMessageAllAuthor = await _httpClient.GetAsync(urlOdataAllAuthor);
            responseMessageAllAuthor.EnsureSuccessStatusCode();
            var listAuthor1 = await responseMessageAllAuthor.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<User>>>();
            var listAuthor = listAuthor1.data.ToList();

            // Call api của App process status
            string urlProcessStastus = "https://localhost:7251/odata/ProcessStatuss";
            var responseMessageAllProcess = await _httpClient.GetAsync(urlProcessStastus);
            responseMessageAllProcess.EnsureSuccessStatusCode();
            var listProcess1 = await responseMessageAllProcess.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<ProcessStatus>>>();
            var listProcess = listProcess1.data.ToList();
            //Call api
            //

            var listDayFilter = new List<object> {
                new {value ="all",title="Tất Cả" },
                new {value ="now",title="Ngày Hôm nay" },
                new {value ="day",title="1 Ngày Trước" },
                new {value ="week",title="1 Tuần Trước" },
               new {value ="month",title="1 Tháng Trước" },
               new {value ="year",title="1 Năm Trước" }
            };

            var listStatusPublish = new List<object> {
                new {value =-1,title="Tất Cả" },
                new {value =1,title="Cộng khai" },
                new {value =0,title="Cá Nhân" },
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


            listCategories.Insert(0, new CategoriesArticle { CategoryId = GuidDefault, CategoryName = "Tất Cả" });
            listAuthor.Insert(0, new BusinessObjects.Models.User { UserId = GuidDefault, DisplayName = "Tất Cả" });
            listProcess.Insert(0, new BusinessObjects.Models.ProcessStatus { ProcessId = 0, NameProcess = "Tất Cả" });


            SelectList selectListTime = new SelectList(listDayFilter, "value", "title", time);
            SelectList selectListPublishStatus = new SelectList(listStatusPublish, "value", "title", statusPublish);
            SelectList selectListCategory = new SelectList(listCategories, nameof(CategoriesArticle.CategoryId), nameof(CategoriesArticle.CategoryName), categoryId);
            SelectList selectListAuthor = new SelectList(listAuthor, "UserId", "DisplayName", authorId);
            SelectList selectListProcess = new SelectList(listProcess, nameof(ProcessStatus.ProcessId), nameof(ProcessStatus.NameProcess), processId);




            var tempCategory = categoryId;
            categoryId = categoryId ?? GuidDefault;
            var tempAuthor = authorId;
            authorId = authorId ?? GuidDefault;


            //Call api của Category Root
            string urlSearch = $"https://localhost:7251/api/Articles/SearchArticleAdmin?categoryId={categoryId}&authorId={authorId}&processId={processId}" +
                $"&keySearch={keySearch}&statusPublish={statusPublish}" +
                $"&from={fromDate}&to={toDate}&currentPage={currentPage}&size={Item_Page}";


            var httpMessage = new HttpRequestMessage(HttpMethod.Get, urlSearch);
            var responseMessageArticleSearch = await _httpClient.SendAsync(httpMessage);
            responseMessageArticleSearch.EnsureSuccessStatusCode();
            var responseData = await responseMessageArticleSearch.Content.ReadFromJsonAsync<SearchPaging<IEnumerable<ViewArticleDto>>>();




            ViewBag.SelectListTimes = selectListTime;
            ViewBag.SelectListCategorys = selectListCategory;
            ViewBag.SelectListAuthors = selectListAuthor;
            ViewBag.SelectListProcess = selectListProcess;
            ViewBag.SelectLisPublishs = selectListPublishStatus;


            // tra ve tham so search
            ViewBag.CurrentPage = currentPage;
            ViewBag.KeySearch = keySearch;

            ViewBag.CategoryId = categoryId;
            ViewBag.Time = time;
            ViewBag.Process = processId;
            ViewBag.StatusPublish = statusPublish;
            ViewBag.Author = authorId;


            ViewBag.TotalResult = responseData.total;
            ViewBag.TotalPage = (int)(Math.Ceiling((decimal)responseData.total / Item_Page));

            ViewBag.ListSearchArticle = responseData.result;



            return View();
        }
        [HttpGet("AddArticleOfAdmin.html")]
        public async Task<IActionResult> AddArticleAdmin()
        {
            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$orderby=OrderLevel";
            var responseMessage = await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessage.EnsureSuccessStatusCode();
            var listCategories1 = await responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
            var listCategories = listCategories1.data;
            SelectList selectListCategory = new SelectList(listCategories, nameof(CategoriesArticle.CategoryId), nameof(CategoriesArticle.CategoryName));

            // Call api của App process status
            string urlProcessStastus = "https://localhost:7251/odata/ProcessStatuss";
            var responseMessageAllProcess = await _httpClient.GetAsync(urlProcessStastus);
            responseMessageAllProcess.EnsureSuccessStatusCode();
            var listProcess1 = await responseMessageAllProcess.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<ProcessStatus>>>();
            var listProcess = listProcess1.data.ToList();
            SelectList selectListProcessStatus = new SelectList(listProcess, nameof(ProcessStatus.ProcessId), nameof(ProcessStatus.NameProcess));

            //
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true and UserId eq {userLogin.UserId}";
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
            ViewBag.SelectListProcess = selectListProcessStatus;
            AddArticleDto add = new AddArticleDto();
            add.StatusProcess = 3;
            add.CoverImage = "images/anh_banner_default.jpg";

            return View("ArticleAdd", add);
        }
        [HttpPost("AddArticleAdmin.html")]
        public async Task<IActionResult> AddArticleAdminPost(AddArticleDto addArticle)
        {
            TempData.Clear();

            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$orderby=OrderLevel";
            var responseMessage = await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessage.EnsureSuccessStatusCode();
            var listCategories1 = await responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
            var listCategories = listCategories1.data;
            SelectList selectListCategory = new SelectList(listCategories, nameof(CategoriesArticle.CategoryId), nameof(CategoriesArticle.CategoryName), addArticle.CategortyId);
            ViewBag.SelectListCategory = selectListCategory;


            // Call api của App process status
            string urlProcessStastus = "https://localhost:7251/odata/ProcessStatuss";
            var responseMessageAllProcess = await _httpClient.GetAsync(urlProcessStastus);
            responseMessageAllProcess.EnsureSuccessStatusCode();
            var listProcess1 = await responseMessageAllProcess.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<ProcessStatus>>>();
            var listProcess = listProcess1.data.ToList();
            SelectList selectListProcessStatus = new SelectList(listProcess, nameof(ProcessStatus.ProcessId), nameof(ProcessStatus.NameProcess), addArticle.StatusProcess);
            ViewBag.SelectListProcess = selectListProcessStatus;


            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister1 = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true and UserId eq {userLogin.UserId}";
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
            ViewBag.InformationUser = userResponse[0];

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

                return View("ArticleAdd", addArticle);
            }
            // call api them bài báo
            string urlRegister = "https://localhost:7251/api/Articles/AddNewArticle";
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
                return View("ArticleAdd", addArticle);
            }

            TempData["success"] = "Đã tạo bài báo thành công.";



            return View("ArticleAdd", addArticle);
        }



        [HttpGet("UpdateArticleOfAdmin/{articleId}.html")]
        public async Task<IActionResult> UpdateArticleAdmin(Guid articleId)
        {

            if (articleId == null)
            {
                return RedirectToAction("Index");
            }
            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$orderby=OrderLevel";
            var responseMessage = await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessage.EnsureSuccessStatusCode();
            var listCategories1 = await responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
            var listCategories = listCategories1.data;
            SelectList selectListCategory = new SelectList(listCategories, nameof(CategoriesArticle.CategoryId), nameof(CategoriesArticle.CategoryName));

            // Call api của App process status
            string urlProcessStastus = "https://localhost:7251/odata/ProcessStatuss";
            var responseMessageAllProcess = await _httpClient.GetAsync(urlProcessStastus);
            responseMessageAllProcess.EnsureSuccessStatusCode();
            var listProcess1 = await responseMessageAllProcess.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<ProcessStatus>>>();
            var listProcess = listProcess1.data.ToList();
            SelectList selectListProcessStatus = new SelectList(listProcess, nameof(ProcessStatus.ProcessId), nameof(ProcessStatus.NameProcess));

            ViewBag.SelectListCategory = selectListCategory;
            ViewBag.SelectListProcess = selectListProcessStatus;

            // Lấy article
            string urlGetArticle = $"https://localhost:7251/api/Articles/GetArticleById/{articleId}";
            var responseMessageUpdate = await _httpClient.GetAsync(urlGetArticle);

            if (!responseMessage.IsSuccessStatusCode)
            {
                var error = await responseMessageUpdate.Content.ReadAsStringAsync();
                TempData["error"] = "Đã xảy ra lỗi" + error;
                return RedirectToAction("ArticleList");
            }
            var editArticle = await responseMessageUpdate.Content.ReadFromJsonAsync<ViewArticleDto>()
                ;
            if (editArticle == null)
            {
                var error = "Không thể tìm thấy được Article của bạn.";
                TempData["error"] = "Đã xảy ra lỗi" + error;
                return RedirectToAction("ArticleList");
            }
            var updateForView = _mapper.Map<UpdateArticleDto>(editArticle);



            //get thông tin login
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true and UserId eq {userLogin.UserId}";
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


            return View("ArticleUpdate", updateForView);
        }

        [HttpPost("UpdateArticleOfAdmin.html")]
        public async Task<IActionResult> UpdateArticleAdminPost(UpdateArticleDto? updateArticleDto)
        {
            TempData.Clear();
            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$orderby=OrderLevel";
            var responseMessage = await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessage.EnsureSuccessStatusCode();
            var listCategories1 = await responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
            var listCategories = listCategories1.data;
            SelectList selectListCategory = new SelectList(listCategories, nameof(CategoriesArticle.CategoryId), nameof(CategoriesArticle.CategoryName));

            // Call api của App process status
            string urlProcessStastus = "https://localhost:7251/odata/ProcessStatuss";
            var responseMessageAllProcess = await _httpClient.GetAsync(urlProcessStastus);
            responseMessageAllProcess.EnsureSuccessStatusCode();
            var listProcess1 = await responseMessageAllProcess.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<ProcessStatus>>>();
            var listProcess = listProcess1.data.ToList();
            SelectList selectListProcessStatus = new SelectList(listProcess, nameof(ProcessStatus.ProcessId), nameof(ProcessStatus.NameProcess));
            ViewBag.SelectListCategory = selectListCategory;
            ViewBag.SelectListProcess = selectListProcessStatus;

            //get thông tin login
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true and UserId eq {userLogin.UserId}";
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

            // Valid thoong tin cuar bai báo
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
                return View("ArticleUpdate", updateArticleDto);
            }

            string token = HttpContext.Request.Cookies[SaveKeySystem.Authentication];

            // call api them bài báo
            string urlEidtArticle = $"https://localhost:7251/api/Articles/UpdateArticle/{updateArticleDto.ArticleId}";
            var request = new HttpRequestMessage(HttpMethod.Put, urlEidtArticle);
            request.Content = new StringContent(JsonConvert.SerializeObject(updateArticleDto), Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", "Bearer " + token);
            var responseArticle = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                TempData["err"] = $"Không thể cập nhật bài báo hãy thử lại \n {errorMessage}";
                ViewBag.SelectListCategory = selectListCategory;
                ViewBag.InformationUser = userResponse[0];
                return View("ArticleUpdate", updateArticleDto);
            }


            TempData["success"] = $"Đã chỉnh sửa bài báo thành công";
            return View("ArticleUpdate", updateArticleDto);
        }

        [HttpGet("ConfirmDeleteArticle/{articleId}.html")]
        public async Task<IActionResult> DeleteArticleAdmin(Guid? articleId)
        {

            TempData.Clear();
            // Thong tin user
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true and UserId eq {userLogin.UserId}";
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
                return RedirectToAction("ArticleList");
            }
            // get article
            string urlGetArticle = $"https://localhost:7251/api/Articles/GetArticleById/{articleId}";
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
            return View("DeleteArticle", editArticle);
        }
        [HttpPost("Admin/DeleteArticle.html")]

        public async Task<IActionResult> DeleteArticlePost(Guid? articleId)
        {
            string token = HttpContext.Request.Cookies[SaveKeySystem.Authentication];
            string urlDeleteArticle = $"https://localhost:7251/api/Articles/DeleteArticle/{articleId.Value}";
            var request = new HttpRequestMessage(HttpMethod.Delete, urlDeleteArticle);
            request.Headers.Add("Authorization", "Bearer " + token);
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                TempData["success"] = "Đã xảy ra lỗi trong quá trình xóa";
                return RedirectToAction("DeleteArticle");
            }

            TempData["success"] = "Đã xóa thành công bài viết.";
            return RedirectToAction("ArticleList");
        }

        #endregion
        #region Quan ly Emotion
        [HttpGet("ManagementEmotion.html")]
        public async Task<IActionResult> EmotionList()
        {
            string urlEmotion = "https://localhost:7251/odata/Emotions";
            var OdataEmotion = await _httpClient.GetFromJsonAsync<OdataResponse<List<BusinessObjects.Models.Emotion>>>(urlEmotion);
            ViewBag.ListEmotion = OdataEmotion.data;
            return View();
        }
        [HttpGet("EmotionAdd.html")]
        public async Task<IActionResult> EmotionAdd()
        {
            AddEmotionDto addDto = new AddEmotionDto();
            return View(addDto);
        }

        [HttpPost("EmotionAdd.html")]
        public async Task<IActionResult> EmotionAddPost(AddEmotionDto addDto)
        {
            TempData.Clear();
            if (!ModelState.IsValid)
            {
                TempData["err"] = "Dữ liệu có vẻ không hợp lệ";
                return View("AdminAdd");
            }
            string token = HttpContext.Request.Cookies[SaveKeySystem.Authentication];
            string urlRegister = "https://localhost:7251/api/Emotions/AddEmotion";
            var request = new HttpRequestMessage(HttpMethod.Post, urlRegister);
            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(addDto), Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", "Bearer " + token);


            var response = _httpClient.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = response.Content.ReadAsStringAsync().Result;
                TempData["err"] = $"Không thể tạo Emotion hãy thử lại \n {errorMessage}";

                return View("EmotionAdd", addDto);
            }
            TempData["success"] = "Thêm Emotion Thành Công";
            return View("EmotionAdd", addDto);
        }
        [HttpGet("UpdateEmotion/{emotionId}")]
        public async Task<IActionResult> EmotionUpdate(Guid emotionId)
        {
            string urlGetEmotion = $"https://localhost:7251/odata/Emotions?$filter=EmotionId eq {emotionId}";
            var OdataEmotion = await _httpClient.GetFromJsonAsync<OdataResponse<List<Emotion>>>(urlGetEmotion);
            if (OdataEmotion.data.Count() == 0)
            {
                TempData["err"] = "Không tìm thấy thông tin của Emotion";
                return RedirectToAction("EmotionList");
            }
            var update = OdataEmotion.data[0];
            var responseUdpate = new UpdateEmotionDto
            {
                EmotionId = update.EmotionId,
                NameEmotion = update.NameEmotion,
                Image = update.Image
            };
            return View(responseUdpate);
        }

        [HttpPost("UpdateEmotion.html")]
        public async Task<IActionResult> EmotionUpdatePost(UpdateEmotionDto updateEmotion)
        {
            TempData.Clear();
            if (!ModelState.IsValid)
            {
                TempData["err"] = "Dữ liệu có vẻ không hợp lệ";
                return View("AdminAdd");
            }
            string token = HttpContext.Request.Cookies[SaveKeySystem.Authentication];
            string urlRegister = $"https://localhost:7251/api/Emotions/UpdateEmotion/{updateEmotion.EmotionId}";
            var request = new HttpRequestMessage(HttpMethod.Put, urlRegister);
            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(updateEmotion), Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", "Bearer " + token);


            var response = _httpClient.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = response.Content.ReadAsStringAsync().Result;
                TempData["err"] = $"Không thể Cập nhật hãy thử lại \n {errorMessage}";

                return View("EmotionUpdate", updateEmotion);
            }
            TempData["success"] = "Cập nhật thành Thành Công";
            return View("EmotionUpdate", updateEmotion);
        }

        #endregion
        [HttpGet("StaisticRevenueArticle.html")]
        public async Task<IActionResult> ManagementRevenueOfArticler(Guid authorId, Guid? categoryId, DateTime? fromDate, DateTime? endDate, string? keySearch = "", int currentPage = 1)
        {
            string urlAuthor = $"https://localhost:7251/odata/Users?$filter=UserId eq {authorId}";
            var odataAuthor = await _httpClient.GetFromJsonAsync<OdataResponse<List<User>>>(urlAuthor);
            var listUser = odataAuthor.data;
            if(listUser.Count == 0)
            {
                return RedirectToAction("ArticleList");
            }

            if (categoryId == null || categoryId.Equals(GuidDefault))
            {
                categoryId = null;
            }
            // 
            string urlGetArticleOfAuthor = $"https://localhost:7251/api/Articles/GetArticleFromToByOfAuthorId?authorId={authorId}&fromDate={fromDate}&endDate={endDate}&keySearch={keySearch}&categoryId={categoryId}";
            var listArticle = await _httpClient.GetFromJsonAsync<List<ViewArticleDto>>(urlGetArticleOfAuthor);

            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$orderby=OrderLevel";
            var odataCategory = await _httpClient.GetFromJsonAsync<OdataResponse<List<CategoriesArticle>>>(urlOdataAllCategory);
            var listCategory = odataCategory.data;
            var temp = categoryId ?? GuidDefault;
            listCategory.Insert(0, new CategoriesArticle { CategoryId = GuidDefault, CategoryName = "Tất cả thể loại" });
            SelectList selectListCategory = new SelectList(listCategory, nameof(CategoriesArticle.CategoryId), nameof(CategoriesArticle.CategoryName), categoryId);


            int totalView = listArticle.Sum(c => c.ViewArticles).Value;
            var totalRevenue = Math.Ceiling((decimal)totalView / 10) * 100;
            int totalResult = listArticle.Count();
            int totalPage = (int)Math.Ceiling((double)totalResult / Item_Page);
            var listArticlePaging = listArticle.Skip((currentPage - 1) * Item_Page).Take(Item_Page);

            CultureInfo vietnameseCulture = new CultureInfo("vi-VN");

            // Định dạng số tiền
            string formattedAmount = string.Format(vietnameseCulture, "{0:C}", totalRevenue);

            // thông tin author 


            ViewBag.Articles = listArticlePaging.ToList();
            ViewBag.TotalRevenue = formattedAmount;
            ViewBag.TotalView = totalView;
            ViewBag.KeySearch = keySearch;
            ViewBag.FromDate = fromDate;
            ViewBag.EndDate = endDate;
            ViewBag.AuthorId = authorId;
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPage = totalPage;
            ViewBag.TotalResult = totalResult;
            ViewBag.CategoryId = temp;
            ViewBag.Author = listUser[0];

            ViewBag.SelectListCategorys = selectListCategory;

            return View();
        }


    }
}
