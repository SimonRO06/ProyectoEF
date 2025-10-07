using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.Facturas;
using AutoMapper;
using Domain.Entities;

namespace Api.Mapping;
public class FacturaProfile : Profile
{
    public FacturaProfile()
    {
        CreateMap<Factura, FacturaDto>();

        CreateMap<CreateFacturaDto, Factura>()
            .ConstructUsing(src => new Factura(
                src.SubTotal,
                src.Impuestos,
                src.Total,
                src.OrdenServicioId
            ));
    }
}