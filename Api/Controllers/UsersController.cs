using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.Auth;
using Api.Services;
using Application.Abstractions;
using Application.Abstractions.Auth;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UsersController(IUserService userService, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<UserMemberDto>>> GetAll(CancellationToken ct)
    {
        var usuarios = await _unitOfWork.UserMembers.GetAllAsync(ct);

        // Depuración para ver qué datos estamos obteniendo
        if (usuarios == null || !usuarios.Any())
        {
            return NotFound("No se encontraron usuarios.");
        }

        // Mostrar los usuarios en el log para ver qué estamos obteniendo
        Console.WriteLine($"Usuarios obtenidos: {usuarios.Count()}");

        // Verifica si los usuarios contienen datos
        foreach (var user in usuarios)
        {
            Console.WriteLine($"Usuario: {user.Username}, Email: {user.Email}");
        }

        var usuariosDto = _mapper.Map<IEnumerable<UserMemberDto>>(usuarios);
        return Ok(usuariosDto);
    }


    // Registrar un nuevo usuario
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, CancellationToken ct)
    {
        var result = await _userService.RegisterAsync(registerDto);
        if (result.Contains("Error") || result.Contains("ya se encuentra registrado"))
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    // Autenticar usuario (login)
    [HttpPost("login")]
    public async Task<ActionResult<DataUserDto>> Login([FromBody] LoginDto loginDto, CancellationToken ct)
    {
        var userDto = await _userService.GetTokenAsync(loginDto, ct);
        if (!userDto.IsAuthenticated)
        {
            return Unauthorized(userDto.Message);
        }

        return Ok(userDto);
    }

    // Refrescar el token usando el refresh token
    [HttpPost("refresh-token")]
    public async Task<ActionResult<DataUserDto>> RefreshToken([FromBody] string refreshToken)
    {
        var userDto = await _userService.RefreshTokenAsync(refreshToken);
        if (!userDto.IsAuthenticated)
        {
            return Unauthorized(userDto.Message);
        }

        return Ok(userDto);
    }

    // Obtener usuario por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<UserMemberDto>> GetById(int id, CancellationToken ct)
    {
        var user = await _userService.GetByIdAsync(id, ct);
        if (user == null)
        {
            return NotFound($"Usuario con ID {id} no encontrado.");
        }

        var dto = _mapper.Map<UserMemberDto>(user);
        return Ok(dto);
    }

    // Agregar un rol a un usuario
    [HttpPost("add-role")]
    public async Task<IActionResult> AddRole([FromBody] AddRoleDto addRoleDto, CancellationToken ct)
    {
        var result = await _userService.AddRoleAsync(addRoleDto);
        if (result.Contains("Error") || result.Contains("invalid"))
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}