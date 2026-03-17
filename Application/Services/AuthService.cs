using Application.Dtos.Auth;
using Application.Interfaces;
using Domain;
using Domain.Entities;
using Infraestructure.Persistences.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
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

        public async Task<OperationResult<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.FindByUsernameAsync(request.Username);

            if (user == null)
            {
                return OperationResult<LoginResponseDto>.Fail("Usuario o contraseña incorrectos.");
            }

            if (!user.Status)
            {
                return OperationResult<LoginResponseDto>.Fail("El usuario está deshabilitado.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return OperationResult<LoginResponseDto>.Fail("Usuario o contraseña incorrectos.");
            }

            var token = GenerateJwtToken(user);
            var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationMinutes", 60);

            var response = new LoginResponseDto
            {
                Token = token,
                Fullname = user.Fullname,
                Username = user.Username,
                Rol = user.Rol.Name,
                Expiration = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };

            return new OperationResult<LoginResponseDto>
            {
                Success = true,
                Data = response,
                Message = "Login exitoso"
            };
        }

        private string GenerateJwtToken(User user)
        {
            var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key no está configurado.");
            var issuer = _configuration["Jwt:Issuer"] ?? "InventarioApi";
            var audience = _configuration["Jwt:Audience"] ?? "InventarioApp";
            var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationMinutes", 60);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.GivenName, user.Fullname),
                new Claim(ClaimTypes.Role, user.Rol.Name),
                new Claim("RolId", user.RolId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
