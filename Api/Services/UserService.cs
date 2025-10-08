using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Api.Dtos.Auth;
using Api.Helpers;
using Application.Abstractions;
using Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services;

public class UserService : IUserService
{
     private readonly JWT _jwt;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    public UserService(IOptions<JWT> jwt, IUnitOfWork unitOfWork, IPasswordHasher<Usuario> passwordHasher)
    {
        _jwt = jwt.Value;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }
    public async Task<string> RegisterAsync(RegisterDto registerDto)
    {
        var usuario = new Usuario
        {
            Nombre = registerDto.Nombre ?? throw new ArgumentNullException(nameof(registerDto.Nombre)),
            Correo = registerDto.Correo ?? throw new ArgumentNullException(nameof(registerDto.Correo)),
            Contraseña = registerDto.Contraseña ?? throw new ArgumentNullException(nameof(registerDto.Contraseña)),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        usuario.Contraseña = _passwordHasher.HashPassword(usuario, registerDto.Password!);

        var usuarioExiste = _unitOfWork.Usuarios
                                    .Find(u => u.Nombre.ToLower() == registerDto.Nombre.ToLower())
                                    .FirstOrDefault();

        if (usuarioExiste == null)
        {
            var defaultRoleName = UserAuthorization.rol_default.ToString();
            var rolPredeterminado = _unitOfWork.Roles
                                    .Find(u => EF.Functions.ILike(u.Nombre, defaultRoleName))
                                    .FirstOrDefault();
            if (rolPredeterminado == null)
            {
                try
                {
                    // Intenta crear el rol por defecto si no existe
                    var nuevoRol = new Rol
                    {
                        Nombre = defaultRoleName,
                        Description = "Default role"
                    };
                    await _unitOfWork.Roles.AddAsync(nuevoRol);
                    await _unitOfWork.SaveChanges();
                    rolPredeterminado = nuevoRol;
                }
                catch
                {
                    // Si otro proceso lo creó en paralelo, reintenta obtenerlo
                    rolPredeterminado = _unitOfWork.Roles
                                        .Find(u => EF.Functions.ILike(u.Nombre, defaultRoleName))
                                        .FirstOrDefault();
                    if (rolPredeterminado == null)
                    {
                        return $"No se encontró ni pudo crearse el rol predeterminado '{defaultRoleName}'.";
                    }
                }
            }
            try
            {
                usuario.Rols.Add(rolPredeterminado);
                await _unitOfWork.Usuarios.AddAsync(usuario);
                await _unitOfWork.SaveChanges();

                return $"El usuario  {registerDto.Nombre} ha sido registrado exitosamente";
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return $"Error: {message}";
            }
        }
        else
        {
            return $"El usuario con {registerDto.Nombre} ya se encuentra registrado.";
        }
    }

    public async Task<DataUserDto> GetTokenAsync(LoginDto model, CancellationToken ct = default)
    {
        var dto = new DataUserDto { IsAuthenticated = false };

        // 1) Normalización y validación básica (evita enumeración de usuarios)
        var username = model.Nombre?.Trim();
        var password = model.Contraseña ?? string.Empty;

        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(contraseña))
        {
            dto.Message = "Usuario o contraseña inválidos.";
            return dto;
        }

        // 2) Lookup del usuario (ideal: repositorio case-insensitive/normalizado)
        var usuario = await _unitOfWork.Usuarios.GetByUserNameAsync(nombre, ct);
        if (usuario is null)
        {
            dto.Message = "Usuario o contraseña inválidos.";
            return dto;
        }

        // 3) Verificación de contraseña (+ rehash si se requiere)
        var verification = _passwordHasher.VerifyHashedPassword(usuario, usuario.Contraseña, contraseña);
        if (verification == PasswordVerificationResult.Failed)
        {
            dto.Message = "Usuario o contraseña inválidos.";
            return dto;
        }

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.Contraseña = _passwordHasher.HashPassword(usuario, contraseña);
            await _unitOfWork.Usuarios.UpdateAsync(usuario, ct);
            // No retornamos aún; seguimos el flujo normal
        }

        // 4) Preparar colecciones de forma segura
        var roles = usuario.Rols?.Select(r => r.Name).ToList() ?? new List<string>();
        usuario.RefreshTokens ??= new List<RefreshToken>();

        // 5) Transacción: rotación de refresh tokens + persistencia
        await _unitOfWork.ExecuteInTransactionAsync(async _ =>
        {
            // Política: ROTACIÓN. Revoca todos los activos antes de emitir uno nuevo
            foreach (var t in usuario.RefreshTokens.Where(t => t.IsActive))
            {
                t.Revoked = DateTime.UtcNow;                 // UTC          // opcional
            }

            var refresh = CreateRefreshToken();
            usuario.RefreshTokens.Add(refresh);

            await _unitOfWork.Usuarios.UpdateAsync(usuario, ct);
            await _unitOfWork.SaveChanges(ct);
        }, ct);

        // 6) Emitir JWT (usa tu CreateJwtToken existente)
        var jwt = CreateJwtToken(usuario);

        // 7) Salida consistente (UTC y DateTimeOffset?)
        var currentRefresh = usuario.RefreshTokens.OrderByDescending(t => t.Created).First();

        dto.IsAuthenticated = true;
        dto.Token = new JwtSecurityTokenHandler().WriteToken(jwt);
        dto.Correo = usuario.Email;
        dto.Nombre = usuario.Username;
        dto.Roles = roles;
        dto.RefreshToken = currentRefresh.Token;
        dto.RefreshTokenExpiration = DateTime.SpecifyKind(currentRefresh.Expires, DateTimeKind.Utc);

        return dto;
    }
    private JwtSecurityToken CreateJwtToken(Usuario usuario)
    {
        var roles = usuario.Rols;
        var roleClaims = new List<Claim>();
        foreach (var role in roles)
        {
            roleClaims.Add(new Claim("roles", role.Nombre));
        }
        var claims = new[]
        {
                                new Claim(JwtRegisteredClaimNames.Sub, usuario.Nombre),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                new Claim(JwtRegisteredClaimNames.Correo, usuario.Correo),
                                new Claim("uid", usuario.Id.ToString())
                        }
        .Union(roleClaims);
        if (string.IsNullOrEmpty(_jwt.Key))
        {
            throw new InvalidOperationException("JWT Key cannot be null or empty.");
        }
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }
    private RefreshToken CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(randomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                Expires = DateTime.UtcNow.AddDays(10),
                Created = DateTime.UtcNow
            };
        }
    }
    public async Task<string> AddRoleAsync(AddRoleDto model)
    {

        if (string.IsNullOrEmpty(model.Nombre))
        {
            return "Username cannot be null or empty.";
        }
        var user = await _unitOfWork.Usuarios
                    .GetByUserNameAsync(model.Nombre);
        if (user == null)
        {
            return $"User {model.Nombre} does not exists.";
        }

        if (string.IsNullOrEmpty(model.Contraseña))
        {
            return $"Password cannot be null or empty.";
        }

        var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Contraseña, model.Contraseña);

        if (result == PasswordVerificationResult.Success)
        {
            if (string.IsNullOrWhiteSpace(model.Role))
            {
                return "Role name cannot be null or empty.";
            }

            var roleName = model.Role.Trim();

            var rolExists = _unitOfWork.Roles
                                        .Find(u => EF.Functions.ILike(u.Nombre, roleName))
                                        .FirstOrDefault();

            if (rolExists == null)
            {
                try
                {
                    var nuevoRol = new Rol
                    {
                        Nombre = roleName,
                        Description = $"{roleName} role"
                    };
                    await _unitOfWork.Roles.AddAsync(nuevoRol);
                    await _unitOfWork.SaveChanges();
                    rolExists = nuevoRol;
                }
                catch
                {
                    // Race condition: role created concurrently, re-fetch
                    rolExists = _unitOfWork.Roles
                                .Find(u => EF.Functions.ILike(u.Nombre, roleName))
                                .FirstOrDefault();
                    if (rolExists == null)
                    {
                        return $"No se encontró ni pudo crearse el rol '{roleName}'.";
                    }
                }
            }

            var userHasRole = usuario.Rols.Any(r => r.Nombre.Equals(roleName, StringComparison.OrdinalIgnoreCase) || r.Id == rolExists.Id);
            if (!userHasRole)
            {
                user.Rols.Add(rolExists);
                await _unitOfWork.Usuarios.UpdateAsync(usuario);
                await _unitOfWork.SaveChanges();
            }

            return $"Role {roleName} added to user {model.Nombre} successfully.";
        }
        return $"Invalid Credentials";
    }
    public async Task<DataUserDto> RefreshTokenAsync(string refreshToken)
    {
        var dataUserDto = new DataUserDto();

        var usuario = await _unitOfWork.Usuarios
                        .GetByRefreshTokenAsync(refreshToken);

        if (usuario == null)
        {
            dataUserDto.IsAuthenticated = false;
            dataUserDto.Message = $"Token is not assigned to any user.";
            return dataUserDto;
        }

        var refreshTokenBd = usuario.RefreshTokens.Single(x => x.Token == refreshToken);

        if (!refreshTokenBd.IsActive)
        {
            dataUserDto.IsAuthenticated = false;
            dataUserDto.Message = $"Token is not active.";
            return dataUserDto;
        }
        //Revoque the current refresh token and
        refreshTokenBd.Revoked = DateTime.UtcNow;
        //generate a new refresh token and save it in the database
        var newRefreshToken = CreateRefreshToken();
        usuario.RefreshTokens.Add(newRefreshToken);
        await _unitOfWork.Usuarios.UpdateAsync(usuario);
        await _unitOfWork.SaveChanges();
        //Generate a new Json Web Token
        dataUserDto.IsAuthenticated = true;
        JwtSecurityToken jwtSecurityToken = CreateJwtToken(usuario);
        dataUserDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        dataUserDto.Correo = usuario.Correo;
        dataUserDto.Nombre = usuario.Nombre;
        dataUserDto.Roles = usuario.Rols
                                        .Select(u => u.Nombre)
                                        .ToList();
        dataUserDto.RefreshToken = newRefreshToken.Token;
        dataUserDto.RefreshTokenExpiration = newRefreshToken.Expires;
        return dataUserDto;
    }
}