
using CampusPlacementSystem.DTOs.College;
using CampusPlacementSystem.Enums;
using CampusPlacementSystem.Models;
using CampusPlacementSystem.Repositories.Interfaces;
using CampusPlacementSystem.Services.Interfaces;

namespace CampusPlacementSystem.Services;

public class CollegeService : ICollegeService
{
    private readonly ICollegeRepository _collegeRepository;

    public CollegeService(ICollegeRepository collegeRepository)
    {
        _collegeRepository = collegeRepository;
    }

    public async Task<CollegeResponseDto> RegisterCollegeAsync(
        CollegeRegistrationRequestDto request)
    {
        string collegeCode = request.CollegeCode.Trim().ToUpperInvariant();

        if (await _collegeRepository.CollegeCodeExistsAsync(collegeCode))
        {
            throw new InvalidOperationException(
                "A college with this code already exists.");
        }

        var college = new College
        {
            CollegeName = request.CollegeName.Trim(),
            CollegeCode = collegeCode,
            UniversityName = request.UniversityName?.Trim(),
            OfficialEmail = request.OfficialEmail.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Address = request.Address.Trim(),
            Website = request.Website?.Trim(),
            Status = CollegeStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _collegeRepository.AddAsync(college);
        await _collegeRepository.SaveChangesAsync();

        return MapToResponse(college);
    }

    public async Task<IEnumerable<CollegeResponseDto>> GetAllCollegesAsync()
    {
        var colleges = await _collegeRepository.GetAllAsync();

        return colleges.Select(MapToResponse);
    }

    public async Task<IEnumerable<CollegeResponseDto>> GetPendingCollegesAsync()
    {
        var colleges = await _collegeRepository.GetPendingAsync();

        return colleges.Select(MapToResponse);
    }

    public async Task<CollegeResponseDto?> GetCollegeByIdAsync(int collegeId)
    {
        var college = await _collegeRepository.GetByIdAsync(collegeId);

        return college == null ? null : MapToResponse(college);
    }

    public async Task<CollegeResponseDto> UpdateCollegeAsync(
        int collegeId,
        CollegeUpdateRequestDto request)
    {
        var college = await GetCollegeOrThrowAsync(collegeId);

        string collegeCode = request.CollegeCode.Trim().ToUpperInvariant();

        if (await _collegeRepository.CollegeCodeExistsAsync(
            collegeCode, collegeId))
        {
            throw new InvalidOperationException(
                "Another college already uses this code.");
        }

        college.CollegeName = request.CollegeName.Trim();
        college.CollegeCode = collegeCode;
        college.UniversityName = request.UniversityName?.Trim();
        college.OfficialEmail = request.OfficialEmail.Trim();
        college.PhoneNumber = request.PhoneNumber.Trim();
        college.Address = request.Address.Trim();
        college.Website = request.Website?.Trim();

        _collegeRepository.Update(college);
        await _collegeRepository.SaveChangesAsync();

        return MapToResponse(college);
    }

    public async Task<CollegeResponseDto> ApproveCollegeAsync(int collegeId)
    {
        var college = await GetCollegeOrThrowAsync(collegeId);

        if (college.Status != CollegeStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending colleges can be approved.");
        }

        college.Status = CollegeStatus.Approved;

        _collegeRepository.Update(college);
        await _collegeRepository.SaveChangesAsync();

        return MapToResponse(college);
    }

    public async Task<CollegeResponseDto> RejectCollegeAsync(int collegeId)
    {
        var college = await GetCollegeOrThrowAsync(collegeId);

        if (college.Status != CollegeStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending colleges can be rejected.");
        }

        college.Status = CollegeStatus.Rejected;

        _collegeRepository.Update(college);
        await _collegeRepository.SaveChangesAsync();

        return MapToResponse(college);
    }

    public async Task<CollegeResponseDto> SuspendCollegeAsync(int collegeId)
    {
        var college = await GetCollegeOrThrowAsync(collegeId);

        if (college.Status != CollegeStatus.Approved)
        {
            throw new InvalidOperationException(
                "Only approved colleges can be suspended.");
        }

        college.Status = CollegeStatus.Suspended;

        _collegeRepository.Update(college);
        await _collegeRepository.SaveChangesAsync();

        return MapToResponse(college);
    }

    private async Task<College> GetCollegeOrThrowAsync(int collegeId)
    {
        var college = await _collegeRepository.GetByIdAsync(collegeId);

        if (college == null)
        {
            throw new KeyNotFoundException(
                $"College with ID {collegeId} was not found.");
        }

        return college;
    }

    private static CollegeResponseDto MapToResponse(College college)
    {
        return new CollegeResponseDto
        {
            CollegeId = college.CollegeId,
            CollegeName = college.CollegeName,
            CollegeCode = college.CollegeCode,
            UniversityName = college.UniversityName,
            OfficialEmail = college.OfficialEmail,
            PhoneNumber = college.PhoneNumber,
            Address = college.Address,
            Website = college.Website,
            Status = college.Status,
            CreatedAt = college.CreatedAt
        };
    }
}
