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

    public PagosController(IMapper mapper, IUnitOfWork unitofwork)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
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
    public async Task<IActionResult> Create([FromBody] CreatePagoDto body, CancellationToken ct)
    {
        var pago = _mapper.Map<Pago>(body);
        await _unitofwork.Pagos.AddAsync(pago, ct);

        var dto = _mapper.Map<PagoDto>(pago);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}
