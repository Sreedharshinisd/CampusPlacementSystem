using CampusPlacementSystem.Data;
using CampusPlacementSystem.Data.Seeders;
using CampusPlacementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

// Register database initializer
builder.Services.AddScoped<DbInitializer>();

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