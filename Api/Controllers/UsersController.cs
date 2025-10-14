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
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[SwaggerTag("Autenticación, registro y gestión de usuarios del sistema")]
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
    [SwaggerOperation(
        Summary = "Obtener todos los usuarios",
        Description = "Retorna una lista de todos los usuarios registrados en el sistema"
    )]
    [SwaggerResponse(200, "Lista de usuarios obtenida exitosamente", typeof(IEnumerable<UserMemberDto>))]
    [SwaggerResponse(401, "No autorizado - Token JWT requerido")]
    [SwaggerResponse(404, "No se encontraron usuarios")]
    public async Task<ActionResult<IEnumerable<UserMemberDto>>> GetAll(CancellationToken ct)
    {
        var usuarios = await _unitOfWork.UserMembers.GetAllAsync(ct);

        if (usuarios == null || !usuarios.Any())
        {
            return NotFound("No se encontraron usuarios.");
        }

        var usuariosDto = _mapper.Map<IEnumerable<UserMemberDto>>(usuarios);
        return Ok(usuariosDto);
    }

    [HttpPost("register")]
    [SwaggerOperation(
        Summary = "Registrar nuevo usuario",
        Description = "Crea una nueva cuenta de usuario en el sistema con rol asignado"
    )]
    [SwaggerResponse(200, "Usuario registrado exitosamente")]
    [SwaggerResponse(400, "Datos de usuario inválidos o email ya registrado")]
    public async Task<IActionResult> Register(
        [SwaggerParameter("Datos de registro del usuario", Required = true)]
        [FromBody] RegisterDto registerDto, 
        CancellationToken ct)
    {
        var result = await _userService.RegisterAsync(registerDto);
        if (result.Contains("Error") || result.Contains("ya se encuentra registrado"))
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "Iniciar sesión",
        Description = "Autentica al usuario y retorna tokens JWT para acceder a los endpoints protegidos"
    )]
    [SwaggerResponse(200, "Login exitoso", typeof(DataUserDto))]
    [SwaggerResponse(401, "Credenciales inválidas - Email o password incorrecto")]
    public async Task<ActionResult<DataUserDto>> Login(
        [SwaggerParameter("Credenciales de acceso", Required = true)]
        [FromBody] LoginDto loginDto, 
        CancellationToken ct)
    {
        var userDto = await _userService.GetTokenAsync(loginDto, ct);
        if (!userDto.IsAuthenticated)
        {
            return Unauthorized(userDto.Message);
        }

        return Ok(userDto);
    }

    [HttpPost("refresh-token")]
    [SwaggerOperation(
        Summary = "Renovar token de acceso",
        Description = "Usa el refresh token para obtener un nuevo access token sin necesidad de login"
    )]
    [SwaggerResponse(200, "Token renovado exitosamente", typeof(DataUserDto))]
    [SwaggerResponse(401, "Refresh token inválido o expirado")]
    public async Task<ActionResult<DataUserDto>> RefreshToken(
        [SwaggerParameter("Refresh token para renovar acceso", Required = true)]
        [FromBody] string refreshToken)
    {
        var userDto = await _userService.RefreshTokenAsync(refreshToken);
        if (!userDto.IsAuthenticated)
        {
            return Unauthorized(userDto.Message);
        }

        return Ok(userDto);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Obtener usuario por ID",
        Description = "Retorna la información de un usuario específico por su ID"
    )]
    [SwaggerResponse(200, "Usuario encontrado", typeof(UserMemberDto))]
    [SwaggerResponse(404, "Usuario no encontrado")]
    public async Task<ActionResult<UserMemberDto>> GetById(
        [SwaggerParameter("ID del usuario", Required = true)]
        int id, 
        CancellationToken ct)
    {
        var user = await _userService.GetByIdAsync(id, ct);
        if (user == null)
        {
            return NotFound($"Usuario con ID {id} no encontrado.");
        }

        var dto = _mapper.Map<UserMemberDto>(user);
        return Ok(dto);
    }

    [HttpPost("add-role")]
    [SwaggerOperation(
        Summary = "Agregar rol a usuario",
        Description = "Asigna un nuevo rol a un usuario existente en el sistema"
    )]
    [SwaggerResponse(200, "Rol agregado exitosamente")]
    [SwaggerResponse(400, "Error al agregar el rol - Usuario o rol inválido")]
    public async Task<IActionResult> AddRole(
        [SwaggerParameter("Datos para asignar rol", Required = true)]
        [FromBody] AddRoleDto addRoleDto, 
        CancellationToken ct)
    {
        var result = await _userService.AddRoleAsync(addRoleDto);
        if (result.Contains("Error") || result.Contains("invalid"))
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}