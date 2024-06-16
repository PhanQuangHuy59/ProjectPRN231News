using AutoMapper;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Repositories;
using WebNewsAPIs.Dtos;

namespace WebNewsAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private IArticleRepository _articleRepository;
        private IMapper _mapper;
        public ArticlesController(IArticleRepository articleRepository, IMapper mapper)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
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
    }
}
