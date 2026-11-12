
using CampusPlacementSystem.Data;
using CampusPlacementSystem.Enums;
using CampusPlacementSystem.Models;
using CampusPlacementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementSystem.Repositories;

public class CollegeRepository : ICollegeRepository
{
    private readonly ApplicationDbContext _context;

    public CollegeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<College?> GetByIdAsync(int collegeId)
    {
        return await _context.Colleges
            .FirstOrDefaultAsync(c => c.CollegeId == collegeId);
    }

    public async Task<IEnumerable<College>> GetAllAsync()
    {
        return await _context.Colleges
            .AsNoTracking()
            .OrderBy(c => c.CollegeName)
            .ToListAsync();
    }

    public async Task<IEnumerable<College>> GetPendingAsync()
    {
        return await _context.Colleges
            .AsNoTracking()
            .Where(c => c.Status == CollegeStatus.Pending)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> CollegeCodeExistsAsync(string collegeCode)
    {
        return await _context.Colleges
            .AnyAsync(c => c.CollegeCode == collegeCode);
    }

    public async Task<bool> CollegeCodeExistsAsync(
        string collegeCode,
        int excludeCollegeId)
    {
        return await _context.Colleges
            .AnyAsync(c =>
                c.CollegeCode == collegeCode &&
                c.CollegeId != excludeCollegeId);
    }

    public async Task AddAsync(College college)
    {
        await _context.Colleges.AddAsync(college);
    }

    public void Update(College college)
    {
        _context.Colleges.Update(college);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
