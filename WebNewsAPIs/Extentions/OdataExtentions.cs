using BusinessObjects.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Security.Permissions;

namespace WebNewsAPIs.Extentions
{
    public static class OdataExtentions
    {
        public static void ConfigOdata(this IServiceCollection services)
        {
            services.AddControllers()
                    .AddOData(builderOdata => builderOdata.Select().Filter().Count().OrderBy().Expand().SetMaxTop(100)
                    .AddRouteComponents("odata", getEdmModel()));
        }
        public static IEdmModel getEdmModel()
        {
            ODataConventionModelBuilder conventionModelBuilder = new ODataConventionModelBuilder();
            conventionModelBuilder.EntitySet<ArticlePermission>("ArticlePermissions");
            conventionModelBuilder.EntitySet<Article>("Articles");
            conventionModelBuilder.EntitySet<CategoriesArticle>("CategoriesArticles");
            conventionModelBuilder.EntitySet<Comment>("Comments");
            conventionModelBuilder.EntitySet<DropEmotion>("DropEmotions");
            conventionModelBuilder.EntitySet<Emotion>("Emotions");
            conventionModelBuilder.EntitySet<Follow>("Follows");
            conventionModelBuilder.EntitySet<Permission>("Permissions");
            conventionModelBuilder.EntitySet<ProcessStatus>("ProcessStatuss");
            conventionModelBuilder.EntitySet<Role>("Roles");
            conventionModelBuilder.EntitySet<SaveArticle>("SaveArticles");
            conventionModelBuilder.EntitySet<User>("Users");
            conventionModelBuilder.EntitySet<View>("Views");


            return conventionModelBuilder.GetEdmModel();
        }
    }
}
