
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementSystem.DTOs.Company;

public class CompanyUpdateRequestDto
{
    [Required]
    [MaxLength(150)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Industry { get; set; } = string.Empty;

    [Url]
    public string? Website { get; set; }

    public string? Description { get; set; }

    [Required]
    [MaxLength(250)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string ContactEmail { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string ContactPhone { get; set; } = string.Empty;
}
