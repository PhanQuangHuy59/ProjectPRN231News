using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Repositories;

namespace WebNewsAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DropEmotionsController : ControllerBase
    {
        private IDropEmotionRepository _dropEmotionRepo;
        private IUserRepository _userRepo;
        private IArticleRepository _articleRepo;
        private IEmotionRepository _emotionRepo;
        public DropEmotionsController(IDropEmotionRepository dropEmotionRepo,
            IUserRepository userRepo, IArticleRepository articleRepo,
            IEmotionRepository emotionRepo)
        {
            _dropEmotionRepo = dropEmotionRepo;
            _userRepo = userRepo;
            _articleRepo = articleRepo;
            _emotionRepo = emotionRepo;
        }

        [HttpGet]
        [EnableQuery]
        public ActionResult<IEnumerable<SaveArticle>> Get()
        {
            return Ok(_dropEmotionRepo.GetAll().AsQueryable());
        }
        [HttpPost("AddOrRemoveDropEmotion")]
        public ActionResult<bool> AddOrRemoveDropEmotion(Guid? emotionId, Guid? userId, Guid? articleId)
        {
            if (emotionId == null || userId == null || articleId == null)
            {
                return BadRequest();
            }

            var checkEmotionExist = _emotionRepo.GetSingleByCondition(c => c.EmotionId.Equals(emotionId)).Result;
            var checkUserExist = _userRepo.GetSingleByCondition(c => c.UserId.Equals(userId)).Result;
            var checkArticleExist = _articleRepo.GetSingleByCondition(c => c.ArticleId.Equals(articleId)).Result;
            if (checkEmotionExist == null || checkUserExist == null || checkArticleExist == null)
            {
                return NotFound();
            }
            var checkDropEmotion = _dropEmotionRepo.GetSingleByCondition(c => c.UserId.Equals(userId)
            && c.ArticleId.Equals(articleId) && c.EmotionId.Equals(emotionId)).Result;
            if (checkDropEmotion == null)
            {
                var newDropEmotion = new DropEmotion
                {
                    EmotionId = emotionId.Value,
                    UserId = userId.Value,
                    ArticleId = articleId.Value,
                    Emotion = checkEmotionExist,
                    User = checkUserExist,
                    Article = checkArticleExist
                };
                var response = _dropEmotionRepo.AddAsync(newDropEmotion).Result;
                return Ok(true);
            }
            try
            {
                var response = _dropEmotionRepo.DeleteAsync(checkDropEmotion).Result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hệ thống đang lỗi.");
            }

            return Ok(false);
        }

    }
}
