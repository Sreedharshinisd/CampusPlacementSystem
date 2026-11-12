
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementSystem.DTOs.College;

public class CollegeRegistrationRequestDto
{
    [Required]
    [MaxLength(150)]
    public string CollegeName { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string UniversityName { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string CollegeCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(250)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string OfficialEmail { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    public string? Website { get; set; }
}
