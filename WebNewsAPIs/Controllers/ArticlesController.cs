using AutoMapper;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Repositories;
using WebNewsAPIs.Dtos;
using WebNewsAPIs.Services;

namespace WebNewsAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private IArticleRepository _articleRepository;
        private ICategoriesArticleRepository _categoryRepository;
        private IMapper _mapper;
        private ArticleService _articleService;
        public ArticlesController(IArticleRepository articleRepository
            , IMapper mapper, ArticleService article
            ,ICategoriesArticleRepository categoriesArticleRepository)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
            this._articleService = article;
            this._categoryRepository = categoriesArticleRepository;
        }
        [HttpGet]
        [EnableQuery]
        public IEnumerable<Article> Get() 
        {
            string [] includes = new string[]
            {
                nameof(Article.Categorty),
                nameof(Article.Comments),
                nameof(Article.AuthorNavigation)
            };
            var listArticles = _articleRepository.GetAll(includes);
            return  listArticles.AsQueryable();
        }

        [HttpGet("GetArticlesWithView")]
        
        public async Task<ActionResult<IEnumerable<Article>>>GetArticlesWithView()
        {

            if (_articleRepository.Equals(null))
            {
                return BadRequest();
            }
            string[] includes = new string[]
            {
                nameof(Article.Categorty),
                nameof(Article.Comments),
                nameof(Article.AuthorNavigation)
            };
            var listArticles = _articleRepository.GetMulti(c => true,includes);
            var response = _mapper.Map<IEnumerable<ViewArticleDto>>(listArticles).OrderBy(c => c.PublishDate);
            return Ok(response);
        }

		[HttpGet("GetArticleOfRootCategory")]

		public async Task<ActionResult<Dictionary<string, List<ViewArticleDto>>>> GetArticleOfRootCategory()
		{
            if(_articleService == null)
            {
                return BadRequest();
            }
            var data = _articleService.GetAllArticlesOfRootCategory();
            var response = new Dictionary<string, List<ViewArticleDto>>();
            foreach (var category in data.Keys)
            {
                var listArticleMapper = _mapper.Map<List<ViewArticleDto>>(data[category]);
                response.Add(category, listArticleMapper);
            }
			return Ok(response);
		}
        [HttpGet("GetAllArticleOfAllCategory")]
		public async Task<ActionResult<List<ViewArticleDto>>> GetArticleOfHaveSameRoot([FromQuery]Guid categoryId)
		{
            string[] includes = new string[]
            {
                nameof(Article.Categorty)

            };
			if (_articleRepository == null)
			{
				return BadRequest();
			}
			var rootCate = _categoryRepository.getRootOfCategory(categoryId);
            if(rootCate == null)
            {
                return NotFound();
            }
			//var listArticle = _articleRepository.GetMulti(c => _categoryRepository.getRootOfCategory(c.CategortyId).CategoryId.Equals(rootCate.CategoryId), includes);
			var data = _articleService.GetAllArticlesOfRootCategory();
            var dataResponse = data[rootCate.CategoryName];

			return Ok(_mapper.Map<List<ViewArticleDto>>(dataResponse));
		}


	}
}
