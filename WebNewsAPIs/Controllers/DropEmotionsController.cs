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
        public async Task<ActionResult<bool>> AddOrRemoveDropEmotion(Guid? emotionId, Guid? userId, Guid? articleId)
        {
            if (emotionId == null || userId == null || articleId == null)
            {
                return BadRequest();
            }

            var checkEmotionExist =await _emotionRepo.GetSingleByCondition(c => c.EmotionId.Equals(emotionId));
            var checkUserExist = await _userRepo.GetSingleByCondition(c => c.UserId.Equals(userId));
            var checkArticleExist = await _articleRepo.GetSingleByCondition(c => c.ArticleId.Equals(articleId));
            if (checkEmotionExist == null || checkUserExist == null || checkArticleExist == null)
            {
                return NotFound();
            }
            var checkDropEmotion =await _dropEmotionRepo.GetSingleByCondition(c => c.UserId.Equals(userId)
            && c.ArticleId.Equals(articleId) && c.EmotionId.Equals(emotionId));
            if (checkDropEmotion == null)
            {
                var newDropEmotion = new DropEmotion
                {
                    EmotionId = emotionId.Value,
                    UserId = userId.Value,
                    ArticleId = articleId.Value 
                };
                var response = await _dropEmotionRepo.AddDropEmotion(newDropEmotion);
                return Ok(true);
            }
            try
            {
                var response = await _dropEmotionRepo.DeleteAsync(checkDropEmotion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hệ thống đang lỗi.");
            }

            return Ok(false);
        }

    }
}
