create database FinalProjectPRN231
go 
use FinalProjectPRN231
go

CREATE TABLE "Follow"(
    "follow_id" uniqueidentifier NOT NULL,
    "follow_id_by" uniqueidentifier NOT NULL,
    "follow_date" DATETIME NOT NULL DEFAULT GETDATE()
);
ALTER TABLE
    "Follow" ADD CONSTRAINT "follow_follow_id_primary" PRIMARY KEY("follow_id","follow_id_by");

CREATE TABLE "ArticlePermissions"(
    "apermission_id" BIGINT NOT NULL IDENTITY(1,1),
    "user_id" uniqueidentifier NOT NULL,
    "article_id" uniqueidentifier NOT NULL,
    "permission_type" INT NOT NULL,
    "created_date" DATETIME NOT NULL DEFAULT GETDATE()
);
ALTER TABLE
    "ArticlePermissions" ADD CONSTRAINT "articlepermissions_apermission_id_primary" PRIMARY KEY("apermission_id");
CREATE TABLE "Categories_Article"(
    "category_id" uniqueidentifier NOT NULL DEFAULT NEWID(),
    "category_name" NVARCHAR(300) NOT NULL,
    "description" NVARCHAR(MAX) NULL,
    "created_date" DATETIME NOT NULL DEFAULT GETDATE(),
    "updated_date" DATETIME  NULL,
    "parent_category_id" uniqueidentifier NULL,
    "order_level" INT NOT NULL
);
ALTER TABLE
    "Categories_Article" ADD CONSTRAINT "categories_article_category_id_primary" PRIMARY KEY("category_id");
CREATE TABLE "Views"(
    "view_id" BIGINT NOT NULL IDENTITY(1,1),
    "article_id" uniqueidentifier NOT NULL,
    "user_id" uniqueidentifier NOT NULL,
    "view_date" DATETIME NOT NULL DEFAULT GETDATE()
);
ALTER TABLE
    "Views" ADD CONSTRAINT "views_view_id_primary" PRIMARY KEY("view_id");
CREATE TABLE "Users"(
    "user_id" uniqueidentifier NOT NULL DEFAULT NEWID(),
    "username" NVARCHAR(300) NOT NULL,
    "password" NVARCHAR(MAX) NOT NULL,
    "display_name" NVARCHAR(255) NULL,
    "roleid" uniqueidentifier NOT NULL,
    "createddate" DATETIME NOT NULL DEFAULT GETDATE(),
    "updateddate" DATETIME NULL,
    "date_of_birth" DATE  NULL,
    "gender" NVARCHAR(255)  NULL,
    "address" NVARCHAR(MAX)  NULL,
    "phone_number" NVARCHAR(255)  NULL,
    "image" NVARCHAR(MAX) NULL,
	"is_confirm" BIT NOT NULL DEFAULT 0,
	CONSTRAINT chk_gender CHECK (gender IN (N'Nam', N'Nữ', N'Khác'))
);
ALTER TABLE
    "Users" ADD CONSTRAINT "users_user_id_primary" PRIMARY KEY("user_id");
CREATE INDEX "users_username_index" ON
    "Users"("username");

CREATE INDEX "users_display_name_index" ON
    "Users"("display_name");
CREATE TABLE "Permissions"(
    "permission_id" INT NOT NULL IDENTITY(1,1),
    "permisstion_name" NVARCHAR(300) NOT NULL,
    "description" NVARCHAR(MAX) NULL
);
ALTER TABLE
    "Permissions" ADD CONSTRAINT "permissions_permission_id_primary" PRIMARY KEY("permission_id");
CREATE TABLE "Role"(
    "role_id" uniqueidentifier NOT NULL DEFAULT NEWID(),
    "rolename" NVARCHAR(255) NOT NULL,
    "createddate" DATETIME NOT NULL DEFAULT GETDATE(),
    "updateddate" DATETIME NULL,
    "description" NVARCHAR(MAX)  NULL
);
ALTER TABLE
    "Role" ADD CONSTRAINT "role_role_id_primary" PRIMARY KEY("role_id");
CREATE UNIQUE INDEX "role_rolename_unique" ON
    "Role"("rolename");
CREATE TABLE "Process_Status"(
    "process_id" INT NOT NULL,
    "name_process" NVARCHAR(300) NOT NULL,
    "description" NVARCHAR(MAX) NOT NULL,
    "role_for_process" INT NOT NULL
);
ALTER TABLE
    "Process_Status" ADD CONSTRAINT "process_status_process_id_primary" PRIMARY KEY("process_id");
CREATE TABLE "Drop_Emotions"(
    "drop_emotion_id" BIGINT NOT NULL IDENTITY(1,1),
    "article_id" uniqueidentifier NOT NULL,
    "user_id" uniqueidentifier NOT NULL,
    "drop_date" DATETIME NOT NULL DEFAULT GETDATE() ,
    "emotion_id" uniqueidentifier NOT NULL
);
ALTER TABLE
    "Drop_Emotions" ADD CONSTRAINT "drop_emotions_drop_emotion_id_primary" PRIMARY KEY("drop_emotion_id");
CREATE TABLE "Articles"(
    "article_id" uniqueidentifier NOT NULL DEFAULT NEWID(),
    "title" NVARCHAR(300) NOT NULL,
    "content" NVARCHAR(MAX) NOT NULL,
    "author" uniqueidentifier NOT NULL,
    "created_date" DATETIME NOT NULL DEFAULT GETDATE(),
    "updated_date" DATETIME  NULL,
    "publish_date" DATETIME  NULL ,
    "is_publish" BIT NOT NULL DEFAULT 0,
    "status_process" INT NOT NULL,
    "categorty_id" uniqueidentifier NOT NULL,
    "short_description" NVARCHAR(300) NOT NULL,
    "slug" VARCHAR(300) NOT NULL,
    "cover_image" VARCHAR(MAX) NOT NULL,
    "processor" uniqueidentifier NOT NULL,
    "link_audio" NVARCHAR(300)  NULL,
	"view_articles" BIGINT NULL DEFAULT 0
);
ALTER TABLE
    "Articles" ADD CONSTRAINT "articles_article_id_primary" PRIMARY KEY("article_id");
CREATE INDEX "articles_title_index" ON
    "Articles"("title");
CREATE INDEX "articles_slug_index" ON
    "Articles"("slug");
CREATE TABLE "Save_Article"(
    "save_id" BIGINT NOT NULL IDENTITY(1,1),
    "article_id" uniqueidentifier NOT NULL,
    "user_id" uniqueidentifier NOT NULL,
    "save_date" DATETIME NOT NULL DEFAULT GETDATE() 
);
ALTER TABLE
    "Save_Article" ADD CONSTRAINT "save_article_save_id_primary" PRIMARY KEY("save_id");
CREATE TABLE "Comments"(
    "comment_id" BIGINT NOT NULL IDENTITY(1,1),
    "article_id" uniqueidentifier NOT NULL,
    "user_id" uniqueidentifier NOT NULL,
    "content" NVARCHAR(MAX) NOT NULL,
    "create_date" DATETIME NOT NULL DEFAULT GETDATE(),
    "likes" BIGINT NULL DEFAULT 0,
    "dislikes" BIGINT  NULL DEFAULT 0,
    "reply_for" BIGINT  NULL,
    "user_id_reply" uniqueidentifier NULL
);
ALTER TABLE
    "Comments" ADD CONSTRAINT "comments_comment_id_primary" PRIMARY KEY("comment_id");
CREATE TABLE "Emotions"(
    "emotion_id" uniqueidentifier NOT NULL DEFAULT NEWID(),
    "name_emotion" NVARCHAR(255) NOT NULL,
    "image" NVARCHAR(MAX) NOT NULL
);
ALTER TABLE
    "Emotions" ADD CONSTRAINT "emotions_emotion_id_primary" PRIMARY KEY("emotion_id");
ALTER TABLE
    "Articles" ADD CONSTRAINT "articles_author_foreign" FOREIGN KEY("author") REFERENCES "Users"("user_id");
ALTER TABLE
    "Follow" ADD CONSTRAINT "follow_follow_id_foreign" FOREIGN KEY("follow_id") REFERENCES "Users"("user_id");
ALTER TABLE
    "Articles" ADD CONSTRAINT "articles_processor_foreign" FOREIGN KEY("processor") REFERENCES "Users"("user_id");
ALTER TABLE
    "Categories_Article" ADD CONSTRAINT "categories_article_parent_category_id_foreign" FOREIGN KEY("parent_category_id") REFERENCES "Categories_Article"("category_id");
ALTER TABLE
    "Views" ADD CONSTRAINT "views_article_id_foreign" FOREIGN KEY("article_id") REFERENCES "Articles"("article_id");
ALTER TABLE
    "Follow" ADD CONSTRAINT "follow_follow_id_by_foreign" FOREIGN KEY("follow_id_by") REFERENCES "Users"("user_id");
ALTER TABLE
    "Comments" ADD CONSTRAINT "comments_user_id_reply_foreign" FOREIGN KEY("user_id_reply") REFERENCES "Users"("user_id");
ALTER TABLE
    "ArticlePermissions" ADD CONSTRAINT "articlepermissions_permission_type_foreign" FOREIGN KEY("permission_type") REFERENCES "Permissions"("permission_id");
ALTER TABLE
    "Comments" ADD CONSTRAINT "comments_reply_for_foreign" FOREIGN KEY("reply_for") REFERENCES "Comments"("comment_id");
ALTER TABLE
    "Users" ADD CONSTRAINT "users_roleid_foreign" FOREIGN KEY("roleid") REFERENCES "Role"("role_id");
ALTER TABLE
    "Save_Article" ADD CONSTRAINT "save_article_article_id_foreign" FOREIGN KEY("article_id") REFERENCES "Articles"("article_id");
ALTER TABLE
    "ArticlePermissions" ADD CONSTRAINT "articlepermissions_article_id_foreign" FOREIGN KEY("article_id") REFERENCES "Articles"("article_id");
ALTER TABLE
    "Drop_Emotions" ADD CONSTRAINT "drop_emotions_article_id_foreign" FOREIGN KEY("article_id") REFERENCES "Articles"("article_id");
ALTER TABLE
    "Articles" ADD CONSTRAINT "articles_categorty_id_foreign" FOREIGN KEY("categorty_id") REFERENCES "Categories_Article"("category_id");
ALTER TABLE
    "Views" ADD CONSTRAINT "views_user_id_foreign" FOREIGN KEY("user_id") REFERENCES "Users"("user_id");
ALTER TABLE
    "Drop_Emotions" ADD CONSTRAINT "drop_emotions_emotion_id_foreign" FOREIGN KEY("emotion_id") REFERENCES "Emotions"("emotion_id");
ALTER TABLE
    "ArticlePermissions" ADD CONSTRAINT "articlepermissions_user_id_foreign" FOREIGN KEY("user_id") REFERENCES "Users"("user_id");
ALTER TABLE
    "Drop_Emotions" ADD CONSTRAINT "drop_emotions_user_id_foreign" FOREIGN KEY("user_id") REFERENCES "Users"("user_id");
ALTER TABLE
    "Articles" ADD CONSTRAINT "articles_status_process_foreign" FOREIGN KEY("status_process") REFERENCES "Process_Status"("process_id");
ALTER TABLE
    "Comments" ADD CONSTRAINT "comments_user_id_foreign" FOREIGN KEY("user_id") REFERENCES "Users"("user_id");
ALTER TABLE
    "Save_Article" ADD CONSTRAINT "save_article_user_id_foreign" FOREIGN KEY("user_id") REFERENCES "Users"("user_id");
ALTER TABLE
    "Comments" ADD CONSTRAINT "comments_article_id_foreign" FOREIGN KEY("article_id") REFERENCES "Articles"("article_id");