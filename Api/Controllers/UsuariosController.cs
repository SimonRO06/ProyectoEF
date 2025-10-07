using System;
using Api.Dtos.Usuarios;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[EnableRateLimiting("ipLimiter")]
public class UsuariosController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;

    public UsuariosController(IMapper mapper, IUnitOfWork unitofwork)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAll(CancellationToken ct)
    {
        var usuarios = await _unitofwork.Usuarios.GetAllAsync(ct); // necesitas este método en IProductRepository
        var dto = _mapper.Map<IEnumerable<UsuarioDto>>(usuarios);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [DisableRateLimiting]
    public async Task<ActionResult<UsuarioDto>> GetById(Guid id, CancellationToken ct)
    {
        var usuario = await _unitofwork.Usuarios.GetByIdAsync(id, ct);
        if (usuario is null) return NotFound();

        return Ok(_mapper.Map<UsuarioDto>(usuario));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUsuarioDto body, CancellationToken ct)
    {
        var product = _mapper.Map<Usuario>(body);
        await _unitofwork.Usuarios.AddAsync(product, ct);

        var dto = _mapper.Map<UsuarioDto>(product);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}
