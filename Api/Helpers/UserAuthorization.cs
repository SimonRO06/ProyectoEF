using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Helpers;
public class UserAuthorization
{
    public enum Roles
    {
        Administrador,
        Mecanico,
        Recepcionista
    }

    public const Roles rol_default = Roles.Mecanico;
}