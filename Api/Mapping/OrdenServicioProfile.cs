using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.OrdenesServicios;
using AutoMapper;
using Domain.Entities;

namespace Api.Mapping;
public class OrdenServicioProfile : Profile
{
    public OrdenServicioProfile()
    {
        CreateMap<OrdenServicio, OrdenServicioDto>();

        CreateMap<CreateOrdenServicioDto, OrdenServicio>()
            .ConstructUsing(src => new OrdenServicio(
                src.TipoServicio,
                src.FechaEstimadaEntrega,
                src.Estado,
                src.UsuarioId,
                src.VehiculoId
            ));
    }
}