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

    public DetallesOrdenesController(IMapper mapper, IUnitOfWork unitofwork)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
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
    public async Task<IActionResult> Create([FromBody] CreateDetalleOrdenDto body, CancellationToken ct)
    {

        var product = _mapper.Map<DetalleOrden>(body);
        await _unitofwork.DetallesOrdenes.AddAsync(product, ct);

        var dto = _mapper.Map<DetalleOrdenDto>(product);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}
