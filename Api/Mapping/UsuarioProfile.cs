using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.Usuarios;
using AutoMapper;
using Domain.Entities;

namespace Api.Mapping;
public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<Usuario, UsuarioDto>();

        CreateMap<CreateUsuarioDto, Usuario>()
            .ConstructUsing(src => new Usuario(
                src.Nombre,
                src.Correo,
                src.Contraseña,
                src.Rol
            ));
    }
}