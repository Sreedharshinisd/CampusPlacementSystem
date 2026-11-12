
using CampusPlacementSystem.Models;

namespace CampusPlacementSystem.Repositories.Interfaces;

public interface ICollegeRepository
{
    Task<College?> GetByIdAsync(int collegeId);

    Task<IEnumerable<College>> GetAllAsync();

    Task<IEnumerable<College>> GetPendingAsync();

    Task<bool> CollegeCodeExistsAsync(string collegeCode);

    Task<bool> CollegeCodeExistsAsync(
        string collegeCode,
        int excludeCollegeId);

    Task AddAsync(College college);

    void Update(College college);

    Task SaveChangesAsync();
}
