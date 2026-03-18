using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.Chat;

public class SendMessageRequestDto
{
    [Required(ErrorMessage = "El contenido es requerido")]
    [StringLength(2000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
}

public class OfferPriceRequestDto
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Price { get; set; }
}
