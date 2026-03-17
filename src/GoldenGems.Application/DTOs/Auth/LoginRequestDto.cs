using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.Auth;

public class LoginRequestDto
{
    [Required]
    public string Identifier { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
