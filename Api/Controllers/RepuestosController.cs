using System;
using Api.Dtos.Repuestos;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("ipLimiter")]
public class RepuestosController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;
    private readonly IRepuestoRepository _repository;


    public RepuestosController(IMapper mapper, IUnitOfWork unitofwork, IRepuestoRepository repository)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
        _repository = repository;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<RepuestoDto>>> GetAll(CancellationToken ct)
    {
        var repuestos = await _unitofwork.Repuestos.GetAllAsync(ct); // necesitas este método en IProductRepository
        var dto = _mapper.Map<IEnumerable<RepuestoDto>>(repuestos);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [DisableRateLimiting]
    public async Task<ActionResult<RepuestoDto>> GetById(Guid id, CancellationToken ct)
    {
        var repuesto = await _unitofwork.Repuestos.GetByIdAsync(id, ct);
        if (repuesto is null) return NotFound();

        return Ok(_mapper.Map<RepuestoDto>(repuesto));
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRepuestoDto dto, CancellationToken ct)
    {
        var spare_parts = new Repuesto(dto.Codigo, dto.Descripcion, dto.CantidadStock, dto.PrecioUnitario);
        await _repository.AddAsync(spare_parts, ct);
        await _unitofwork.SaveChangesAsync(ct);

        var created = new RepuestoDto(spare_parts.Id, spare_parts.Codigo, spare_parts.Descripcion, spare_parts.CantidadStock, spare_parts.PrecioUnitario);
        return CreatedAtAction(nameof(GetById), new { id = spare_parts.Id }, created);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRepuestoDto dto, CancellationToken ct)
    {
        var existing = await _unitofwork.Repuestos.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        // Actualizamos los campos
        existing.Update(dto.Descripcion, dto.CantidadStock, dto.PrecioUnitario);

        await _unitofwork.Repuestos.UpdateAsync(existing, ct);
        await _unitofwork.SaveChangesAsync(ct);

        return NoContent();
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var existing = await _unitofwork.Repuestos.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        await _unitofwork.Repuestos.RemoveAsync(existing, ct);
        await _unitofwork.SaveChangesAsync(ct);

        return NoContent();
    }
}
