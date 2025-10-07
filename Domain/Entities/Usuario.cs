using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities;
public class Usuario
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string? Nombre { get; private set; }
    public string? Correo { get; private set; }
    public string? ContraseñaHasheada { get; private set; }
    public Rol Rol { get; private set; }

    public virtual ICollection<OrdenServicio> OrdenesServicios { get; set; } = new HashSet<OrdenServicio>();
    private Usuario() { }
    public Usuario(string nombre, string correo, string contraseña_hasheada, Rol rol)
    { Nombre = nombre; Correo = correo; ContraseñaHasheada = contraseña_hasheada; Rol = rol; }
}