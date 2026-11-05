
using CampusPlacementSystem.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccessController : ControllerBase
{
    [HttpGet("authenticated")]
    [Authorize]
    public IActionResult Authenticated()
    {
        return Ok(new
        {
            message = "You are authenticated."
        });
    }

    [HttpGet("super-admin")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public IActionResult SuperAdmin()
    {
        return Ok(new
        {
            message = "SuperAdmin access granted."
        });
    }

    [HttpGet("placement-officer")]
    [Authorize(Roles = AppRoles.PlacementOfficer)]
    public IActionResult PlacementOfficer()
    {
        return Ok(new
        {
            message = "PlacementOfficer access granted."
        });
    }

    [HttpGet("company-hr")]
    [Authorize(Roles = AppRoles.CompanyHR)]
    public IActionResult CompanyHR()
    {
        return Ok(new
        {
            message = "CompanyHR access granted."
        });
    }

    [HttpGet("student")]
    [Authorize(Roles = AppRoles.Student)]
    public IActionResult Student()
    {
        return Ok(new
        {
            message = "Student access granted."
        });
    }
}
