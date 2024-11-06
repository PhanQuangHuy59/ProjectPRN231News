using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Repositories;

namespace WebNewsAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaveArticlesController : ControllerBase
    {
        private ISaveArticleRepository _saveRepo;
        private IUserRepository _userRepo;
        private IArticleRepository _articleRepo;
        public SaveArticlesController(ISaveArticleRepository saveRepo,
            IUserRepository userRepo, IArticleRepository articleRepo)
        {
            _saveRepo = saveRepo;
            _userRepo = userRepo;
            _articleRepo = articleRepo;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<SaveArticle>>> Get()
        {
            return Ok(_saveRepo.GetAll().AsQueryable());
        }
        [HttpPost("RemoveOrAddSaveArticle")]
        public async Task<ActionResult<bool>> RemoveOrAddSaveArticle(Guid? userId, Guid? articleId)
        {
            if (userId == null || articleId == null)
            {
                return BadRequest();
            }
            var userCheck = await _userRepo.GetSingleByCondition(c => c.UserId.Equals(userId));
            if (userCheck == null)
            {
                return StatusCode(404, "User không tồn tại!");
            }
            var articleCheck = await _articleRepo.GetSingleByCondition(c => c.ArticleId.Equals(articleId));
            if (articleCheck == null)
            {
                return StatusCode(404, "Article Không tồn tại");
            }

            var checkExist = await _saveRepo.GetSingleByCondition(c => c.UserId.Equals(userId) & c.ArticleId.Equals(articleId));
            if (checkExist == null)
            {
                var addSave = new SaveArticle
                {
                    UserId = userId.Value,
                    ArticleId = articleId.Value
                };

                var response =await _saveRepo.AddSaveArticle(addSave);
                return Ok(true);
            }
            else
            {
                var a =await _saveRepo.DeleteAsync(checkExist);
                return Ok(false);
            }


        }
    }
}
