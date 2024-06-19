using AutoMapper;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using ProjectAPIAss.MapperConfig;
using WebNewsAPIs.Extentions;
using WebNewsAPIs.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddDbContext<FinalProjectPRN231Context>(options =>
{
    var connectString = builder.Configuration.GetConnectionString("value");
    options.UseSqlServer(connectString);
});

builder.Services.AddControllers();
//builder.Services.AddSession(options =>
//{
//    //options.IdleTimeout = TimeSpan.FromHours(2);
//    //options.Cookie.HttpOnly = true;
//    //options.Cookie.IsEssential = true;
//});
builder.Services.ConfigOdata();
builder.Services.InjectService();
builder.Services.AddSingleton<IMapper>(MapperInstanse.GetMapper());
builder.ConfigAuthenAuthor();
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy", builder =>
    builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((host) => true));
});
//MailSetting
var mailsettings = builder.Configuration.GetSection("MailSettings");  // đọc config
builder.Services.Configure<MailSettings>(mailsettings);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CORSPolicy");

app.MapControllers();

app.Run();
