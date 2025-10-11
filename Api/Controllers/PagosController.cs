using System;
using Api.Dtos.Pagos;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[EnableRateLimiting("ipLimiter")]
public class PagosController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;
    private readonly IPagoRepository _repository;


    public PagosController(IMapper mapper, IUnitOfWork unitofwork, IPagoRepository repository)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
        _repository = repository;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<PagoDto>>> GetAll(CancellationToken ct)
    {
        var pagos = await _unitofwork.Pagos.GetAllAsync(ct);
        var dto = _mapper.Map<IEnumerable<PagoDto>>(pagos);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [DisableRateLimiting]
    public async Task<ActionResult<PagoDto>> GetById(Guid id, CancellationToken ct)
    {
        var pago = await _unitofwork.Pagos.GetByIdAsync(id, ct);
        if (pago is null) return NotFound();

        return Ok(_mapper.Map<PagoDto>(pago));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePagoDto dto, CancellationToken ct)
    {
        var payments = new Pago(dto.FechaPago, dto.Monto, dto.MetodoPago, dto.FacturaId);
        await _repository.AddAsync(payments, ct);
        await _unitofwork.SaveChangesAsync(ct);

        var created = new PagoDto(payments.Id, payments.Monto, payments.FechaPago, payments.MetodoPago, payments.FacturaId);
        return CreatedAtAction(nameof(GetById), new { id = payments.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePagoDto dto, CancellationToken ct)
    {
        var existing = await _unitofwork.Pagos.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        // Actualizamos los campos
        existing.Update(dto.Monto, dto.MetodoPago);

        await _unitofwork.Pagos.UpdateAsync(existing, ct);
        await _unitofwork.SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var existing = await _unitofwork.Pagos.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        await _unitofwork.Pagos.RemoveAsync(existing, ct);
        await _unitofwork.SaveChangesAsync(ct);

        return NoContent();
    }
}
