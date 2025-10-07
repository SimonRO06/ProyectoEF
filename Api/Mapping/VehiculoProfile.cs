using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.Vehiculos;
using AutoMapper;
using Domain.Entidades;

namespace Api.Mapping;
public class VehiculoProfile : Profile
{
    public VehiculoProfile()
    {
        CreateMap<Vehiculo, VehiculoDto>();

        CreateMap<CreateVehiculoDto, Vehiculo>()
            .ConstructUsing(src => new Vehiculo(
                src.Modelo,
                src.Año,
                src.NumeroSerie,
                src.Kilometraje,
                src.ClienteId,
                src.MarcaId
            ));
    }
}