using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;
using WebNewsAPIs.Dtos;

namespace WebNewsClients.Controllers
{
	public class AuthorArticleController : Controller
	{
        HttpClient _httpClient;
        private const int Items_Page_Search = 18;

		public AuthorArticleController(HttpClient httpClient) 
        { 
            _httpClient = httpClient;
        
        }
        [HttpGet("ArticleOfAuthor/{authorId}.html")]
		public IActionResult Index(Guid? authorId, Guid? categoryId, string keySearch="", int? currentPage=1)
		{
			string guid_Default = "00000000-0000-0000-0000-000000000000";
			//Call api của Category Root
			string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";
            var responseMessage = _httpClient.GetAsync(urlOdataAllCategory).Result;
            responseMessage.EnsureSuccessStatusCode();
            var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
                .Result.data;
           if(authorId  == null)
            {
                ViewBag.message = "Không Cung cấp đủ thông tin! ";
                ViewBag.Category = listCategories;
                return RedirectToAction("Error400", "Home");
            }
            // call api author
            var urlCallAuthor = $"https://localhost:7251/odata/Users?$expand=Role&$filter=UserId eq {authorId}";
            var responseMessageCallAuthor = _httpClient.GetAsync(urlCallAuthor).Result;
            responseMessageCallAuthor.EnsureSuccessStatusCode();
            var authorData = responseMessageCallAuthor.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<User>>>()
                .Result.data.ToList();
            if (authorData.Count() == 0)
            {
                ViewBag.message = "Không thể tìm thấy Author có Id : " + authorId.ToString();
                ViewBag.Category = listCategories;
                return RedirectToAction("Error400", "Home");
            }

            if(categoryId == null)
            {
                categoryId = Guid.Parse(guid_Default);
            }

			// Các bài báo của author
			string urlSearch = $"https://localhost:7251/api/Articles/SearchArticleOfAuthor?categoryId={categoryId}&keySearch={keySearch}&authorId={authorId}" +
				$"&currentPage={currentPage}&size={Items_Page_Search}";
			var httpMessage = new HttpRequestMessage(HttpMethod.Get, urlSearch);
			var responseMessageArticleSearch = _httpClient.SendAsync(httpMessage).Result;
			responseMessageArticleSearch.EnsureSuccessStatusCode();
			var responseData = responseMessageArticleSearch.Content.ReadFromJsonAsync<SearchPaging<IEnumerable<ViewArticleDto>>>().Result;

			//
			var tempListCategory = listCategories.ToList();
			tempListCategory.Insert(0, new CategoriesArticle { CategoryId = Guid.Parse(guid_Default), CategoryName = "Tất Cả" });
			SelectList selectListCategory = new SelectList(tempListCategory, nameof(CategoriesArticle.CategoryId), nameof(CategoriesArticle.CategoryName), categoryId);

			// phan trang 
			ViewBag.CurrentPage = currentPage;
			ViewBag.KeySearch = keySearch;
			ViewBag.CategoryId = categoryId;
			ViewBag.TotalResultCount = responseData.total;
			ViewBag.TotalPage = (int)(Math.Ceiling((decimal)responseData.total / Items_Page_Search));
			ViewBag.SelectListCategory = selectListCategory;
			//
			ViewBag.ArticleOfAuthor = responseData.result;
            ViewBag.Author = authorData[0];
            ViewBag.Category = listCategories;

            return View();
		}
	}
}
