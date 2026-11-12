
using CampusPlacementSystem.Enums;

namespace CampusPlacementSystem.Models;

public class Company
{
    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string CompanyCode { get; set; } = string.Empty;

    public string Industry { get; set; } = string.Empty;

    public string? Website { get; set; }

    public string? Description { get; set; }

    public string Address { get; set; } = string.Empty;

    public string ContactEmail { get; set; } = string.Empty;

    public string ContactPhone { get; set; } = string.Empty;

    public CompanyStatus Status { get; set; } = CompanyStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
