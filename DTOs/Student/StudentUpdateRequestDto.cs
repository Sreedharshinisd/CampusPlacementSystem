
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementSystem.DTOs.Student;

public class StudentUpdateRequestDto
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

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

    public string? ResumeUrl { get; set; }
}
