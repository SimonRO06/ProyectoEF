using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Api.Converts;
using Domain.Enums;

namespace Api.Dtos.OrdenesServicios;
public record CreateOrdenServicioDto(
    [property: JsonConverter(typeof(FlexibleEnumConverter<TipoServicio>))]
    TipoServicio TipoServicio,
    
    DateTime FechaIngreso, 
    DateTime FechaEstimadaEntrega, 
    
    [property: JsonConverter(typeof(FlexibleEnumConverter<Estado>))]
    Estado Estado, 
    
    int UserMemberId, 
    Guid VehiculoId
);