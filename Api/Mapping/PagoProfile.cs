using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.Pagos;
using AutoMapper;
using Domain.Entities;

namespace Api.Mapping;
public class PagoProfile : Profile
{
    public PagoProfile()
    {
        CreateMap<Pago, PagoDto>();

        CreateMap<CreatePagoDto, Pago>()
            .ConstructUsing(src => new Pago(
                src.Monto,
                src.MetodoPago,
                src.FacturaId
            ));
    }
}