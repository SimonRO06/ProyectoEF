using System;
using Api.Dtos.Vehiculos;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[EnableRateLimiting("ipLimiter")]
[SwaggerTag("Gestión de vehículos de clientes del taller")]
public class VehiculosController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;
    private readonly IVehiculoRepository _repository;

    public VehiculosController(IMapper mapper, IUnitOfWork unitofwork, IVehiculoRepository repository)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
        _repository = repository;
    }

    [HttpGet("all")]
    [SwaggerOperation(
        Summary = "Obtener todos los vehículos",
        Description = "Retorna una lista completa de todos los vehículos registrados en el sistema"
    )]
    [SwaggerResponse(200, "Lista de vehículos obtenida exitosamente", typeof(IEnumerable<VehiculoDto>))]
    public async Task<ActionResult<IEnumerable<VehiculoDto>>> GetAll(CancellationToken ct)
    {
        var vehiculos = await _unitofwork.Vehiculos.GetAllAsync(ct);
        var dto = _mapper.Map<IEnumerable<VehiculoDto>>(vehiculos);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Obtener vehículo por ID",
        Description = "Retorna la información detallada de un vehículo específico"
    )]
    [SwaggerResponse(200, "Vehículo encontrado", typeof(VehiculoDto))]
    [SwaggerResponse(404, "Vehículo no encontrado")]
    public async Task<ActionResult<VehiculoDto>> GetById(
        [SwaggerParameter("ID único del vehículo (GUID)", Required = true)]
        Guid id, 
        CancellationToken ct)
    {
        var product = await _unitofwork.Vehiculos.GetByIdAsync(id, ct);
        if (product is null) return NotFound();

        return Ok(_mapper.Map<VehiculoDto>(product));
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Registrar nuevo vehículo",
        Description = "Registra un nuevo vehículo asociado a un cliente en el sistema"
    )]
    [SwaggerResponse(201, "Vehículo registrado exitosamente", typeof(VehiculoDto))]
    [SwaggerResponse(400, "Datos de entrada inválidos")]
    public async Task<IActionResult> Create(
        [SwaggerParameter("Datos del vehículo a registrar", Required = true)]
        [FromBody] CreateVehiculoDto dto, 
        CancellationToken ct)
    {
        var vehicles = new Vehiculo(dto.Año, dto.NumeroSerie, dto.Kilometraje, dto.ClienteId, dto.ModeloId);
        await _repository.AddAsync(vehicles, ct);
        await _unitofwork.SaveChangesAsync(ct);

        var created = new VehiculoDto(vehicles.Id, vehicles.Año, vehicles.NumeroSerie, vehicles.Kilometraje, vehicles.ClienteId, vehicles.ModeloId);
        return CreatedAtAction(nameof(GetById), new { id = vehicles.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "Actualizar vehículo existente",
        Description = "Actualiza la información de un vehículo existente en el sistema"
    )]
    [SwaggerResponse(204, "Vehículo actualizado exitosamente")]
    [SwaggerResponse(404, "Vehículo no encontrado")]
    public async Task<IActionResult> Update(
        [SwaggerParameter("ID del vehículo a actualizar", Required = true)]
        Guid id, 
        [SwaggerParameter("Datos actualizados del vehículo", Required = true)]
        [FromBody] UpdateVehiculoDto dto, 
        CancellationToken ct)
    {
        var existing = await _unitofwork.Vehiculos.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        existing.Update(dto.Año, dto.NumeroSerie, dto.Kilometraje);

        await _unitofwork.Vehiculos.UpdateAsync(existing, ct);
        await _unitofwork.SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Eliminar vehículo",
        Description = "Elimina permanentemente un vehículo del sistema"
    )]
    [SwaggerResponse(204, "Vehículo eliminado exitosamente")]
    [SwaggerResponse(404, "Vehículo no encontrado")]
    public async Task<IActionResult> Delete(
        [SwaggerParameter("ID del vehículo a eliminar", Required = true)]
        Guid id, 
        CancellationToken ct)
    {
        var existing = await _unitofwork.Vehiculos.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        await _unitofwork.Vehiculos.RemoveAsync(existing, ct);
        await _unitofwork.SaveChangesAsync(ct);

        return NoContent();
    }
}