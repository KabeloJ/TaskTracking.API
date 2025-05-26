using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskTracking.Core.Entities;
using TaskTracking.Core.Interfaces;

namespace TaskTracking.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly string _jwtSecretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _jwtSecretKey = configuration["JwtSettings:SecretKey"];
            _issuer = configuration["JwtSettings:Issuer"];
            _audience = configuration["JwtSettings:Audience"];

        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || user.PasswordHash != ComputeHash(password))
                throw new UnauthorizedAccessException("Invalid credentials.");

            return GenerateJwtToken(user);
        }

        public async Task<User> RegisterUserAsync(string username, string email, string password)
        {
            if (await _userRepository.GetUserByEmailAsync(email) != null)
                throw new Exception("User with this email already exists.");

            var hashedPassword = ComputeHash(password);
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hashedPassword,
                Role = "User"
            };

            return await _userRepository.AddUserAsync(user);
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string ComputeHash(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}
