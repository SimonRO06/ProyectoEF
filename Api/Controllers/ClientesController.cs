using System;
using Api.Dtos.Clientes;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Authorize(Policy = "RecepcionistaOnly")]
[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("ipLimiter")]
[SwaggerTag("Gestión completa de clientes del taller")]
public class ClientesController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;
    private readonly IClienteRepository _repository;

    public ClientesController(IMapper mapper, IUnitOfWork unitofwork, IClienteRepository repository)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
        _repository = repository;
    }

    [HttpGet("all")]
    [SwaggerOperation(
        Summary = "Obtener todos los clientes",
        Description = "Retorna una lista completa de todos los clientes registrados en el sistema"
    )]
    [SwaggerResponse(200, "Lista de clientes obtenida exitosamente", typeof(IEnumerable<ClienteDto>))]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll(CancellationToken ct)
    {
        var clientes = await _unitofwork.Clientes.GetAllAsync(ct);
        var dto = _mapper.Map<IEnumerable<ClienteDto>>(clientes);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Obtener cliente por ID",
        Description = "Retorna la información detallada de un cliente específico"
    )]
    [SwaggerResponse(200, "Cliente encontrado", typeof(ClienteDto))]
    [SwaggerResponse(404, "Cliente no encontrado")]
    public async Task<ActionResult<ClienteDto>> GetById(
        [SwaggerParameter("ID único del cliente (GUID)", Required = true)]
        Guid id, 
        CancellationToken ct)
    {
        var cliente = await _unitofwork.Clientes.GetByIdAsync(id, ct);
        if (cliente is null) return NotFound();

        return Ok(_mapper.Map<ClienteDto>(cliente));
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Crear nuevo cliente",
        Description = "Registra un nuevo cliente en el sistema con información básica"
    )]
    [SwaggerResponse(201, "Cliente creado exitosamente", typeof(ClienteDto))]
    [SwaggerResponse(400, "Datos de entrada inválidos")]
    public async Task<IActionResult> Create(
        [SwaggerParameter("Datos del cliente a crear", Required = true)]
        [FromBody] CreateClienteDto dto, 
        CancellationToken ct = default)
    {
        var customer = new Cliente(dto.Nombre, dto.Correo, dto.Telefono);
        await _repository.AddAsync(customer, ct);

        var created = new ClienteDto(customer.Id, customer.Nombre!, customer.Correo!, customer.Telefono ?? "");
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "Actualizar cliente existente",
        Description = "Actualiza la información de un cliente existente en el sistema"
    )]
    [SwaggerResponse(204, "Cliente actualizado exitosamente")]
    [SwaggerResponse(404, "Cliente no encontrado")]
    public async Task<IActionResult> Update(
        [SwaggerParameter("ID del cliente a actualizar", Required = true)]
        Guid id, 
        [SwaggerParameter("Datos actualizados del cliente", Required = true)]
        [FromBody] UpdateClienteDto dto, 
        CancellationToken ct)
    {
        var existing = await _unitofwork.Clientes.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        existing.Update(dto.Nombre, dto.Telefono, dto.Correo);

        await _unitofwork.Clientes.UpdateAsync(existing, ct);
        await _unitofwork.SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Eliminar cliente",
        Description = "Elimina permanentemente un cliente del sistema"
    )]
    [SwaggerResponse(204, "Cliente eliminado exitosamente")]
    [SwaggerResponse(404, "Cliente no encontrado")]
    public async Task<IActionResult> Delete(
        [SwaggerParameter("ID del cliente a eliminar", Required = true)]
        Guid id, 
        CancellationToken ct)
    {
        var existing = await _unitofwork.Clientes.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        await _unitofwork.Clientes.RemoveAsync(existing, ct);
        await _unitofwork.SaveChangesAsync(ct);

        return NoContent();
    }
}