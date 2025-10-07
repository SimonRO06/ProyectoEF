using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.Clientes;
using AutoMapper;
using Domain.Entities;

namespace Api.Mapping;
public class ClienteProfile : Profile
{
    public ClienteProfile()
    {
        CreateMap<Cliente, ClienteDto>();

        CreateMap<CreateClienteDto, Cliente>()
            .ConstructUsing(src => new Cliente(
                src.Nombre,
                src.Correo,
                src.Telefono
            ));
    }
}