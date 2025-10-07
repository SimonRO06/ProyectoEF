using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Api.Dtos.Usuarios;
public record UpdateUsuarioDto( string Nombre, string Correo, Rol Rol);