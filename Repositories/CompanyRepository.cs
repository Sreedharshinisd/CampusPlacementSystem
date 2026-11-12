
using CampusPlacementSystem.Data;
using CampusPlacementSystem.Enums;
using CampusPlacementSystem.Models;
using CampusPlacementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementSystem.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Company?> GetByIdAsync(int companyId)
    {
        return await _context.Companies
            .FirstOrDefaultAsync(c => c.CompanyId == companyId);
    }

    public async Task<IEnumerable<Company>> GetAllAsync()
    {
        return await _context.Companies
            .AsNoTracking()
            .OrderBy(c => c.CompanyName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Company>> GetPendingAsync()
    {
        return await _context.Companies
            .AsNoTracking()
            .Where(c => c.Status == CompanyStatus.Pending)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> CompanyCodeExistsAsync(string companyCode)
    {
        return await _context.Companies
            .AnyAsync(c => c.CompanyCode == companyCode);
    }

    public async Task<bool> CompanyCodeExistsAsync(
        string companyCode, int excludeCompanyId)
    {
        return await _context.Companies.AnyAsync(c =>
            c.CompanyCode == companyCode &&
            c.CompanyId != excludeCompanyId);
    }

    public async Task AddAsync(Company company)
    {
        await _context.Companies.AddAsync(company);
    }

    public void Update(Company company)
    {
        _context.Companies.Update(company);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
