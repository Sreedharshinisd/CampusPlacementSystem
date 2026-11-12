
using CampusPlacementSystem.Models;

namespace CampusPlacementSystem.Repositories.Interfaces;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(int companyId);
    Task<IEnumerable<Company>> GetAllAsync();
    Task<IEnumerable<Company>> GetPendingAsync();

    Task<bool> CompanyCodeExistsAsync(string companyCode);
    Task<bool> CompanyCodeExistsAsync(string companyCode, int excludeCompanyId);

    Task AddAsync(Company company);
    void Update(Company company);
    Task SaveChangesAsync();
}
