using System;
using Api.Dtos.Facturas;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[Authorize(Roles = "Administrador,Mecanico")]
[ApiController]  
[Route("api/[controller]")]  
[EnableRateLimiting("ipLimiter")]
public class FacturasController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;
    private readonly IFacturaRepository _repository;


    public FacturasController(IMapper mapper, IUnitOfWork unitofwork, IFacturaRepository repository)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
        _repository = repository;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<FacturaDto>>> GetAll(CancellationToken ct)
    {
        var facturas = await _unitofwork.Facturas.GetAllAsync(ct);
        var dto = _mapper.Map<IEnumerable<FacturaDto>>(facturas);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [DisableRateLimiting]
    public async Task<ActionResult<FacturaDto>> GetById(Guid id, CancellationToken ct)
    {
        var factura = await _unitofwork.Facturas.GetByIdAsync(id, ct);
        if (factura is null) return NotFound();

        return Ok(_mapper.Map<FacturaDto>(factura));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFacturaDto dto, CancellationToken ct)
    {
        try
        {
            var invoices = new Factura(dto.FechaEmision, dto.Total, dto.OrdenServicioId);
            await _repository.AddAsync(invoices, ct);
            await _unitofwork.SaveChangesAsync(ct);

            var created = new FacturaDto(invoices.Id, invoices.FechaEmision, invoices.Total!, invoices.OrdenServicioId!);
            return CreatedAtAction(nameof(GetById), new { id = invoices.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFacturaDto dto, CancellationToken ct)
    {
        var existing = await _unitofwork.Facturas.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        // Actualizamos los campos
        existing.Update(dto.Total);

        await _unitofwork.Facturas.UpdateAsync(existing, ct);
        await _unitofwork.SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var existing = await _unitofwork.Facturas.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        await _unitofwork.Facturas.RemoveAsync(existing, ct);
        await _unitofwork.SaveChangesAsync(ct);

        return NoContent();
    }
}
