using AccessDatas;
using BusinessObjects.Models;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Repositories
{
    public interface IArticleRepository : IRepository<Article>
    {
        public IEnumerable<Article> GetLastestArticleInDay(int statusProcess);
        public IEnumerable<Article> GetAllArticlePublishOrUnPublish(bool ispublish, int statusProcess);
        public IEnumerable<Article> GetAllArticleByKey(string keySearch, int statusProcess);
        public IEnumerable<Article> GetAllByCategoryId(Guid idCategory, int statusProcess);
        public IEnumerable<Article> GetAllFromDateToDate(DateTime from, DateTime to, int statusProcess);
		
		///
		public Article GetArticleBySlug(string slug, int statusProcess);
        Task<Article> AddArticle(Article article);
        Task<Article> DeleteArticle(Guid articleid);
       Article UpdateArticle(Article article);



    }

    public class ArticleRepository : RepositoryBase<Article>, IArticleRepository
    {
        private string[] includes = new string[]
            {
                "AuthorNavigation","CategoriesArticle", "Comments","Views"
            };
        public ArticleRepository(FinalProjectPRN231Context context) : base(context)
        {

        }
        public IEnumerable<Article> GetAllFromDateToDate(DateTime from, DateTime to, int statusProcess)
        {

            IEnumerable<Article> articles = GetMulti(
                 predicate: a => a.IsPublish == true
                 && a.StatusProcess == statusProcess
                 && (from == null || a.CreatedDate.Date >= from.Date)
                 && (to == null || a.CreatedDate.Date <= to)
                 ,
                 includes: includes
                 );

            return articles;
        }
        public IEnumerable<Article> GetAllArticleByKey(string keySearch, int statusProcess)
        {
            keySearch = keySearch.ToLower();
            IEnumerable<Article> articles = GetMulti(
                 predicate: a => a.IsPublish == true
                 && a.StatusProcess == statusProcess
                 && a.Title.ToLower().Contains(keySearch)
                 & a.ShortDescription.ToLower().Contains(keySearch)
                 ,
                 includes: includes
                 );
            return articles;
        }

        public IEnumerable<Article> GetAllArticlePublishOrUnPublish(bool ispublish, int statusProcess)
        {
            IEnumerable<Article> articles = GetMulti(
                predicate: a => a.IsPublish == ispublish
                && a.StatusProcess == statusProcess
                ,
                includes: includes
                );
            return articles;
        }

        public IEnumerable<Article> GetAllByCategoryId(Guid idCategory, int statusProcess)
        {
            IEnumerable<Article> result = GetMulti(
                predicate: a => a.IsPublish == true
                && a.CategortyId == idCategory
                && a.StatusProcess == statusProcess
                ,
                includes: includes
                );

            return result;
        }

        public IEnumerable<Article> GetLastestArticleInDay(int statusProcess)
        {
            DateTime dateNow = DateTime.Now.Date;

            IEnumerable<Article> result = GetMulti(
                predicate: a => a.IsPublish == true
                && a.PublishDate.Value.Date.Equals(dateNow.Date)
                && a.StatusProcess == statusProcess
                ,
                includes: includes
                );

            return result;
        }

        public Article GetArticleBySlug(string slug, int statusProcess)
        {
            var article = GetSingleByCondition(
                 expression: a => a.Slug == slug && a.StatusProcess == statusProcess
                , includes: includes);
            return article.Result;
        }
        public async Task<Article> AddArticle(Article article)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response = await _context.Articles.AddAsync(article);
                await _context.SaveChangesAsync();
                return response.Entity;
            }
        }

        public async Task<Article> DeleteArticle(Guid articleid)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var article = await _context.Articles.FirstOrDefaultAsync(c => c.ArticleId.Equals(articleid));
                if (article == null)
                {
                    return null;
                }
                var dropEmotions = _context.DropEmotions.Where(c => c.ArticleId == articleid);
                if (dropEmotions.Any())
                {
                    _context.DropEmotions.RemoveRange(dropEmotions);
                    await _context.SaveChangesAsync();
                }
                var saveArticle = _context.SaveArticles.Where(c => c.ArticleId.Equals(articleid));
                if (saveArticle.Any())
                {
                    _context.SaveArticles.RemoveRange(saveArticle);
                    await _context.SaveChangesAsync();
                }
                var articlepermission = _context.ArticlePermissions.Where(c => c.ArticleId.Equals(articleid));
                if (articlepermission.Any())
                {
                    _context.ArticlePermissions.RemoveRange(articlepermission);
                    await _context.SaveChangesAsync();
                }
                var articleView = _context.Views.Where(c => c.ArticleId.Equals(c.ArticleId));
                if (articleView.Any())
                {
                    _context.Views.RemoveRange(articleView);
                    await _context.SaveChangesAsync();
                }
               await DeleteCommentsForArticle(articleid);

                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();

                return article;

            }
        }
        public Article UpdateArticle(Article article)
        {
            using (FinalProjectPRN231Context _context = new FinalProjectPRN231Context())
            {
                var response = _context.Articles.Update(article);
                _context.SaveChanges();
                return response.Entity;
            }
        }


        public async Task DeleteCommentsForArticle(Guid articleId)
        {
            using (var context = new FinalProjectPRN231Context())
            {
                // Tải tất cả các comment của bài báo
                var comments = await context.Comments
                                            .Where(c => c.ArticleId == articleId && c.ReplyFor == null)
                                            .Include(c => c.InverseReplyForNavigation)
                                            .ToListAsync();
                if (comments.Any())
                {
                    // Xóa các comment đệ quy
                    foreach (var comment in comments)
                    {
                        await DeleteCommentRecursively(context, comment);
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    await context.SaveChangesAsync();
                }

            }
        }

        private async Task DeleteCommentRecursively(FinalProjectPRN231Context context, Comment comment)
        {
            // Tải các comment con
            var replies = await context.Comments
                                       .Where(c => c.ReplyFor == comment.CommentId)
                                       .Include(c => c.InverseReplyForNavigation)
                                       .ToListAsync();

            // Gọi đệ quy để xóa các comment con trước
            foreach (var reply in replies)
            {
                if (reply.InverseReplyForNavigation.Any())
                {
                    await DeleteCommentRecursively(context, reply);
                }
                else
                {
                    context.Comments.Remove(reply);
                    await context.SaveChangesAsync();
                }

            }

            // Xóa comment hiện tại
            context.Comments.Remove(comment);
            await context.SaveChangesAsync();
        }

		
	}
}
