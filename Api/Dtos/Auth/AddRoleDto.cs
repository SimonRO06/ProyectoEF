using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Dtos.Auth;

public class AddRoleDto
{
    [Required]
    public string? Nombre { get; set; }
    [Required]
    public string? Contraseña { get; set; }
    [Required]
    public string? Role { get; set; }
}
