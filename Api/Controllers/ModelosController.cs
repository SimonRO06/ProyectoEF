using System;
using Api.Dtos.Modelos;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[EnableRateLimiting("ipLimiter")]
public class ModelosController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;

    public ModelosController(IMapper mapper, IUnitOfWork unitofwork)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<ModeloDto>>> GetAll(CancellationToken ct)
    {
        var modelos = await _unitofwork.Modelos.GetAllAsync(ct);
        var dto = _mapper.Map<IEnumerable<ModeloDto>>(modelos);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [DisableRateLimiting]
    public async Task<ActionResult<ModeloDto>> GetById(Guid id, CancellationToken ct)
    {
        var modelo = await _unitofwork.Modelos.GetByIdAsync(id, ct);
        if (modelo is null) return NotFound();

        return Ok(_mapper.Map<ModeloDto>(modelo));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateModeloDto body, CancellationToken ct)
    {
        var product = _mapper.Map<Modelo>(body);
        await _unitofwork.Modelos.AddAsync(product, ct);

        var dto = _mapper.Map<ModeloDto>(product);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}
