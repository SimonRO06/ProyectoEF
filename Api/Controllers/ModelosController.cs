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
    private readonly IModeloRepository _repository;


    public ModelosController(IMapper mapper, IUnitOfWork unitofwork, IModeloRepository repository)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
        _repository = repository;
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
    public async Task<IActionResult> Create([FromBody] CreateModeloDto dto, CancellationToken ct)
    {
        var models = new Modelo(dto.Nombre, dto.MarcaId);
        await _repository.AddAsync(models, ct);
        await _unitofwork.SaveChangesAsync(ct);

        var created = new ModeloDto(models.Id, models.Nombre, models.MarcaId);
        return CreatedAtAction(nameof(GetById), new { id = models.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateModeloDto dto, CancellationToken ct)
    {
        var existing = await _unitofwork.Modelos.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        // Actualizamos los campos
        existing.Update(dto.Nombre);

        await _unitofwork.Modelos.UpdateAsync(existing, ct);
        await _unitofwork.SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var existing = await _unitofwork.Modelos.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        await _unitofwork.Modelos.RemoveAsync(existing, ct);
        await _unitofwork.SaveChangesAsync(ct);

        return NoContent();
    }
}
