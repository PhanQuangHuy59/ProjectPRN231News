using AutoMapper;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Repositories;
using System.Linq.Expressions;
using System.Reflection;
using WebNewsAPIs.Dtos;
using WebNewsAPIs.Services;
using static System.Net.Mime.MediaTypeNames;

namespace WebNewsAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private IArticleRepository _articleRepository;
        private ICategoriesArticleRepository _categoryRepository;
        private IUserRepository _userRepo;
        private IViewRepository _viewRepository;
        private IMapper _mapper;
        private ArticleService _articleService;
        public ArticlesController(IArticleRepository articleRepository
            , IMapper mapper, ArticleService article
            , ICategoriesArticleRepository categoriesArticleRepository,
            IViewRepository viewRepository, IUserRepository userRepo)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
            this._articleService = article;
            this._categoryRepository = categoriesArticleRepository;
            _viewRepository = viewRepository;
            _userRepo = userRepo;
        }

        [HttpGet]
        [EnableQuery]
        public IEnumerable<Article> Get()
        {
            //string[] includes = new string[]
            //{
            //    nameof(Article.Categorty),
            //    nameof(Article.Comments),
            //    nameof(Article.AuthorNavigation)
            //};
            var listArticles = _articleRepository.GetAll();
            return listArticles.AsQueryable();
        }

        [HttpGet("GetArticlesWithView")]

        public async Task<ActionResult<IEnumerable<Article>>> GetArticlesWithView()
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

            var listArticles = _articleRepository.GetMulti(c => true, includes);


            var response = _mapper.Map<IEnumerable<ViewArticleDto>>(listArticles).OrderBy(c => c.PublishDate);
            return Ok(response);
        }
        [HttpGet("GetArticleById/{articleId}")]
        public IActionResult GetArticleById(Guid? articleId)
        {
            if (articleId == null)
            {
                return NotFound("Id không hợp lệ.");
            }

            var getArticle = _articleRepository.GetSingleByCondition(c => c.ArticleId == articleId).Result;
            if (getArticle == null)
            {
                return NotFound("Không tìm thấy bài báo nào phù hợp.");
            }

            return Ok(_mapper.Map<ViewArticleDto>(getArticle));
        }


        [HttpGet("GetArticleOfRootCategory")]

        public async Task<ActionResult<Dictionary<string, List<ViewArticleDto>>>> GetArticleOfRootCategory()
        {
            if (_articleService == null)
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
        public async Task<ActionResult<List<ViewArticleDto>>> GetArticleOfHaveSameRoot([FromQuery] Guid categoryId)
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
            if (rootCate == null)
            {
                return NotFound();
            }
            //var listArticle = _articleRepository.GetMulti(c => _categoryRepository.getRootOfCategory(c.CategortyId).CategoryId.Equals(rootCate.CategoryId), includes);
            var data = _articleService.GetAllArticlesOfRootCategory();
            var dataResponse = data[rootCate.CategoryName];

            return Ok(_mapper.Map<List<ViewArticleDto>>(dataResponse));
        }

        [HttpGet("SearchArticle")]
        public async Task<ActionResult<SearchPaging<IEnumerable<ViewArticleDto>>>> SearchArticle(Guid? categoryId = null, string? keySearch = "", DateTime? from = null, DateTime? to = null, int currentPage = 1, int size = 20)
        {
            string guid_Default = "00000000-0000-0000-0000-000000000000";
            string[] includes = new string[]
            {
                nameof(Article.Categorty),
                nameof(Article.Comments)
            };
            if (_articleRepository == null)
            {
                return BadRequest();
            }
            List<Article> listNew = new List<Article>();
            int sizeResult = 0;
            keySearch = (keySearch ?? string.Empty);

            Expression<Func<Article, bool>> predicate = (c => (categoryId == Guid.Parse(guid_Default) || c.CategortyId == categoryId)
            && (from == null || c.CreatedDate.Date >= from.Value.Date) && (to == null || c.CreatedDate.Date <= to.Value.Date) &&
            (c.Title.ToLower().Contains(keySearch.ToLower()) || c.ShortDescription.ToLower().Contains(keySearch.ToLower()))
            && c.StatusProcess == 3);
            listNew = _articleRepository.GetMultiPaging(predicate, out sizeResult, currentPage - 1, size, includes).ToList();

            var listResponse = _mapper.Map<IEnumerable<ViewArticleDto>>(listNew);
            var result = new SearchPaging<IEnumerable<ViewArticleDto>>
            {
                total = sizeResult,
                result = listResponse
            };

            return Ok(result);
        }
        [HttpGet("SearchArticleOfAuthor")]
        public async Task<ActionResult<SearchPaging<IEnumerable<ViewArticleDto>>>> SearchArticleOfAuthor(Guid? authorId, Guid? categoryId = null, string? keySearch = "", int currentPage = 1, int size = 20)
        {
            string guid_Default = "00000000-0000-0000-0000-000000000000";
            string[] includes = new string[]
            {
                nameof(Article.Categorty),
                nameof(Article.Comments)
            };
            if (_articleRepository == null || authorId == null)
            {
                return BadRequest();
            }
            List<Article> listNew = new List<Article>();
            int sizeResult = 0;
            keySearch = (keySearch ?? string.Empty);

            Expression<Func<Article, bool>> predicate = (c => (categoryId == Guid.Parse(guid_Default) || c.CategortyId == categoryId)
            && c.Author.Equals(authorId) && c.StatusProcess == 3 &&
            (c.Title.ToLower().Contains(keySearch.ToLower()) || c.ShortDescription.ToLower().Contains(keySearch.ToLower())));
            listNew = _articleRepository.GetMultiPaging(predicate, out sizeResult, currentPage - 1, size, includes).ToList();

            var listResponse = _mapper.Map<IEnumerable<ViewArticleDto>>(listNew);
            var result = new SearchPaging<IEnumerable<ViewArticleDto>>
            {
                total = sizeResult,
                result = listResponse
            };

            return Ok(result);
        }
        [HttpGet("SearchArticleOfUser")]
        public async Task<ActionResult<SearchPaging<IEnumerable<ViewArticleDto>>>> SearchArticleOfUser(int? processId, Guid? authorId, Guid? categoryId = null, string? keySearch = "", int currentPage = 1, int size = 20)
        {
            string guid_Default = "00000000-0000-0000-0000-000000000000";
            string[] includes = new string[]
            {
                nameof(Article.Categorty),
                nameof(Article.Comments)
            };
            if (_articleRepository == null || authorId == null)
            {
                return BadRequest();
            }
            List<Article> listNew = new List<Article>();
            int sizeResult = 0;
            keySearch = (keySearch ?? string.Empty);

            Expression<Func<Article, bool>> predicate = (c => (categoryId == Guid.Parse(guid_Default) || c.CategortyId == categoryId)
            && c.Author.Equals(authorId) && (processId == null || c.StatusProcess == processId) &&
            (c.Title.ToLower().Contains(keySearch.ToLower()) || c.ShortDescription.ToLower().Contains(keySearch.ToLower())));
            listNew = _articleRepository.GetMultiPaging(predicate, out sizeResult, currentPage - 1, size, includes).ToList();

            var listResponse = _mapper.Map<IEnumerable<ViewArticleDto>>(listNew);
            var result = new SearchPaging<IEnumerable<ViewArticleDto>>
            {
                total = sizeResult,
                result = listResponse
            };

            return Ok(result);
        }

        [HttpGet("GetBoardArticleOfUser")]
        public async Task<ActionResult<IEnumerable<ViewArticleDto>>> GetBoardArticleOfUser(Guid? userId = null)
        {
            string[] includes = new string[]
            {
                nameof(Article.Categorty),
                nameof(Article.Comments)
            };
            string[] includesforViewArticle = new string[]
            {
                nameof(View.Article)
            };
            if (userId == null)
            {
                return BadRequest();
            }
            var getViewArticleOfUser = _viewRepository.GetMulti(c => c.UserId == userId, includesforViewArticle);

            var listArticleIgnore = getViewArticleOfUser.Select(c => c.ArticleId).ToList();
            var getCategoryFromViewArticleOfUser = getViewArticleOfUser.Select(c => c.Article.CategortyId).ToList();

            var listArticleBoard = _articleRepository.GetMulti(c => getCategoryFromViewArticleOfUser.Contains(c.CategortyId)
            & !listArticleIgnore.Contains(c.ArticleId)
            , includes).OrderBy(c => c.CreatedDate).OrderBy(c => c.CreatedDate);


            return Ok(_mapper.Map<IEnumerable<ViewArticleDto>>(listArticleBoard));
        }

        [HttpPost("IncreaseViewArticle")]
        public IActionResult IncreaseViewArticle(Guid? articleId)
        {
            if (articleId == null)
            {
                return BadRequest();
            }
            var articleCheck = _articleRepository.GetSingleByCondition(c => c.ArticleId.Equals(articleId)).Result;
            if (articleCheck == null)
            {
                return NotFound();
            }
            articleCheck.ViewArticles = articleCheck.ViewArticles + 1;
            _articleRepository.UpdateAsync(articleCheck).Wait();
            return Ok();
        }

        //Add New Article 
        [Authorize(Roles = "Articles,Admin")]
        [HttpPost("AddNewArticle")]
        public IActionResult AddNewArticle(AddArticleDto? articleNew)
        {
            if (articleNew == null)
            {
                return BadRequest();
            }
            string[] includes = new string[]
            {
                nameof(BusinessObjects.Models.User.Role)
            };

            var checkUser = _userRepo.GetSingleByCondition(c => c.UserId.Equals(articleNew.Author) && c.Role.Rolename.Equals("Articles"), includes).Result;
            var checkCategory = _categoryRepository.GetSingleByCondition(c => c.CategoryId.Equals(articleNew.CategortyId)).Result;
            if (checkUser == null || checkCategory == null)
            {
                return StatusCode(404, "Thông tin cung cấp không phù hợp. ");
            }

            var articleAdd = _mapper.Map<Article>(articleNew);
            articleAdd.Slug = VerifyInformation.ToSlug(articleNew.Title) + DateTime.Now.ToString(" dd-MM-yyyy:hh:mm:ss");
            articleAdd.Processor = Guid.Parse("D5999D13-FC43-4A3D-B18D-1A63F3960BA9");
            articleAdd.StatusProcess = 1;
            articleNew.Title = articleNew.Title.Replace(@"\s+", "");
            articleAdd.CreatedDate = DateTime.Now;
            if (articleAdd.CoverImage == null)
            {
                articleAdd.CoverImage = "/images/anh_banner_default.jpg";
            }
            var checkExistArticle = _articleRepository.GetSingleByCondition(c => c.Slug.Equals(articleAdd.Slug)
            || c.Title.ToLower().Equals(articleAdd.Title)).Result;
            if (checkExistArticle != null)
            {
                return StatusCode(400, "Bài báo đã tồn tại không thể thêm. Hãy tạo với tiêu đề khác.");
            }
            try
            {
                _articleRepository.AddAsync(articleAdd).Wait();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình thêm bài báo.");
            }

            return Ok(articleAdd);
        }

        [Authorize(Roles = "Articles,Admin")]
        [HttpPut("UpdateArticle/{articleId}")]
        public IActionResult UpdateArticle(Guid? articleId, UpdateArticleDto? articleUpdate)
        {
            if (articleUpdate == null || articleId == null)
            {
                return BadRequest();
            }
            string[] includes = new string[]
           {
                nameof(BusinessObjects.Models.User.Role)
           };

            var checkUser = _userRepo.GetSingleByCondition(c => c.UserId.Equals(articleUpdate.Author), includes).Result;
            var checkCategory = _categoryRepository.GetSingleByCondition(c => c.CategoryId.Equals(articleUpdate.CategortyId)).Result;
            if (checkUser == null || checkCategory == null)
            {
                return StatusCode(404, "Thông tin cung cấp không phù hợp. ");
            }


            if (!articleId.Equals(articleUpdate.ArticleId))
            {
                return BadRequest();
            }
            var checkArticleExist = _articleRepository.GetSingleByCondition(c => c.ArticleId.Equals(articleId)).Result;
            if (checkArticleExist == null)
            {
                return NotFound("Không tìm thấy bài báo nào phù hợp để cập nhật.");
            }


            checkArticleExist.UpdatedDate = DateTime.Now;
            checkArticleExist.IsPublish = articleUpdate.IsPublish;
            checkArticleExist.CategortyId = articleUpdate.CategortyId;
            checkArticleExist.Content = articleUpdate.Content;
            checkArticleExist.CoverImage = articleUpdate.CoverImage;
            checkArticleExist.Processor = articleUpdate.Processor;
            checkArticleExist.LinkAudio = articleUpdate.LinkAudio;
            checkArticleExist.UpdatedDate = DateTime.Now;
            checkArticleExist.Author = articleUpdate.Author;
            checkArticleExist.PublishDate = articleUpdate.PublishDate;
            checkArticleExist.ShortDescription = articleUpdate.ShortDescription;
            checkArticleExist.Title = articleUpdate.Title.Replace(@"\s+", "");
            if (!checkArticleExist.Title.ToLower().Equals(articleUpdate.Title.ToLower()))
            {
                checkArticleExist.Slug = VerifyInformation.ToSlug(articleUpdate.Title) + DateTime.Now.ToString(" dd-MM-yyyy:hh:mm:ss");
            }

            var checkExistArticleBySlug = _articleRepository.GetSingleByCondition(c =>
            c.Title.ToLower().Equals(checkArticleExist.Title.ToLower()) && !c.ArticleId.Equals(articleId)).Result;

            if (checkExistArticleBySlug != null)
            {
                return StatusCode(400, "Bài báo đã tồn tại không thể cập nhât. Hãy cập nhật với tiêu đề khác.");
            }
            _articleRepository.UpdateAsync(checkArticleExist).Wait();
            return Ok(checkArticleExist);
        }


    }
}
