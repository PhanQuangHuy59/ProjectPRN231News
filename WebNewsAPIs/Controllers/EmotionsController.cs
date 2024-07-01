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
