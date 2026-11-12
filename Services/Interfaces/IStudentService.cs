
using CampusPlacementSystem.DTOs.Student;

namespace CampusPlacementSystem.Services.Interfaces;

public interface IStudentService
{
    Task<StudentResponseDto> RegisterStudentAsync(
        StudentRegistrationRequestDto request);

    Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync();

    Task<IEnumerable<StudentResponseDto>> GetPendingStudentsAsync();

    Task<IEnumerable<StudentResponseDto>> GetStudentsByCollegeAsync(int collegeId);

    Task<StudentResponseDto?> GetStudentByIdAsync(int studentId);

    Task<StudentResponseDto?> GetMyProfileAsync(string userId);

    Task<StudentResponseDto> UpdateStudentAsync(
        int studentId,
        StudentUpdateRequestDto request);

    Task<StudentResponseDto> ApproveStudentAsync(int studentId);

    Task<StudentResponseDto> RejectStudentAsync(int studentId);
}
