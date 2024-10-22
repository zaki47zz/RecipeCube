using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecipeCubeWebService.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RecipeCubeWebService.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using RecipeCubeWebService.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<RecipeCubeContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("RecipeCube"));
});

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

void ConfigureServices(IServiceCollection services)
{
    // JWT 認證配置
    var key = Encoding.ASCII.GetBytes("3f50a33d7bca2e1a346f5a8c4f5b7e9a");
    services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

    services.AddControllers();
}

void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseAuthentication(); // 必須啟用 JWT 驗證
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}


//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
