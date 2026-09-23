using System;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PersonalizedInvention.Application.DTOs;
using PersonalizedInvention.Application.Interfaces;
using PersonalizedInvention.Domain.Entities;
using PersonalizedInvention.Domain.Interfaces;

namespace PersonalizedInvention.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (await _userRepository.EmailExistsAsync(dto.Email))
                throw new InvalidOperationException("An account with this email already exists.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower().Trim(),
                PasswordHash = passwordHash,
                IsAdmin = false,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _userRepository.CreateAsync(user);
            var token = GenerateJwtToken(created);
            return BuildResponse(created, token);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            var token = GenerateJwtToken(user);
            return BuildResponse(user, token);
        }

        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["Jwt:SecretKey"]!;
            var issuer = _configuration["Jwt:Issuer"]!;
            var audience = _configuration["Jwt:Audience"]!;
            var expiryDays = int.Parse(_configuration["Jwt:ExpiryInDays"]!);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name,               user.FullName),
            new Claim("userId",                      user.Id.ToString()),
            new Claim("isAdmin",                     user.IsAdmin.ToString().ToLower()) // ← new
        };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(expiryDays),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static AuthResponseDto BuildResponse(User user, string token) => new()
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            IsAdmin = user.IsAdmin,    // ← new
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
    }
}
