
using CampusPlacementSystem.Models;

namespace CampusPlacementSystem.Repositories.Interfaces;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int studentId);

    Task<Student?> GetByUserIdAsync(string userId);

    Task<IEnumerable<Student>> GetAllAsync();

    Task<IEnumerable<Student>> GetByCollegeIdAsync(int collegeId);

    Task<IEnumerable<Student>> GetPendingAsync();

    Task<bool> RegisterNumberExistsAsync(
        int collegeId, string registerNumber);

    Task<bool> RegisterNumberExistsAsync(
        int collegeId, string registerNumber, int excludeStudentId);

    Task AddAsync(Student student);

    void Update(Student student);

    Task SaveChangesAsync();
}
