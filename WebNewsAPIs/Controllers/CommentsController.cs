using AutoMapper;
using BusinessObjects.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.ModelBuilder;
using Repositories;
using WebNewsAPIs.Dtos;

namespace WebNewsAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private ICommentRepository _commentRepo;
        IUserRepository _userRepo;
        private IMapper _mapper;
        public CommentsController(ICommentRepository _comment, 
            IMapper mapper,
            IUserRepository userRepo)
        {
            _commentRepo = _comment;
            _mapper = mapper;
            _userRepo = userRepo;
        }
        [HttpGet]
        [EnableQuery]

        public async Task<ActionResult<IQueryable<Comment>>> Get()
        {
            if(_commentRepo == null || _mapper == null)
            {
               return BadRequest();
            }
            
            return Ok(_commentRepo.GetAll());
        }

        [HttpPost]
        public  ActionResult<Comment> PostComment(AddCommentStringDto comment)
        {
            if(_commentRepo == null|| _mapper == null)
            {
                return StatusCode(500, "Dịch vụ hệ thống đang gặp lỗi xin hãy chờ đợi");
            }
            if (!ModelState.IsValid)
            {
                return StatusCode(400, "Dữ liệu của bạn không thỏa mãn");
            }
            var checkUser = _userRepo.GetSingleByCondition(c => c.UserId.ToString().Equals(comment.UserId));
            if (checkUser == null){
                return StatusCode(404, "Không tìm thấy User");
            }
            var commentAdd = _mapper.Map<Comment>(comment);
            commentAdd.CreateDate = DateTime.Now;
            _commentRepo.AddAsync(commentAdd);

            return Ok(_mapper.Map<ViewCommentDto>(commentAdd));
        }
    }
}
