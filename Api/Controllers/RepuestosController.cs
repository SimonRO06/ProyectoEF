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
    private readonly IRepuestoRepository _repository;


    public RepuestosController(IMapper mapper, IUnitOfWork unitofwork, IRepuestoRepository repository)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
        _repository = repository;
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
    public async Task<IActionResult> Create([FromBody] CreateRepuestoDto dto, CancellationToken ct)
    {
        var spare_parts = new Repuesto(dto.Codigo, dto.Descripcion, dto.CantidadStock, dto.PrecioUnitario);
        await _repository.AddAsync(spare_parts, ct);

        var created = new RepuestoDto(spare_parts.Id, spare_parts.Codigo, spare_parts.Descripcion, spare_parts.CantidadStock, spare_parts.PrecioUnitario);
        return CreatedAtAction(nameof(GetById), new { id = spare_parts.Id }, created);
    }
}
