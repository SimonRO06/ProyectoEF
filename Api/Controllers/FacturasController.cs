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
        var invoices = new Factura(dto.FechaEmision, dto.Total, dto.OrdenServicioId);
        await _repository.AddAsync(invoices, ct);
        await _unitofwork.SaveChangesAsync(ct);

        var created = new FacturaDto(invoices.Id, invoices.FechaEmision, invoices.Total!, invoices.OrdenServicioId!);
        return CreatedAtAction(nameof(GetById), new { id = invoices.Id }, created);
    }
}
