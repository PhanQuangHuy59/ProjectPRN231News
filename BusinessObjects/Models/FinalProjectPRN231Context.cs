using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace BusinessObjects.Models
{
    public partial class FinalProjectPRN231Context : DbContext
    {
        public FinalProjectPRN231Context()
        {
        }

        public FinalProjectPRN231Context(DbContextOptions<FinalProjectPRN231Context> options)
            : base(options)
        {
            
        }

        public virtual DbSet<Article> Articles { get; set; } = null!;
        public virtual DbSet<ArticlePermission> ArticlePermissions { get; set; } = null!;
        public virtual DbSet<CategoriesArticle> CategoriesArticles { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<DropEmotion> DropEmotions { get; set; } = null!;
        public virtual DbSet<Emotion> Emotions { get; set; } = null!;
        public virtual DbSet<Follow> Follows { get; set; } = null!;
        public virtual DbSet<Permission> Permissions { get; set; } = null!;
        public virtual DbSet<ProcessStatus> ProcessStatuses { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<SaveArticle> SaveArticles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<View> Views { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var ConnectionString = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json").Build().GetConnectionString("value");
                optionsBuilder.UseSqlServer(ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasIndex(e => e.Slug, "articles_slug_index");

                entity.HasIndex(e => e.Title, "articles_title_index");

                entity.Property(e => e.ArticleId)
                    .HasColumnName("article_id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Author).HasColumnName("author");

                entity.Property(e => e.CategortyId).HasColumnName("categorty_id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.CoverImage)
                    .IsUnicode(false)
                    .HasColumnName("cover_image");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsPublish).HasColumnName("is_publish");

                entity.Property(e => e.LinkAudio)
                    .HasMaxLength(300)
                    .HasColumnName("link_audio");

                entity.Property(e => e.Processor).HasColumnName("processor");

                entity.Property(e => e.PublishDate).HasColumnName("publish_date");

                entity.Property(e => e.ShortDescription)
                    .HasMaxLength(255)
                    .HasColumnName("short_description");

                entity.Property(e => e.Slug)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("slug");

                entity.Property(e => e.StatusProcess).HasColumnName("status_process");

                entity.Property(e => e.Title)
                    .HasMaxLength(300)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_date");

                entity.HasOne(d => d.AuthorNavigation)
                    .WithMany(p => p.ArticleAuthorNavigations)
                    .HasForeignKey(d => d.Author)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("articles_author_foreign");

                entity.HasOne(d => d.Categorty)
                    .WithMany(p => p.Articles)
                    .HasForeignKey(d => d.CategortyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("articles_categorty_id_foreign");

                entity.HasOne(d => d.ProcessorNavigation)
                    .WithMany(p => p.ArticleProcessorNavigations)
                    .HasForeignKey(d => d.Processor)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("articles_processor_foreign");

                entity.HasOne(d => d.StatusProcessNavigation)
                    .WithMany(p => p.Articles)
                    .HasForeignKey(d => d.StatusProcess)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("articles_status_process_foreign");
            });

            modelBuilder.Entity<ArticlePermission>(entity =>
            {
                entity.HasKey(e => e.ApermissionId)
                    .HasName("articlepermissions_apermission_id_primary");

                entity.Property(e => e.ApermissionId).HasColumnName("apermission_id");

                entity.Property(e => e.ArticleId).HasColumnName("article_id");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PermissionType).HasColumnName("permission_type");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArticlePermissions)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("articlepermissions_article_id_foreign");

                entity.HasOne(d => d.PermissionTypeNavigation)
                    .WithMany(p => p.ArticlePermissions)
                    .HasForeignKey(d => d.PermissionType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("articlepermissions_permission_type_foreign");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ArticlePermissions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("articlepermissions_user_id_foreign");
            });

            modelBuilder.Entity<CategoriesArticle>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("categories_article_category_id_primary");

                entity.ToTable("Categories_Article");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(300)
                    .HasColumnName("category_name");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.OrderLevel).HasColumnName("order_level");

                entity.Property(e => e.ParentCategoryId).HasColumnName("parent_category_id");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_date");

                entity.HasOne(d => d.ParentCategory)
                    .WithMany(p => p.InverseParentCategory)
                    .HasForeignKey(d => d.ParentCategoryId)
                    .HasConstraintName("categories_article_parent_category_id_foreign");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.CommentId).HasColumnName("comment_id");

                entity.Property(e => e.ArticleId).HasColumnName("article_id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Dislikes)
                    .HasColumnName("dislikes")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Likes)
                    .HasColumnName("likes")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ReplyFor).HasColumnName("reply_for");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserIdReply).HasColumnName("user_id_reply");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("comments_article_id_foreign");

                entity.HasOne(d => d.ReplyForNavigation)
                    .WithMany(p => p.InverseReplyForNavigation)
                    .HasForeignKey(d => d.ReplyFor)
                    .HasConstraintName("comments_reply_for_foreign");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CommentUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("comments_user_id_foreign");

                entity.HasOne(d => d.UserIdReplyNavigation)
                    .WithMany(p => p.CommentUserIdReplyNavigations)
                    .HasForeignKey(d => d.UserIdReply)
                    .HasConstraintName("comments_user_id_reply_foreign");
            });

            modelBuilder.Entity<DropEmotion>(entity =>
            {
                entity.ToTable("Drop_Emotions");

                entity.Property(e => e.DropEmotionId).HasColumnName("drop_emotion_id");

                entity.Property(e => e.ArticleId).HasColumnName("article_id");

                entity.Property(e => e.DropDate)
                    .HasColumnType("datetime")
                    .HasColumnName("drop_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EmotionId).HasColumnName("emotion_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.DropEmotions)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("drop_emotions_article_id_foreign");

                entity.HasOne(d => d.Emotion)
                    .WithMany(p => p.DropEmotions)
                    .HasForeignKey(d => d.EmotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("drop_emotions_emotion_id_foreign");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DropEmotions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("drop_emotions_user_id_foreign");
            });

            modelBuilder.Entity<Emotion>(entity =>
            {
                entity.Property(e => e.EmotionId)
                    .HasColumnName("emotion_id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Image).HasColumnName("image");

                entity.Property(e => e.NameEmotion)
                    .HasMaxLength(255)
                    .HasColumnName("name_emotion");
            });

            modelBuilder.Entity<Follow>(entity =>
            {
                entity.HasKey(e => new { e.FollowId, e.FollowIdBy })
                    .HasName("follow_follow_id_primary");

                entity.ToTable("Follow");

                entity.Property(e => e.FollowId).HasColumnName("follow_id");

                entity.Property(e => e.FollowIdBy).HasColumnName("follow_id_by");

                entity.Property(e => e.FollowDate)
                    .HasColumnType("datetime")
                    .HasColumnName("follow_date")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.FollowNavigation)
                    .WithMany(p => p.FollowFollowNavigations)
                    .HasForeignKey(d => d.FollowId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("follow_follow_id_foreign");

                entity.HasOne(d => d.FollowIdByNavigation)
                    .WithMany(p => p.FollowFollowIdByNavigations)
                    .HasForeignKey(d => d.FollowIdBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("follow_follow_id_by_foreign");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.PermissionId).HasColumnName("permission_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.PermisstionName)
                    .HasMaxLength(300)
                    .HasColumnName("permisstion_name");
            });

            modelBuilder.Entity<ProcessStatus>(entity =>
            {
                entity.HasKey(e => e.ProcessId)
                    .HasName("process_status_process_id_primary");

                entity.ToTable("Process_Status");

                entity.Property(e => e.ProcessId)
                    .ValueGeneratedNever()
                    .HasColumnName("process_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.NameProcess)
                    .HasMaxLength(300)
                    .HasColumnName("name_process");

                entity.Property(e => e.RoleForProcess).HasColumnName("role_for_process");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.HasIndex(e => e.Rolename, "role_rolename_unique")
                    .IsUnique();

                entity.Property(e => e.RoleId)
                    .HasColumnName("role_id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Rolename)
                    .HasMaxLength(255)
                    .HasColumnName("rolename");

                entity.Property(e => e.Updateddate)
                    .HasColumnType("datetime")
                    .HasColumnName("updateddate");
            });

            modelBuilder.Entity<SaveArticle>(entity =>
            {
                entity.HasKey(e => e.SaveId)
                    .HasName("save_article_save_id_primary");

                entity.ToTable("Save_Article");

                entity.Property(e => e.SaveId).HasColumnName("save_id");

                entity.Property(e => e.ArticleId).HasColumnName("article_id");

                entity.Property(e => e.SaveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("save_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.SaveArticles)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("save_article_article_id_foreign");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SaveArticles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("save_article_user_id_foreign");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.DisplayName, "users_display_name_index");

                entity.HasIndex(e => e.Username, "users_username_index");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).HasColumnName("address");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("date_of_birth");

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(255)
                    .HasColumnName("display_name");

                entity.Property(e => e.Gender)
                    .HasMaxLength(255)
                    .HasColumnName("gender");

                entity.Property(e => e.Image).HasColumnName("image");

                entity.Property(e => e.Password).HasColumnName("password");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(255)
                    .HasColumnName("phone_number");

                entity.Property(e => e.RoleId).HasColumnName("roleid");

                entity.Property(e => e.Updateddate)
                    .HasColumnType("datetime")
                    .HasColumnName("updateddate");

                entity.Property(e => e.Username)
                    .HasMaxLength(300)
                    .HasColumnName("username");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("users_roleid_foreign");
            });

            modelBuilder.Entity<View>(entity =>
            {
                entity.Property(e => e.ViewId).HasColumnName("view_id");

                entity.Property(e => e.ArticleId).HasColumnName("article_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.ViewDate)
                    .HasColumnType("datetime")
                    .HasColumnName("view_date")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.Views)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("views_article_id_foreign");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Views)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("views_user_id_foreign");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
