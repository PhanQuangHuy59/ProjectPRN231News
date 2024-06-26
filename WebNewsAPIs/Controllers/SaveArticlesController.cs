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
        public ActionResult<IEnumerable<SaveArticle>> Get()
        {
            return Ok(_saveRepo.GetAll().AsQueryable());    
        }
        [HttpPost("RemoveOrAddSaveArticle")]
        public  ActionResult<bool> RemoveOrAddSaveArticle(Guid? userId, Guid? articleId)
        {
            if(userId == null || articleId == null)
            {
                return BadRequest();
            }
            var userCheck = _userRepo.GetSingleByCondition(c => c.UserId.Equals(userId)).Result;
            if(userCheck == null)
            {
                return StatusCode(404,"User không tồn tại!");
            }
            var articleCheck = _articleRepo.GetSingleByCondition(c => c.ArticleId.Equals(articleId)).Result;
            if (articleCheck == null)
            {
                return StatusCode(404, "Article Không tồn tại");
            }

            var checkExist = _saveRepo.GetSingleByCondition(c => c.UserId.Equals(userId) & c.ArticleId.Equals(articleId)).Result;
            if(checkExist == null)
            {
                var addSave = new SaveArticle
                {
                    UserId = userId.Value,
                    ArticleId = articleId.Value,
                    Article = articleCheck,
                    User = userCheck
                };

                var response = _saveRepo.AddAsync(addSave).Result;
                return Ok(true);
            }
            else
            {
               var a = _saveRepo.DeleteAsync(checkExist).Result;
                return Ok(false);
            }

            
        }
    }
}
