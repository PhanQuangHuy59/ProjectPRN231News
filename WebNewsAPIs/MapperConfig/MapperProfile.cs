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
            CreateMap<Article, ArticleViewDto>();
            CreateMap<ArticleCreateDto, Article>();
            CreateMap<ArticleUpdateDto, Article>();

            //Categories
            CreateMap<CategoriesArticle, CategoriesArticleViewDto>();
            CreateMap<CategoriesArticleCreateDto, CategoriesArticle>();
            CreateMap<CategoriesArticleUpdateDto, CategoriesArticle>();
            //CommentDto
            CreateMap<Comment, CommentViewDto>();
            CreateMap<CommentCreateDto, Comment>();
            CreateMap<CommentUpdateDto, Comment>();

            //Emotion
            CreateMap<Emotion, EmotionViewDto>();
            CreateMap<EmotionCreateDto, Emotion>();
            CreateMap<EmotionUpdateDto, Emotion>();

            //Permission
            CreateMap<Permission, PermissionViewDto>();
            CreateMap<PermissionCreateDto, Permission>();
            CreateMap<PermissionUpdateDto, Permission>();
            // ProcessStatus
            CreateMap<ProcessStatus, ProcessStatusViewDto>();
            CreateMap<ProcessStatusCreateDto, ProcessStatus>();
            CreateMap<ProcessStatusUpdateDto, ProcessStatus>();
            // Role
            CreateMap<Role, RoleViewDto>();
            CreateMap<RoleCreateDto, Role>();
            CreateMap<RoleUpdateDto, Role>();
            // User
            CreateMap<User, UserViewDto>();
            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>();



            //CreateMap<Transcript, TranscriptDTOs>()
            //    .ForMember(des => des.StudentName, act => { act.MapFrom(src => src.Student.StudentName); })
            //    .ForMember(des => des.SubjectName, act => { act.MapFrom(src => src.Subject.SubjectName); })
            //    ;


        }
    }
}
