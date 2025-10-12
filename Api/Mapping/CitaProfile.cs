using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.Citas;
using AutoMapper;
using Domain.Entities;

namespace Api.Mapping;
public class CitaProfile : Profile
{
    public CitaProfile()
    {
        CreateMap<Cita, CitaDto>();

        CreateMap<CreateCitaDto, Cita>()
            .ConstructUsing(src => new Cita(
                src.Fecha,
                src.Hora,
                src.Observaciones,
                src.VehiculoId,
                src.ClienteId
            ));
    }
}