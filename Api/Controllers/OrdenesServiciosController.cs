using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.OrdenesServicios;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[EnableRateLimiting("ipLimiter")]
public class OrdenesServiciosController : BaseApiController
{
        private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;

    public OrdenesServiciosController(IMapper mapper, IUnitOfWork unitofwork)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<OrdenServicioDto>>> GetAll(CancellationToken ct)
    {
        var ordeneservicios = await _unitofwork.OrdenesServicios.GetAllAsync(ct); // necesitas este método en IProductRepository
        var dto = _mapper.Map<IEnumerable<OrdenServicioDto>>(ordeneservicios);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [DisableRateLimiting]
    public async Task<ActionResult<OrdenServicioDto>> GetById(Guid id, CancellationToken ct)
    {
        var ordenservicio = await _unitofwork.OrdenesServicios.GetByIdAsync(id, ct);
        if (ordenservicio is null) return NotFound();

        return Ok(_mapper.Map<OrdenServicioDto>(ordenservicio));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrdenServicioDto body, CancellationToken ct)
    {
        var ordenservicio = _mapper.Map<OrdenServicio>(body);
        await _unitofwork.OrdenesServicios.AddAsync(ordenservicio, ct);

        var dto = _mapper.Map<OrdenServicioDto>(ordenservicio);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}
