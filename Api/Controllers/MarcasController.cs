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
    private readonly IMarcaRepository _repository;


    public MarcasController(IMapper mapper, IUnitOfWork unitofwork, IMarcaRepository repository)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
        _repository = repository;
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
    public async Task<IActionResult> Create([FromBody] CreateMarcaDto dto, CancellationToken ct)
    {
        var brands = new Marca(dto.Nombre);
        await _repository.AddAsync(brands, ct);

        var created = new MarcaDto(brands.Id, brands.Nombre);
        return CreatedAtAction(nameof(GetById), new { id = brands.Id }, created);
    }
}
