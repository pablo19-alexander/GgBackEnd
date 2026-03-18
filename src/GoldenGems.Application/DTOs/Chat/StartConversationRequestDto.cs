using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.Chat;

public class StartConversationRequestDto
{
    [Required(ErrorMessage = "El ID del producto es requerido")]
    public Guid ProductId { get; set; }
}
