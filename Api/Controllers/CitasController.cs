using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.Citas;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;
[EnableRateLimiting("ipLimiter")]
public class CitasController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;
    private readonly ICitaRepository _repository;


    public CitasController(IMapper mapper, IUnitOfWork unitofwork, ICitaRepository repository)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
        _repository = repository;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<CitaDto>>> GetAll(CancellationToken ct)
    {
        var cita = await _unitofwork.Citas.GetAllAsync(ct);
        var dto = _mapper.Map<IEnumerable<CitaDto>>(cita);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [DisableRateLimiting]
    public async Task<ActionResult<CitaDto>> GetById(Guid id, CancellationToken ct)
    {
        var cita = await _unitofwork.Citas.GetByIdAsync(id, ct);
        if (cita is null) return NotFound();

        return Ok(_mapper.Map<CitaDto>(cita));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCitaDto dto, CancellationToken ct = default)
    {
        var meeting = new Cita(dto.Fecha, dto.Hora, dto.Observaciones, dto.VehiculoId, dto.ClienteId);
        await _repository.AddAsync(meeting, ct);

        var created = new CitaDto(meeting.Id, meeting.Fecha, meeting.Hora, meeting.Observaciones, meeting.ClienteId, meeting.VehiculoId);
        return CreatedAtAction(nameof(GetById), new { id = meeting.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCitaDto dto, CancellationToken ct)
    {
        var existing = await _unitofwork.Citas.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        // Actualizamos los campos
        existing.Update(dto.Observaciones);

        await _unitofwork.Citas.UpdateAsync(existing, ct);
        await _unitofwork.SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var existing = await _unitofwork.Citas.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        await _unitofwork.Citas.RemoveAsync(existing, ct);
        await _unitofwork.SaveChangesAsync(ct);

        return NoContent();
    }
}