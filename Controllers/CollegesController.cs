
using CampusPlacementSystem.Constants;
using CampusPlacementSystem.DTOs.College;
using CampusPlacementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollegesController : ControllerBase
{
    private readonly ICollegeService _collegeService;

    public CollegesController(ICollegeService collegeService)
    {
        _collegeService = collegeService;
    }

    // Public college registration
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterCollege(
        [FromBody] CollegeRegistrationRequestDto request)
    {
        var college = await _collegeService.RegisterCollegeAsync(request);

        return CreatedAtAction(
            nameof(GetCollegeById),
            new { collegeId = college.CollegeId },
            college);
    }

    // Get all colleges
    [HttpGet]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> GetAllColleges()
    {
        var colleges = await _collegeService.GetAllCollegesAsync();

        return Ok(colleges);
    }

    // Get pending colleges
    [HttpGet("pending")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> GetPendingColleges()
    {
        var colleges = await _collegeService.GetPendingCollegesAsync();

        return Ok(colleges);
    }

    // Get college by ID
    [HttpGet("{collegeId:int}")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> GetCollegeById(int collegeId)
    {
        var college = await _collegeService.GetCollegeByIdAsync(collegeId);

        if (college == null)
        {
            return NotFound(new
            {
                message = $"College with ID {collegeId} was not found."
            });
        }

        return Ok(college);
    }

    // Update college
    [HttpPut("{collegeId:int}")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> UpdateCollege(
        int collegeId,
        [FromBody] CollegeUpdateRequestDto request)
    {
        var college = await _collegeService.UpdateCollegeAsync(
            collegeId, request);

        return Ok(college);
    }

    // Approve college
    [HttpPut("{collegeId:int}/approve")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> ApproveCollege(int collegeId)
    {
        var college = await _collegeService.ApproveCollegeAsync(collegeId);

        return Ok(college);
    }

    // Reject college
    [HttpPut("{collegeId:int}/reject")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> RejectCollege(int collegeId)
    {
        var college = await _collegeService.RejectCollegeAsync(collegeId);

        return Ok(college);
    }

    // Suspend college
    [HttpPut("{collegeId:int}/suspend")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> SuspendCollege(int collegeId)
    {
        var college = await _collegeService.SuspendCollegeAsync(collegeId);

        return Ok(college);
    }
}
