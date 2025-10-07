using System;
using Api.Dtos.Vehiculos;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[EnableRateLimiting("ipLimiter")]
public class VehiculosController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;

    public VehiculosController(IMapper mapper, IUnitOfWork unitofwork)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<VehiculoDto>>> GetAll(CancellationToken ct)
    {
        var vehiculos = await _unitofwork.Vehiculos.GetAllAsync(ct);
        var dto = _mapper.Map<IEnumerable<VehiculoDto>>(vehiculos);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [DisableRateLimiting]
    public async Task<ActionResult<VehiculoDto>> GetById(Guid id, CancellationToken ct)
    {
        var product = await _unitofwork.Vehiculos.GetByIdAsync(id, ct);
        if (product is null) return NotFound();

        return Ok(_mapper.Map<VehiculoDto>(product));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVehiculoDto body, CancellationToken ct)
    {

        var vehiculo = _mapper.Map<Vehiculo>(body);
        await _unitofwork.Vehiculos.AddAsync(vehiculo, ct);

        var dto = _mapper.Map<VehiculoDto>(vehiculo);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}
