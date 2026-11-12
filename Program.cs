using CampusPlacementSystem.Repositories;
using CampusPlacementSystem.Repositories.Interfaces;
using CampusPlacementSystem.Data;
using CampusPlacementSystem.Data.Seeders;
using CampusPlacementSystem.Models;
using CampusPlacementSystem.Services;
using CampusPlacementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT settings
var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT settings are missing.");

if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey) ||
    Encoding.UTF8.GetByteCount(jwtSettings.SecretKey) < 32)
{
    throw new InvalidOperationException(
        "JWT secret key must contain at least 32 bytes.");
}
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

// JWT authentication
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Register database initializer
// Register application services
builder.Services.AddScoped<DbInitializer>();

// Authentication
builder.Services.AddScoped<IAuthService, AuthService>();

// College module
builder.Services.AddScoped<ICollegeRepository, CollegeRepository>();
builder.Services.AddScoped<ICollegeService, CollegeService>();

// Student module
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
// Controllers
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// Initialize roles and SuperAdmin
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider
        .GetRequiredService<DbInitializer>();

    await dbInitializer.InitializeAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();