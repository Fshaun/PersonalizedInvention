using PersonalizedInvention.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalizedInvention.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}
