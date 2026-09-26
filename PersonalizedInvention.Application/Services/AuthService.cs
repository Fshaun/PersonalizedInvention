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
<<<<<<< HEAD
            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(dto.Email))
                throw new InvalidOperationException("An account with this email already exists.");

            // Hash the password — never store plain text passwords
=======
            if (await _userRepository.EmailExistsAsync(dto.Email))
                throw new InvalidOperationException("An account with this email already exists.");

>>>>>>> beta
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower().Trim(),
                PasswordHash = passwordHash,
<<<<<<< HEAD
=======
                IsAdmin = false,
>>>>>>> beta
                CreatedAt = DateTime.UtcNow
            };

            var created = await _userRepository.CreateAsync(user);
            var token = GenerateJwtToken(created);
<<<<<<< HEAD

=======
>>>>>>> beta
            return BuildResponse(created, token);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
<<<<<<< HEAD
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            // Verify password against stored hash
=======
            var user = await _userRepository.GetByEmailAsync(dto.Email);

>>>>>>> beta
            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            var token = GenerateJwtToken(user);
            return BuildResponse(user, token);
        }

<<<<<<< HEAD
        // ── Private helpers ───────────────────────────────────────────────

=======
>>>>>>> beta
        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["Jwt:SecretKey"]!;
            var issuer = _configuration["Jwt:Issuer"]!;
            var audience = _configuration["Jwt:Audience"]!;
            var expiryDays = int.Parse(_configuration["Jwt:ExpiryInDays"]!);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

<<<<<<< HEAD
            // Claims are pieces of data stored inside the token
            // Angular reads these to know who the user is
=======
>>>>>>> beta
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name,               user.FullName),
<<<<<<< HEAD
            new Claim("userId",                      user.Id.ToString())
=======
            new Claim("userId",                      user.Id.ToString()),
            new Claim("isAdmin",                     user.IsAdmin.ToString().ToLower()) // ← new
>>>>>>> beta
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
<<<<<<< HEAD
=======
            IsAdmin = user.IsAdmin,    // ← new
>>>>>>> beta
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
    }
}
