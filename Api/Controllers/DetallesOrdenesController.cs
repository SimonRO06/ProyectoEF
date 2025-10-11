using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.DetallesOrdenes;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[EnableRateLimiting("ipLimiter")]
public class DetallesOrdenesController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;
    private readonly IDetalleOrdenRepository _repository;


    public DetallesOrdenesController(IMapper mapper, IUnitOfWork unitofwork, IDetalleOrdenRepository repository)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
        _repository = repository;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<DetalleOrdenDto>>> GetAll(CancellationToken ct)
    {
        var detallesordenes = await _unitofwork.DetallesOrdenes.GetAllAsync(ct); 
        var dto = _mapper.Map<IEnumerable<DetalleOrdenDto>>(detallesordenes);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [DisableRateLimiting]
    public async Task<ActionResult<DetalleOrdenDto>> GetById(Guid id, CancellationToken ct)
    {
        var product = await _unitofwork.DetallesOrdenes.GetByIdAsync(id, ct);
        if (product is null) return NotFound();

        return Ok(_mapper.Map<DetalleOrdenDto>(product));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDetalleOrdenDto dto, CancellationToken ct)
    {
        var order_details = new DetalleOrden(dto.Cantidad, dto.CostoUnitario, dto.OrdenServicioId, dto.RepuestoId);
        await _repository.AddAsync(order_details, ct);
        await _unitofwork.SaveChangesAsync(ct);

        var created = new DetalleOrdenDto(order_details.Id, order_details.Cantidad, order_details.CostoUnitario!, order_details.OrdenServicioId!, order_details.RepuestoId!);
        return CreatedAtAction(nameof(GetById), new { id = order_details.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDetalleOrdenDto dto, CancellationToken ct)
    {
        var existing = await _unitofwork.DetallesOrdenes.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        // Actualizamos los campos
        existing.Update(dto.Cantidad, dto.CostoUnitario, dto.OrdenServicioId, dto.RepuestoId);

        await _unitofwork.DetallesOrdenes.UpdateAsync(existing, ct);
        await _unitofwork.SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var existing = await _unitofwork.DetallesOrdenes.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        await _unitofwork.DetallesOrdenes.RemoveAsync(existing, ct);
        await _unitofwork.SaveChangesAsync(ct);

        return NoContent();
    }
}
