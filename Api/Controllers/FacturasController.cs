using System;
using Api.Dtos.Facturas;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[EnableRateLimiting("ipLimiter")]
public class FacturasController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;

    public FacturasController(IMapper mapper, IUnitOfWork unitofwork)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
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
    public async Task<IActionResult> Create([FromBody] CreateFacturaDto body, CancellationToken ct)
    {

        var factura = _mapper.Map<Factura>(body);
        await _unitofwork.Facturas.AddAsync(factura, ct);

        var dto = _mapper.Map<FacturaDto>(factura);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}
