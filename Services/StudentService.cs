
using CampusPlacementSystem.Constants;
using CampusPlacementSystem.DTOs.Student;
using CampusPlacementSystem.Enums;
using CampusPlacementSystem.Models;
using CampusPlacementSystem.Repositories.Interfaces;
using CampusPlacementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementSystem.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly ICollegeRepository _collegeRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentService(
        IStudentRepository studentRepository,
        ICollegeRepository collegeRepository,
        UserManager<ApplicationUser> userManager)
    {
        _studentRepository = studentRepository;
        _collegeRepository = collegeRepository;
        _userManager = userManager;
    }

    public async Task<StudentResponseDto> RegisterStudentAsync(
        StudentRegistrationRequestDto request)
    {
        var college = await _collegeRepository.GetByIdAsync(request.CollegeId);

        if (college == null || college.Status != CollegeStatus.Approved)
        {
            throw new InvalidOperationException(
                "Student can only register under an approved college.");
        }

        string email = request.Email.Trim();
        string registerNumber = request.RegisterNumber.Trim().ToUpperInvariant();

        if (await _userManager.FindByEmailAsync(email) != null)
        {
            throw new InvalidOperationException(
                "An account with this email already exists.");
        }

        if (await _studentRepository.RegisterNumberExistsAsync(
            request.CollegeId, registerNumber))
        {
            throw new InvalidOperationException(
                "This register number already exists in the college.");
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = request.FullName.Trim(),
            AccountStatus = AccountStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var identityResult = await _userManager.CreateAsync(
            user, request.Password);

        if (!identityResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join("; ", identityResult.Errors.Select(e => e.Description)));
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user, AppRoles.Student);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            throw new InvalidOperationException(
                string.Join("; ", roleResult.Errors.Select(e => e.Description)));
        }

        var student = new Student
        {
            UserId = user.Id,
            CollegeId = request.CollegeId,
            RegisterNumber = registerNumber,
            Department = request.Department.Trim(),
            Degree = request.Degree.Trim(),
            GraduationYear = request.GraduationYear,
            CGPA = request.CGPA,
            Status = StudentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await _studentRepository.AddAsync(student);
            await _studentRepository.SaveChangesAsync();
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }

        return MapToResponse(student);
    }

    public async Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync()
    {
        var students = await _studentRepository.GetAllAsync();
        return students.Select(MapToResponse);
    }

    public async Task<IEnumerable<StudentResponseDto>> GetPendingStudentsAsync()
    {
        var students = await _studentRepository.GetPendingAsync();
        return students.Select(MapToResponse);
    }

    public async Task<IEnumerable<StudentResponseDto>> GetStudentsByCollegeAsync(
        int collegeId)
    {
        var college = await _collegeRepository.GetByIdAsync(collegeId);

        if (college == null)
        {
            throw new KeyNotFoundException("College not found.");
        }

        var students = await _studentRepository.GetByCollegeIdAsync(collegeId);
        return students.Select(MapToResponse);
    }

    public async Task<StudentResponseDto?> GetStudentByIdAsync(int studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);

        return student == null ? null : MapToResponse(student);
    }

    public async Task<StudentResponseDto?> GetMyProfileAsync(string userId)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId);

        return student == null ? null : MapToResponse(student);
    }

    public async Task<StudentResponseDto> UpdateStudentAsync(
        int studentId,
        StudentUpdateRequestDto request)
    {
        var student = await GetStudentOrThrowAsync(studentId);

        string registerNumber = request.RegisterNumber.Trim().ToUpperInvariant();

        if (await _studentRepository.RegisterNumberExistsAsync(
            student.CollegeId, registerNumber, studentId))
        {
            throw new InvalidOperationException(
                "Another student already uses this register number.");
        }

        student.User.FullName = request.FullName.Trim();
        student.RegisterNumber = registerNumber;
        student.Department = request.Department.Trim();
        student.Degree = request.Degree.Trim();
        student.GraduationYear = request.GraduationYear;
        student.CGPA = request.CGPA;
        student.ResumeUrl = request.ResumeUrl?.Trim();
        student.UpdatedAt = DateTime.UtcNow;

        _studentRepository.Update(student);
        await _studentRepository.SaveChangesAsync();

        return MapToResponse(student);
    }

    public async Task<StudentResponseDto> ApproveStudentAsync(int studentId)
    {
        var student = await GetStudentOrThrowAsync(studentId);

        if (student.Status != StudentStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending students can be approved.");
        }

        student.Status = StudentStatus.Approved;
        student.User.AccountStatus = AccountStatus.Approved;
        student.User.UpdatedAt = DateTime.UtcNow;

        _studentRepository.Update(student);
        await _studentRepository.SaveChangesAsync();

        return MapToResponse(student);
    }

    public async Task<StudentResponseDto> RejectStudentAsync(int studentId)
    {
        var student = await GetStudentOrThrowAsync(studentId);

        if (student.Status != StudentStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending students can be rejected.");
        }

        student.Status = StudentStatus.Rejected;
        student.User.AccountStatus = AccountStatus.Rejected;
        student.User.UpdatedAt = DateTime.UtcNow;

        _studentRepository.Update(student);
        await _studentRepository.SaveChangesAsync();

        return MapToResponse(student);
    }

    private async Task<Student> GetStudentOrThrowAsync(int studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);

        if (student == null)
        {
            throw new KeyNotFoundException(
                $"Student with ID {studentId} was not found.");
        }

        return student;
    }

    private static StudentResponseDto MapToResponse(Student student)
    {
        return new StudentResponseDto
        {
            StudentId = student.StudentId,
            UserId = student.UserId,
            FullName = student.User.FullName,
            Email = student.User.Email ?? string.Empty,
            CollegeId = student.CollegeId,
            CollegeName = student.College.CollegeName,
            RegisterNumber = student.RegisterNumber,
            Department = student.Department,
            Degree = student.Degree,
            GraduationYear = student.GraduationYear,
            CGPA = student.CGPA,
            ResumeUrl = student.ResumeUrl,
            Status = student.Status,
            CreatedAt = student.CreatedAt
        };
    }
}
