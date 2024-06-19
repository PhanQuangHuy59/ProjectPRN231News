using AutoMapper;
using BusinessObjects;
using WebNewsAPIs.Dtos;
using BusinessObjects.Models;

namespace ProjectAPIAss.MapperConfig

{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {

            //Article
            CreateMap<AddArticleDto, Article>();
            CreateMap<UpdateArticleDto, Article>();
            CreateMap<Article, ViewArticleDto>()
                .ForMember(des => des.CategortyName
                , act =>
                {
                    act.MapFrom(src => src.Categorty.CategoryName);
                })
                .ForMember(des => des.ProcessorName
                , act =>
                {
                    act.MapFrom(src => src.ProcessorNavigation.DisplayName);
                })
                 .ForMember(des => des.AuthorName
                , act =>
                {
                    act.MapFrom(src => src.AuthorNavigation.DisplayName);
                });

            //Article
            CreateMap<AddArticlePermissionDto, ArticlePermission>();
            CreateMap<UpdateArticlePermissionDto, ArticlePermission>();
            CreateMap<ArticlePermission, ViewArticlePermissionDto>()
                .ForMember(des => des.Username
                , act =>
                {
                    act.MapFrom(src => src.User.DisplayName);
                })
                .ForMember(des => des.ArticleName
                , act =>
                {
                    act.MapFrom(src => src.Article.Title);
                })
                 .ForMember(des => des.PermissionTypeName
                , act =>
                {
                    act.MapFrom(src => src.PermissionTypeNavigation.PermisstionName);
                });
            //Article
            CreateMap<AddArticlePermissionDto, ArticlePermission>();
            CreateMap<UpdateArticlePermissionDto, ArticlePermission>();
            CreateMap<ArticlePermission, ViewArticlePermissionDto>()
                .ForMember(des => des.Username
                , act =>
                {
                    act.MapFrom(src => src.User.DisplayName);
                })
                .ForMember(des => des.ArticleName
                , act =>
                {
                    act.MapFrom(src => src.Article.Title);
                })
                 .ForMember(des => des.PermissionTypeName
                , act =>
                {
                    act.MapFrom(src => src.PermissionTypeNavigation.PermisstionName);
                });
            //Category Article
            CreateMap<AddCategoriesArticleDto, ArticlePermission>();
            CreateMap<UpdateCategoriesArticleDto, ArticlePermission>();
            CreateMap<CategoriesArticle, ViewCategoriesArticleDto>()
                .ForMember(des => des.ParentCategoryName
                , act =>
                {
                    act.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.CategoryName : null);
                })
                ;

            //Comment
            CreateMap<AddCommentDto, Comment>();
            CreateMap<UpdateCommentDto, Comment>();
            CreateMap<AddCommentStringDto, Comment>()
                .ForMember(des => des.ArticleId
                , act =>
                {
                    act.MapFrom(src => Guid.Parse(src.ArticleId));
                }).ForMember(des => des.UserId
                , act =>
                {
                    act.MapFrom(src => Guid.Parse(src.UserId));
                })
                .ForMember(des => des.ReplyFor
                , act =>
                {
                    act.MapFrom(src => src.ReplyFor == -1 ? null: src.ReplyFor);
                });

            CreateMap<Comment, ViewCommentDto>()
                .ForMember(des => des.UserName
                , act =>
                {
                    act.MapFrom(src => src.User != null ? src.User.DisplayName : null);
                })
                 .ForMember(des => des.ArticleName
                , act =>
                {
                    act.MapFrom(src => src.Article != null ? src.Article.Title : null);
                })
                 .ForMember(des => des.UserReplyName
                , act =>
                {
                    act.MapFrom(src => src.UserIdReplyNavigation != null ? src.UserIdReplyNavigation.DisplayName : null);
                }).ForMember(des => des.InverseReplyForNavigation
                , act =>
                {
                    act.MapFrom(src => src.InverseReplyForNavigation);
                });


            //Drop Emotion
            CreateMap<AddDropEmotionDto, DropEmotion>();
            CreateMap<UpdateDropEmotionDto, DropEmotion>();
            CreateMap<DropEmotion, ViewDropEmotionDto>()
                .ForMember(des => des.ArticleName
                , act =>
                {
                    act.MapFrom(src => src.Article != null ? src.Article.Title : null);
                })
                 .ForMember(des => des.UserName
                , act =>
                {
                    act.MapFrom(src => src.User != null ? src.User.DisplayName : null);
                })
                 .ForMember(des => des.EmotionName
                , act =>
                {
                    act.MapFrom(src => src.Emotion != null ? src.Emotion.NameEmotion : null);
                });
            // Emotion
            CreateMap<AddEmotionDto, DropEmotion>();
            CreateMap<UpdateEmotionDto, DropEmotion>();
            CreateMap<DropEmotion, ViewEmotionDto>();
            // Follow
            CreateMap<AddFollowDto, DropEmotion>();
            CreateMap<UpdateEmotionDto, DropEmotion>();
            CreateMap<Follow, ViewFollowDto>()
                 .ForMember(des => des.FollowIdName
                , act =>
                {
                    act.MapFrom(src => src.FollowId != null ? src.FollowIdByNavigation.DisplayName : null);
                })
                 .ForMember(des => des.FollowIdByName
                , act =>
                {
                    act.MapFrom(src => src.FollowIdByNavigation != null ? src.FollowIdByNavigation.DisplayName : null);
                });
            // Permission
            CreateMap<AddPermissionDto, Permission>();
            CreateMap<UpdatePermissionDto, Permission>();
            CreateMap<Permission, ViewPermissionDto>();
            // Permission
            CreateMap<AddProcessStatusDto, ProcessStatus>();
            CreateMap<UpdateProcessStatusDto, ProcessStatus>();
            CreateMap<ProcessStatus, ViewProcessStatusDto>();
            // Permission
            CreateMap<AddRoleDto, Role>();
            CreateMap<UpdateRoleDto, Role>();
            CreateMap<Role, ViewRoleDto>();
            // Permission
            CreateMap<AddSaveArticleDto, SaveArticle>();
            CreateMap<UpdateSaveArticleDto, SaveArticle>();
            CreateMap<SaveArticle, ViewSaveArticleDto>()
                 .ForMember(des => des.ArticleName
                , act =>
                {
                    act.MapFrom(src => src.Article != null ? src.Article.Title : null);
                })
                  .ForMember(des => des.UserName
                , act =>
                {
                    act.MapFrom(src => src.User != null ? src.User.DisplayName : null);
                });
            // User
            CreateMap<AddUserDto, User>();
            CreateMap<UpdateSaveArticleDto, User>();
            CreateMap<User, ViewUserDto>()
                 .ForMember(des => des.RoleName
                , act =>
                {
                    act.MapFrom(src => src.Role != null ? src.Role.Rolename : null);
                });
                 // View
            CreateMap<AddViewDto, View>();
            CreateMap<UpdateViewDto, View>();
            CreateMap<View, ViewViewDto>()
                 .ForMember(des => des.UserName
                , act =>
                {
                    act.MapFrom(src => src.User != null ? src.User.Username : null);
                }).ForMember(des => des.ArticleName
                , act =>
                {
                    act.MapFrom(src => src.Article != null ? src.Article.Title : null);
                });



            //CreateMap<Transcript, TranscriptDTOs>()
            //    .ForMember(des => des.StudentName, act => { act.MapFrom(src => src.Student.StudentName); })
            //    .ForMember(des => des.SubjectName, act => { act.MapFrom(src => src.Subject.SubjectName); })
            //    ;


        }
    }
}
