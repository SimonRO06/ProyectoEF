using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities.Auth;
public class Usuario : BaseEntity
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Correo { get; set; }
    public string? Contraseña { get; set; }
    public ICollection<Rol> Rols { get; set; } = new HashSet<Rol>();
    public ICollection<UserMemberRol> UserMemberRols { get; set; } = new HashSet<UserMemberRol>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();

    public virtual ICollection<OrdenServicio> OrdenesServicios { get; set; } = new HashSet<OrdenServicio>();
}