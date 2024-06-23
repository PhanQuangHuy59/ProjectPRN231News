using AccessDatas;
using BusinessObjects.Models;
using Castle.Core.Logging;
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
           
            IEnumerable <Article> articles = GetMulti(
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
                , includes:includes);
            return article.Result;
        }
    }
}
