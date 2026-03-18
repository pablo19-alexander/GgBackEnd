using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.Payment;

public class CommissionRequestDto
{
    [Required]
    public Guid CompanyId { get; set; }

    [Required]
    [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
    public decimal Percentage { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MinAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MaxAmount { get; set; }
}
