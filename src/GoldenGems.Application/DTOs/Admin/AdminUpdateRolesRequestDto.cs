using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.Admin;

public class AdminUpdateRolesRequestDto
{
    [Required(ErrorMessage = "Debe asignar al menos un rol")]
    [MinLength(1, ErrorMessage = "Debe asignar al menos un rol")]
    public List<Guid> RoleIds { get; set; } = new();
}
