using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Repositories;

namespace WebNewsAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmotionsController : ControllerBase
    {
        private IEmotionRepository _emotionRepo;
        public EmotionsController(IEmotionRepository emotionRepo
            )
        {
            _emotionRepo = emotionRepo;
          
        }
        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_emotionRepo.GetAll());
        }
    }
}
