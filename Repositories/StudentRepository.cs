
using CampusPlacementSystem.Data;
using CampusPlacementSystem.Enums;
using CampusPlacementSystem.Models;
using CampusPlacementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementSystem.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(int studentId)
    {
        return await _context.Students
            .Include(s => s.User)
            .Include(s => s.College)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);
    }

    public async Task<Student?> GetByUserIdAsync(string userId)
    {
        return await _context.Students
            .Include(s => s.User)
            .Include(s => s.College)
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _context.Students
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.College)
            .OrderBy(s => s.User.FullName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Student>> GetByCollegeIdAsync(int collegeId)
    {
        return await _context.Students
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.College)
            .Where(s => s.CollegeId == collegeId)
            .OrderBy(s => s.User.FullName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Student>> GetPendingAsync()
    {
        return await _context.Students
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.College)
            .Where(s => s.Status == StudentStatus.Pending)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> RegisterNumberExistsAsync(
        int collegeId, string registerNumber)
    {
        return await _context.Students.AnyAsync(s =>
            s.CollegeId == collegeId &&
            s.RegisterNumber == registerNumber);
    }

    public async Task<bool> RegisterNumberExistsAsync(
        int collegeId,
        string registerNumber,
        int excludeStudentId)
    {
        return await _context.Students.AnyAsync(s =>
            s.CollegeId == collegeId &&
            s.RegisterNumber == registerNumber &&
            s.StudentId != excludeStudentId);
    }

    public async Task AddAsync(Student student)
    {
        await _context.Students.AddAsync(student);
    }

    public void Update(Student student)
    {
        _context.Students.Update(student);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
