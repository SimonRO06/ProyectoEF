using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Api.Dtos.OrdenesServicios;

namespace Api.Controllers;

[EnableRateLimiting("ipLimiter")]
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
    public async Task<ActionResult<IEnumerable<OrdenServicioDto>>> GetAll(CancellationToken ct)
    {
        var ordeneservicios = await _unitofwork.OrdenesServicios.GetAllAsync(ct);
        var dto = _mapper.Map<IEnumerable<OrdenServicioDto>>(ordeneservicios);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [DisableRateLimiting]
    public async Task<ActionResult<OrdenServicioDto>> GetById(Guid id, CancellationToken ct)
    {
        var ordenservicio = await _unitofwork.OrdenesServicios.GetByIdAsync(id, ct);
        if (ordenservicio is null) return NotFound();
        return Ok(_mapper.Map<OrdenServicioDto>(ordenservicio));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrdenServicioDto dto, CancellationToken ct)
    {
        try
        {
            // Validación básica
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ✅ Conversión de fechas a UTC (para evitar el error anterior)
            var fechaIngresoUtc = dto.FechaIngreso.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(dto.FechaIngreso, DateTimeKind.Utc)
                : dto.FechaIngreso.ToUniversalTime();
                
            var fechaEstimadaUtc = dto.FechaEstimadaEntrega.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(dto.FechaEstimadaEntrega, DateTimeKind.Utc)
                : dto.FechaEstimadaEntrega.ToUniversalTime();

            var service_orders = new OrdenServicio(
                dto.TipoServicio, // ✅ Ahora acepta tanto números como strings automáticamente
                fechaIngresoUtc,  // ✅ Usando fechas UTC
                fechaEstimadaUtc, // ✅ Usando fechas UTC
                dto.Estado,       // ✅ Ahora acepta tanto números como strings automáticamente
                dto.UserMemberId, 
                dto.VehiculoId
            );

            // Usando UnitOfWork (Opción recomendada)
            await _unitofwork.OrdenesServicios.AddAsync(service_orders, ct);
            await _unitofwork.SaveChangesAsync(ct);

            var created = _mapper.Map<OrdenServicioDto>(service_orders);
            return CreatedAtAction(nameof(GetById), new { id = service_orders.Id }, created);
        }
        catch (DbUpdateException ex)
        {
            // Loggear el error real
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
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrdenServicioDto dto, CancellationToken ct)
    {
        try
        {
            var existing = await _unitofwork.OrdenesServicios.GetByIdAsync(id, ct);
            if (existing is null) return NotFound();

            // ✅ Conversión de fechas a UTC también en update
            var fechaIngresoUtc = dto.FechaIngreso.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(dto.FechaIngreso, DateTimeKind.Utc)
                : dto.FechaIngreso.ToUniversalTime();
                
            var fechaEstimadaUtc = dto.FechaEstimadaEntrega.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(dto.FechaEstimadaEntrega, DateTimeKind.Utc)
                : dto.FechaEstimadaEntrega.ToUniversalTime();

            existing.Update(
                dto.TipoServicio, 
                fechaIngresoUtc,   // ✅ Usando fechas UTC
                fechaEstimadaUtc,  // ✅ Usando fechas UTC
                dto.Estado
            );
            
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
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
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