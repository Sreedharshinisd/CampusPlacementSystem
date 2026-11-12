
using CampusPlacementSystem.Constants;
using CampusPlacementSystem.DTOs.Company;
using CampusPlacementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompaniesController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpPost("register")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> Register(CompanyRegistrationRequestDto request)
    {
        var result = await _companyService.RegisterCompanyAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { companyId = result.CompanyId },
            result);
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.SuperAdmin + "," + AppRoles.PlacementOfficer)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _companyService.GetAllCompaniesAsync());
    }

    [HttpGet("pending")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> GetPending()
    {
        return Ok(await _companyService.GetPendingCompaniesAsync());
    }

    [HttpGet("{companyId:int}")]
    [Authorize(Roles = AppRoles.SuperAdmin + "," + AppRoles.PlacementOfficer + "," + AppRoles.CompanyHR)]
    public async Task<IActionResult> GetById(int companyId)
    {
        var result = await _companyService.GetCompanyByIdAsync(companyId);

        if (result == null)
            return NotFound(new { message = "Company not found." });

        return Ok(result);
    }

    [HttpPut("{companyId:int}")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> Update(
        int companyId, CompanyUpdateRequestDto request)
    {
        return Ok(await _companyService.UpdateCompanyAsync(companyId, request));
    }

    [HttpPut("{companyId:int}/approve")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> Approve(int companyId)
    {
        return Ok(await _companyService.ApproveCompanyAsync(companyId));
    }

    [HttpPut("{companyId:int}/reject")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> Reject(int companyId)
    {
        return Ok(await _companyService.RejectCompanyAsync(companyId));
    }

    [HttpPut("{companyId:int}/suspend")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> Suspend(int companyId)
    {
        return Ok(await _companyService.SuspendCompanyAsync(companyId));
    }
}
