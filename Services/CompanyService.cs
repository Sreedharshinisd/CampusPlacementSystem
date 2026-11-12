
using CampusPlacementSystem.DTOs.Company;
using CampusPlacementSystem.Enums;
using CampusPlacementSystem.Models;
using CampusPlacementSystem.Repositories.Interfaces;
using CampusPlacementSystem.Services.Interfaces;

namespace CampusPlacementSystem.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _repository;

    public CompanyService(ICompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task<CompanyResponseDto> RegisterCompanyAsync(
        CompanyRegistrationRequestDto request)
    {
        string code = request.CompanyCode.Trim().ToUpperInvariant();

        if (await _repository.CompanyCodeExistsAsync(code))
            throw new InvalidOperationException("Company code already exists.");

        var company = new Company
        {
            CompanyName = request.CompanyName.Trim(),
            CompanyCode = code,
            Industry = request.Industry.Trim(),
            Website = request.Website?.Trim(),
            Description = request.Description?.Trim(),
            Address = request.Address.Trim(),
            ContactEmail = request.ContactEmail.Trim(),
            ContactPhone = request.ContactPhone.Trim(),
            Status = CompanyStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(company);
        await _repository.SaveChangesAsync();

        return MapToResponse(company);
    }

    public async Task<IEnumerable<CompanyResponseDto>> GetAllCompaniesAsync()
    {
        var companies = await _repository.GetAllAsync();
        return companies.Select(MapToResponse);
    }

    public async Task<IEnumerable<CompanyResponseDto>> GetPendingCompaniesAsync()
    {
        var companies = await _repository.GetPendingAsync();
        return companies.Select(MapToResponse);
    }

    public async Task<CompanyResponseDto?> GetCompanyByIdAsync(int companyId)
    {
        var company = await _repository.GetByIdAsync(companyId);
        return company == null ? null : MapToResponse(company);
    }

    public async Task<CompanyResponseDto> UpdateCompanyAsync(
        int companyId, CompanyUpdateRequestDto request)
    {
        var company = await GetCompanyOrThrowAsync(companyId);

        company.CompanyName = request.CompanyName.Trim();
        company.Industry = request.Industry.Trim();
        company.Website = request.Website?.Trim();
        company.Description = request.Description?.Trim();
        company.Address = request.Address.Trim();
        company.ContactEmail = request.ContactEmail.Trim();
        company.ContactPhone = request.ContactPhone.Trim();
        company.UpdatedAt = DateTime.UtcNow;

        _repository.Update(company);
        await _repository.SaveChangesAsync();

        return MapToResponse(company);
    }

    public async Task<CompanyResponseDto> ApproveCompanyAsync(int companyId)
    {
        var company = await GetCompanyOrThrowAsync(companyId);

        if (company.Status != CompanyStatus.Pending)
            throw new InvalidOperationException("Only pending companies can be approved.");

        company.Status = CompanyStatus.Approved;
        company.UpdatedAt = DateTime.UtcNow;

        _repository.Update(company);
        await _repository.SaveChangesAsync();

        return MapToResponse(company);
    }

    public async Task<CompanyResponseDto> RejectCompanyAsync(int companyId)
    {
        var company = await GetCompanyOrThrowAsync(companyId);

        if (company.Status != CompanyStatus.Pending)
            throw new InvalidOperationException("Only pending companies can be rejected.");

        company.Status = CompanyStatus.Rejected;
        company.UpdatedAt = DateTime.UtcNow;

        _repository.Update(company);
        await _repository.SaveChangesAsync();

        return MapToResponse(company);
    }

    public async Task<CompanyResponseDto> SuspendCompanyAsync(int companyId)
    {
        var company = await GetCompanyOrThrowAsync(companyId);

        if (company.Status != CompanyStatus.Approved)
            throw new InvalidOperationException("Only approved companies can be suspended.");

        company.Status = CompanyStatus.Suspended;
        company.UpdatedAt = DateTime.UtcNow;

        _repository.Update(company);
        await _repository.SaveChangesAsync();

        return MapToResponse(company);
    }

    private async Task<Company> GetCompanyOrThrowAsync(int companyId)
    {
        var company = await _repository.GetByIdAsync(companyId);

        if (company == null)
            throw new KeyNotFoundException($"Company with ID {companyId} was not found.");

        return company;
    }

    private static CompanyResponseDto MapToResponse(Company company)
    {
        return new CompanyResponseDto
        {
            CompanyId = company.CompanyId,
            CompanyName = company.CompanyName,
            CompanyCode = company.CompanyCode,
            Industry = company.Industry,
            Website = company.Website,
            Description = company.Description,
            Address = company.Address,
            ContactEmail = company.ContactEmail,
            ContactPhone = company.ContactPhone,
            Status = company.Status,
            CreatedAt = company.CreatedAt
        };
    }
}
