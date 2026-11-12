
using CampusPlacementSystem.DTOs.College;

namespace CampusPlacementSystem.Services.Interfaces;

public interface ICollegeService
{
    Task<CollegeResponseDto> RegisterCollegeAsync(
        CollegeRegistrationRequestDto request);

    Task<IEnumerable<CollegeResponseDto>> GetAllCollegesAsync();

    Task<IEnumerable<CollegeResponseDto>> GetPendingCollegesAsync();

    Task<CollegeResponseDto?> GetCollegeByIdAsync(int collegeId);

    Task<CollegeResponseDto> UpdateCollegeAsync(
        int collegeId,
        CollegeUpdateRequestDto request);

    Task<CollegeResponseDto> ApproveCollegeAsync(int collegeId);

    Task<CollegeResponseDto> RejectCollegeAsync(int collegeId);

    Task<CollegeResponseDto> SuspendCollegeAsync(int collegeId);
}
