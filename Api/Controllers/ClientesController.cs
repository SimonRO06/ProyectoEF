using System;
using Api.Dtos.Clientes;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[EnableRateLimiting("ipLimiter")]
public class ClientesController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;
    public ClientesController(IMapper mapper, IUnitOfWork unitofwork)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
    }
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll(CancellationToken ct)
    {
        var clientes = await _unitofwork.Clientes.GetAllAsync(ct);
        var dto = _mapper.Map<IEnumerable<ClienteDto>>(clientes);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [DisableRateLimiting]
    public async Task<ActionResult<ClienteDto>> GetById(Guid id, CancellationToken ct)
    {
        var cliente = await _unitofwork.Clientes.GetByIdAsync(id, ct);
        if (cliente is null) return NotFound();

        return Ok(_mapper.Map<ClienteDto>(cliente));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClienteDto body, CancellationToken ct)
    {
        var cliente = _mapper.Map<Cliente>(body);
        await _unitofwork.Clientes.AddAsync(cliente, ct);

        var dto = _mapper.Map<ClienteDto>(cliente);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}