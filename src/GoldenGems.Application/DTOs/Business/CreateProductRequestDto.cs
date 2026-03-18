using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.Business;

public class CreateProductRequestDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
    public decimal ReferencePrice { get; set; }

    public bool IsNegotiable { get; set; }

    [StringLength(500)]
    public string WhatsAppMessage { get; set; } = string.Empty;

    [Required(ErrorMessage = "La empresa es requerida")]
    public Guid CompanyId { get; set; }

    [Required(ErrorMessage = "El tipo de producto es requerido")]
    public Guid ProductTypeId { get; set; }
}
