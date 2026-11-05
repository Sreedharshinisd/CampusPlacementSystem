using CampusPlacementSystem.Constants;
using CampusPlacementSystem.Enums;
using CampusPlacementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementSystem.Data.Seeders;

public class DbInitializer
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public DbInitializer(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task InitializeAsync()
    {
        await SeedRolesAsync();
        await SeedSuperAdminAsync();
    }

    private async Task SeedRolesAsync()
    {
        string[] roles =
        {
            AppRoles.SuperAdmin,
            AppRoles.PlacementOfficer,
            AppRoles.CompanyHR,
            AppRoles.Student
        };

        foreach (string role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var result = await _roleManager.CreateAsync(
                    new IdentityRole(role));

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to create role: {role}");
                }
            }
        }
    }

    private async Task SeedSuperAdminAsync()
    {
        string? email = _configuration["SuperAdmin:Email"];
        string? password = _configuration["SuperAdmin:Password"];
        string? fullName = _configuration["SuperAdmin:FullName"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(fullName))
        {
            throw new InvalidOperationException(
                "SuperAdmin credentials are missing from configuration.");
        }

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            if (!await _userManager.IsInRoleAsync(
                existingUser, AppRoles.SuperAdmin))
            {
                throw new InvalidOperationException(
                    "The configured SuperAdmin email already belongs to another account.");
            }

            return;
        }

        var superAdmin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            EmailConfirmed = true,
            AccountStatus = AccountStatus.Approved
        };

        var createResult = await _userManager.CreateAsync(
            superAdmin, password);

        if (!createResult.Succeeded)
        {
            string errors = string.Join(
                ", ",
                createResult.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Failed to create SuperAdmin: {errors}");
        }

        var roleResult = await _userManager.AddToRoleAsync(
            superAdmin, AppRoles.SuperAdmin);

        if (!roleResult.Succeeded)
        {
            string errors = string.Join(
                ", ",
                roleResult.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Failed to assign SuperAdmin role: {errors}");
        }
    }
}