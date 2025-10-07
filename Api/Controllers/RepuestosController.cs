using System;
using Api.Dtos.Repuestos;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[EnableRateLimiting("ipLimiter")]
public class RepuestosController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;

    public RepuestosController(IMapper mapper, IUnitOfWork unitofwork)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRepuestoDto body, CancellationToken ct)
    {

        var product = _mapper.Map<Repuesto>(body);
        await _unitofwork.Repuestos.AddAsync(product, ct);

        var dto = _mapper.Map<RepuestoDto>(product);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}
