using AccessDatas;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<Comment> AddComment(Comment comment);
        Task<Comment> UpdateComment(Comment comment);
    }
    public class CommentRepository :  RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(FinalProjectPRN231Context context) : base(context)
        {
        }
        public async Task<Comment> AddComment(Comment comment)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response = await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }
        public async Task<Comment> UpdateComment(Comment comment)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response =  _context.Comments.Update(comment);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }

    }
}
