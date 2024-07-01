using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Repositories;

namespace WebNewsAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessStatussController : ControllerBase
    {
        private IProcessStatusRepository _processRepo;
        public ProcessStatussController(IProcessStatusRepository processRepo)
        {
            _processRepo = processRepo;
        }

        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_processRepo.GetAll().AsQueryable());
        }
    }
}
