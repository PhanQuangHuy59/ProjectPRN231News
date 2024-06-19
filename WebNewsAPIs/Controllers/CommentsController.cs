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
            var checkUser = _userRepo.GetSingleByCondition(c => c.UserId.ToString().Equals(comment.UserId)).Result;
            if (checkUser == null){
                return StatusCode(404, "Không tìm thấy User");
            }

            var commentAdd = _mapper.Map<Comment>(comment);
            commentAdd.CreateDate = DateTime.Now;
            commentAdd = _commentRepo.AddAsync(commentAdd).Result;
            commentAdd.User = checkUser;

            return Ok(_mapper.Map<ViewCommentDto>(commentAdd));
        }

        [HttpGet("GetCommentOfArticle")]
        public  ActionResult<List<ViewCommentDto>> GetCommentOfArticle(Guid id)
        {
            if (_commentRepo == null || _mapper == null)
            {
                return StatusCode(500, "Dịch vụ hệ thống đang gặp lỗi xin hãy chờ đợi");
            }
            if (!ModelState.IsValid)
            {
                return StatusCode(400, "Dữ liệu của bạn không thỏa mãn");
            }
            string[] includes = new string[]
            {
                nameof(Comment.User),
                nameof(Comment.InverseReplyForNavigation)

            };
            var commentOfArticle = _commentRepo.GetMulti(c => c.ArticleId.Equals(id), includes);
            var listCommentRespont = _mapper.Map<List<ViewCommentDto>>(commentOfArticle);
            
            return Ok(listCommentRespont);
        }
    }
}
