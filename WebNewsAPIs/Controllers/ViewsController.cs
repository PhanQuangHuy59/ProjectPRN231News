using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Repositories;

namespace WebNewsAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewsController : ControllerBase
    {
        private IViewRepository _viewRepo;
        public ViewsController(IViewRepository viewRepo)
        { 
            _viewRepo = viewRepo;   
        }
        [EnableQuery]
        [HttpGet]
        public ActionResult<IEnumerable<View>> Get()
        {
            var listResponse = _viewRepo.GetAll();
            return Ok(listResponse.AsQueryable());
        }
    }
}
