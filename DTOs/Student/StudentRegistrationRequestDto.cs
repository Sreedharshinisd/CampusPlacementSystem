
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementSystem.DTOs.Student;

public class StudentRegistrationRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public int CollegeId { get; set; }

    [Required]
    [MaxLength(30)]
    public string RegisterNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Degree { get; set; } = string.Empty;

    [Range(2020, 2100)]
    public int GraduationYear { get; set; }

    [Range(typeof(decimal), "0", "10")]
    public decimal CGPA { get; set; }
}
