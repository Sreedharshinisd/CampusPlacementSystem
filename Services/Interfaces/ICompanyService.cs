
using CampusPlacementSystem.DTOs.Company;

namespace CampusPlacementSystem.Services.Interfaces;

public interface ICompanyService
{
    Task<CompanyResponseDto> RegisterCompanyAsync(CompanyRegistrationRequestDto request);
    Task<IEnumerable<CompanyResponseDto>> GetAllCompaniesAsync();
    Task<IEnumerable<CompanyResponseDto>> GetPendingCompaniesAsync();
    Task<CompanyResponseDto?> GetCompanyByIdAsync(int companyId);
    Task<CompanyResponseDto> UpdateCompanyAsync(int companyId, CompanyUpdateRequestDto request);
    Task<CompanyResponseDto> ApproveCompanyAsync(int companyId);
    Task<CompanyResponseDto> RejectCompanyAsync(int companyId);
    Task<CompanyResponseDto> SuspendCompanyAsync(int companyId);
}
