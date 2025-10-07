using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.Marcas;
using AutoMapper;
using Domain.Entidades;

namespace Api.Mapping;

public class MarcaProfile : Profile
{
    public MarcaProfile()
    {
        CreateMap<Marca, MarcaDto>();

        CreateMap<CreateMarcaDto, Marca>()
            .ConstructUsing(src => new Marca(
                src.Nombre
            ));
    }
}