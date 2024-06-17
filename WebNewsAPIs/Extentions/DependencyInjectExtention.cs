using Repositories;
using WebNewsAPIs.Services;

namespace WebNewsAPIs.Extentions
{
    public static class DependencyInjectExtention
    {
        public static void InjectService(this IServiceCollection services)
        {
            services.AddTransient<IArticlePermissionRepository, ArticlePermissionRepository>();
            services.AddTransient<IArticleRepository, ArticleRepository>();
            services.AddTransient<ICategoriesArticleRepository, CategoriesArticleRepository>();
            services.AddTransient<ICommentRepository, CommentRepository>();
            services.AddTransient<IDropEmotionRepository, DropEmotionRepository>();
            services.AddTransient<IEmotionRepository, EmotionRepository>();
            services.AddTransient<IFollowRepository, FollowRepository>();
            services.AddTransient<IPermissionRepository, PermissionRepository>();   
            services.AddTransient<IProcessStatusRepository, ProcessStatusRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<ISaveArticleRepository,  SaveArticleRepository>();    
            services.AddTransient<IUserRepository,  UserRepository>();
            services.AddTransient<IViewRepository, ViewRepository>();

            //add Service
            services.AddTransient<ArticleService>();
        }
    }
}
