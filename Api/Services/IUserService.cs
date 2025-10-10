using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.Auth;
using Domain.Entities.Auth;

namespace Api.Services;

public interface IUserService
{
    Task<string> RegisterAsync(RegisterDto model);
    Task<DataUserDto> GetTokenAsync(LoginDto model, CancellationToken ct = default);

    Task<string> AddRoleAsync(AddRoleDto model);
    Task<IEnumerable<UserMember>> GetAllAsync(CancellationToken ct = default);
    Task<UserMember?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<DataUserDto> RefreshTokenAsync(string refreshToken);
}
