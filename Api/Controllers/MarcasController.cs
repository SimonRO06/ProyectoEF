using System;
using Api.Dtos.Marcas;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[EnableRateLimiting("ipLimiter")]
public class MarcasController : BaseApiController
{
     private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;

    public MarcasController(IMapper mapper, IUnitOfWork unitofwork)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<MarcaDto>>> GetAll(CancellationToken ct)
    {
        var marcas = await _unitofwork.Marcas.GetAllAsync(ct);
        var dto = _mapper.Map<IEnumerable<MarcaDto>>(marcas);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [DisableRateLimiting]
    public async Task<ActionResult<MarcaDto>> GetById(Guid id, CancellationToken ct)
    {
        var marca = await _unitofwork.Marcas.GetByIdAsync(id, ct);
        if (marca is null) return NotFound();

        return Ok(_mapper.Map<MarcaDto>(marca));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMarcaDto body, CancellationToken ct)
    {

        var marca = _mapper.Map<Marca>(body);
        await _unitofwork.Marcas.AddAsync(marca, ct);

        var dto = _mapper.Map<MarcaDto>(marca);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}
