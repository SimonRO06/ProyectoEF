using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.Modelos;
using AutoMapper;
using Domain.Entities;

namespace Api.Mapping;
public class ModeloProfile : Profile
{
    public ModeloProfile()
    {
        CreateMap<Modelo, ModeloDto>();

        CreateMap<CreateModeloDto, Modelo>()
            .ConstructUsing(src => new Modelo(
                src.Nombre,
                src.MarcaId
            ));
    }
}