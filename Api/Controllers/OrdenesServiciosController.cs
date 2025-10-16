using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Api.Dtos.OrdenesServicios;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[Authorize(Roles = "Administrador,Mecanico")]
[ApiController]  
[Route("api/[controller]")]  
[EnableRateLimiting("ipLimiter")]
[SwaggerTag("Gestión de órdenes de servicio - Core del negocio del taller")]
public class OrdenesServiciosController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;
    private readonly IOrdenServicioRepository _repository;

    public OrdenesServiciosController(IMapper mapper, IUnitOfWork unitofwork, IOrdenServicioRepository repository)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
        _repository = repository;
    }

    [HttpGet("all")]
    [SwaggerOperation(
        Summary = "Obtener todas las órdenes de servicio",
        Description = "Retorna una lista completa de todas las órdenes de servicio en el sistema"
    )]
    [SwaggerResponse(200, "Lista de órdenes obtenida exitosamente", typeof(IEnumerable<OrdenServicioDto>))]
    public async Task<ActionResult<IEnumerable<OrdenServicioDto>>> GetAll(CancellationToken ct)
    {
        var ordeneservicios = await _unitofwork.OrdenesServicios.GetAllAsync(ct);
        var dto = _mapper.Map<IEnumerable<OrdenServicioDto>>(ordeneservicios);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Obtener orden de servicio por ID",
        Description = "Retorna la información detallada de una orden de servicio específica"
    )]
    [SwaggerResponse(200, "Orden de servicio encontrada", typeof(OrdenServicioDto))]
    [SwaggerResponse(404, "Orden de servicio no encontrada")]
    public async Task<ActionResult<OrdenServicioDto>> GetById(
        [SwaggerParameter("ID único de la orden de servicio (GUID)", Required = true)]
        Guid id, 
        CancellationToken ct)
    {
        var ordenservicio = await _unitofwork.OrdenesServicios.GetByIdAsync(id, ct);
        if (ordenservicio is null) return NotFound();
        return Ok(_mapper.Map<OrdenServicioDto>(ordenservicio));
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Crear nueva orden de servicio",
        Description = "Crea una nueva orden de servicio para un vehículo con tipo de servicio, fechas y estado"
    )]
    [SwaggerResponse(201, "Orden de servicio creada exitosamente", typeof(OrdenServicioDto))]
    [SwaggerResponse(400, "Datos de entrada inválidos")]
    [SwaggerResponse(500, "Error interno del servidor al crear la orden")]
    public async Task<IActionResult> Create(
        [SwaggerParameter("Datos para crear la orden de servicio", Required = true)]
        [FromBody] CreateOrdenServicioDto dto, 
        CancellationToken ct)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fechaIngresoUtc = dto.FechaIngreso.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(dto.FechaIngreso, DateTimeKind.Utc)
                : dto.FechaIngreso.ToUniversalTime();
                
            var fechaEstimadaUtc = dto.FechaEstimadaEntrega.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(dto.FechaEstimadaEntrega, DateTimeKind.Utc)
                : dto.FechaEstimadaEntrega.ToUniversalTime();

            var service_orders = new OrdenServicio(
                dto.TipoServicio,
                fechaIngresoUtc,
                fechaEstimadaUtc,
                dto.Estado,
                dto.UserMemberId, 
                dto.VehiculoId
            );

            await _unitofwork.OrdenesServicios.AddAsync(service_orders, ct);
            await _unitofwork.SaveChangesAsync(ct);

            var created = _mapper.Map<OrdenServicioDto>(service_orders);
            return CreatedAtAction(nameof(GetById), new { id = service_orders.Id }, created);
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Error de BD: {ex.InnerException?.Message}");
            return StatusCode(500, $"Error al crear la orden: {ex.InnerException?.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inesperado: {ex.Message}");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "Actualizar orden de servicio existente",
        Description = "Actualiza la información de una orden de servicio existente"
    )]
    [SwaggerResponse(204, "Orden de servicio actualizada exitosamente")]
    [SwaggerResponse(404, "Orden de servicio no encontrada")]
    [SwaggerResponse(500, "Error interno del servidor al actualizar")]
    public async Task<IActionResult> Update(
        [SwaggerParameter("ID de la orden a actualizar", Required = true)]
        Guid id, 
        [SwaggerParameter("Datos actualizados de la orden", Required = true)]
        [FromBody] UpdateOrdenServicioDto dto, 
        CancellationToken ct)
    {
        try
        {
            var existing = await _unitofwork.OrdenesServicios.GetByIdAsync(id, ct);
            if (existing is null) return NotFound();

            var fechaIngresoUtc = dto.FechaIngreso.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(dto.FechaIngreso, DateTimeKind.Utc)
                : dto.FechaIngreso.ToUniversalTime();
                
            var fechaEstimadaUtc = dto.FechaEstimadaEntrega.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(dto.FechaEstimadaEntrega, DateTimeKind.Utc)
                : dto.FechaEstimadaEntrega.ToUniversalTime();

            existing.Update(dto.TipoServicio, fechaIngresoUtc, fechaEstimadaUtc, dto.Estado);
            
            await _unitofwork.OrdenesServicios.UpdateAsync(existing, ct);
            await _unitofwork.SaveChangesAsync(ct);

            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Error de BD en update: {ex.InnerException?.Message}");
            return StatusCode(500, $"Error al actualizar: {ex.InnerException?.Message}");
        }
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Eliminar orden de servicio",
        Description = "Elimina permanentemente una orden de servicio del sistema"
    )]
    [SwaggerResponse(204, "Orden de servicio eliminada exitosamente")]
    [SwaggerResponse(404, "Orden de servicio no encontrada")]
    [SwaggerResponse(500, "Error interno del servidor al eliminar")]
    public async Task<IActionResult> Delete(
        [SwaggerParameter("ID de la orden a eliminar", Required = true)]
        Guid id, 
        CancellationToken ct)
    {
        try
        {
            var existing = await _unitofwork.OrdenesServicios.GetByIdAsync(id, ct);
            if (existing is null) return NotFound();

            await _unitofwork.OrdenesServicios.RemoveAsync(existing, ct);
            await _unitofwork.SaveChangesAsync(ct);

            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Error de BD en delete: {ex.InnerException?.Message}");
            return StatusCode(500, $"Error al eliminar: {ex.InnerException?.Message}");
        }
    }
}