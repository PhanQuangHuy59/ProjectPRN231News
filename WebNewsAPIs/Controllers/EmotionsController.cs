using AutoMapper;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Repositories;
using WebNewsAPIs.Dtos;

namespace WebNewsAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmotionsController : ControllerBase
    {
        private IEmotionRepository _emotionRepo;
        private IMapper _mapper;
        public EmotionsController(IEmotionRepository emotionRepo,
            IMapper mapper
            )
        {
            _emotionRepo = emotionRepo;
            _mapper = mapper;
        }
        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_emotionRepo.GetAll().AsQueryable());
        }
        [HttpPost("AddEmotion")]
        [Authorize(Roles = "Admin,Articles")]
        public async Task<IActionResult> AddEmotion(AddEmotionDto add)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var checkEmotion = await _emotionRepo.GetSingleByCondition(c => c.NameEmotion.ToLower().Equals(add.NameEmotion.ToLower()));
            if (checkEmotion != null)
            {
                return StatusCode(410, "Emotion bị trùng tên với một emotion khác hãy thử tên khác.");
            }
            var addEmotion = _mapper.Map<Emotion>(add);

            _emotionRepo.AddEmotion(addEmotion);

            return Ok();
        }
        [HttpPut("UpdateEmotion/{emotionId}")]
        [Authorize(Roles = "Admin,Articles")]
        public async Task<IActionResult> UpdateEmotion(Guid emotionId, UpdateEmotionDto update)
        {
            if (emotionId != update.EmotionId)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var emotionExist = await _emotionRepo.GetSingleByCondition(c => c.EmotionId.Equals(emotionId));
            if (emotionExist == null)
            {
                return NotFound("Không tồn tại emotion");
            }
            emotionExist.NameEmotion = update.NameEmotion;
            emotionExist.Image = update.Image;


            var checkEmotion = await _emotionRepo.GetSingleByCondition(c => c.NameEmotion.ToLower().Equals(update.NameEmotion.ToLower())
            && !c.EmotionId.Equals(emotionId));
            if (checkEmotion != null)
            {
                return StatusCode(410, "Emotion bị trùng tên với một emotion khác hãy thử tên khác.");
            }

            await _emotionRepo.UpdateEmotion(emotionExist);

            return Ok();
        }
        [HttpDelete("DeleteEmotion/{emotionId}")]
        [Authorize(Roles = "Admin,Articles")]
        public async Task<IActionResult> DeleteEmotion(Guid emotionId)
        {

            var emotionExist = await _emotionRepo.DeleteEmotion(emotionId);
            if (emotionExist == null)
            {
                return NotFound("Không tìm thấy thông tin của Emotion.");
            }


            return Ok("Xóa thành công Emotion");
        }
    }
}
