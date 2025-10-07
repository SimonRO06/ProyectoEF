using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.Repuestos;
using AutoMapper;
using Domain.Entidades;

namespace Api.Mapping;
public class RepuestoProfile : Profile
{
    public RepuestoProfile()
    {
        CreateMap<Repuesto, RepuestoDto>();

        CreateMap<CreateRepuestoDto, Repuesto>()
            .ConstructUsing(src => new Repuesto(
                src.Codigo,
                src.Descripcion,
                src.CantidadStock,
                src.PrecioUnitario
            ));
    }
}