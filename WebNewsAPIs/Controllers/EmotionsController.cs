using AutoMapper;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Repositories;
using WebNewsAPIs.Dtos;
using WebNewsAPIs.Services;

namespace WebNewsAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmotionsController : ControllerBase
    {
        private IMapper _mapper;
        private IEmotionRepository emotionRepository;

        public EmotionsController(IMapper mapper, IEmotionRepository emotionRepository)
        {
            _mapper = mapper;
            this.emotionRepository = emotionRepository;
        }
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<Emotion>>> Get()
        {
            if (_mapper == null || emotionRepository == null)
            {
                return StatusCode(500, "Hệ thống api đang bảo trì.");
            }
            var list = emotionRepository.GetAll();
            return Ok(list.AsQueryable());
        }
    }
}
