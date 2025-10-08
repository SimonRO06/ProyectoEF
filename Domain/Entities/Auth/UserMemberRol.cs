using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities.Auth;
public class UserMemberRol : BaseEntity
{
        public int UsuarioId { get; set; }
        public Usuario usuarios { get; set; } = null!;
        public int RolId { get; set; }
        public Rol Rol { get; set; }  = null!;
}