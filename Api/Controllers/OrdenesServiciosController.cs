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
    private readonly IOrdenServicioRepository _repository;


    public OrdenesServiciosController(IMapper mapper, IUnitOfWork unitofwork, IOrdenServicioRepository repository)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
        _repository = repository;
    }
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<OrdenServicioDto>>> GetAll(CancellationToken ct)
    {
        var ordeneservicios = await _unitofwork.OrdenesServicios.GetAllAsync(ct); 
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
    public async Task<IActionResult> Create([FromBody] CreateOrdenServicioDto dto, CancellationToken ct)
    {
        var service_orders = new OrdenServicio(dto.TipoServicio, dto.FechaIngreso, dto.FechaEstimadaEntrega, dto.Estado, dto.UserMemberId, dto.VehiculoId);
        await _repository.AddAsync(service_orders, ct);
        await _unitofwork.SaveChangesAsync(ct);

        var created = new OrdenServicioDto(service_orders.Id, service_orders.TipoServicio, service_orders.FechaIngreso, service_orders.FechaEstimadaEntrega, service_orders.Estado, service_orders.UserMemberId, service_orders.VehiculoId);
        return CreatedAtAction(nameof(GetById), new { id = service_orders.Id }, created);
    }
}
