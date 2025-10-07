using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dtos.Clientes;
public record CreateClienteDto(string Nombre,string Telefono ,string Correo);