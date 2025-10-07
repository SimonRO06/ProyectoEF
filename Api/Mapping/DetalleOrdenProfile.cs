using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.DetallesOrdenes;
using AutoMapper;
using Domain.Entities;

namespace Api.Mapping;
public class DetalleOrdenProfile : Profile
{
    public DetalleOrdenProfile()
    {
        CreateMap<DetalleOrden, DetalleOrdenDto>();

        CreateMap<CreateDetalleOrdenDto, DetalleOrden>()
            .ConstructUsing(src => new DetalleOrden(
                src.Cantidad,
                src.CostoUnitario,
                src.OrdenServicioId,
                src.RespuestaId
            ));
    }
}