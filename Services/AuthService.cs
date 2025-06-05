using FinanceTracker.Api.Data;
using FinanceTracker.Api.DTOs.Auth;
using FinanceTracker.Api.Models;
using FinanceTracker.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace FinanceTracker.Api.Services
{
    public class AuthService(FinanceTrackerDbContext dbContext, IConfiguration configuration) : IAuthService
    {
        private readonly FinanceTrackerDbContext _dbContext = dbContext;
        private readonly IConfiguration _configuration = configuration;

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Username == request.Username))
                throw new Exception("Username already exists.");

            if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
                throw new Exception("Email already exists.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var defaultRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (defaultRole != null)
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = defaultRole.Id
                };
                await _dbContext.UserRoles.AddAsync(userRole);
                await _dbContext.SaveChangesAsync();
            }

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _dbContext.Users
                .Include(u => (IEnumerable<UserRole>)u.UserRoles!)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == request.UsernameOrEmail || u.Email == request.UsernameOrEmail);

            if (user == null)
                throw new Exception("Invalid credentials.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new Exception("Invalid credentials.");

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSecret = _configuration["JWT_SECRET"];
            var jwtIssuer = _configuration["JWT_ISSUER"];
            var jwtAudience = _configuration["JWT_AUDIENCE"];
            var jwtExpire = _configuration["JWT_EXPIRE_MINUTES"];

            if (string.IsNullOrWhiteSpace(jwtSecret))
                throw new InvalidOperationException("JWT_SECRET ถูกตั้งค่าไม่ถูกต้องหรือว่างอยู่ (null/empty). ตรวจสอบไฟล์ .env หรือ Environment Variables อีกครั้ง. ");
            if (string.IsNullOrWhiteSpace(jwtIssuer))
                throw new InvalidOperationException("JWT_ISSUER ถูกตั้งค่าไม่ถูกต้องหรือว่างอยู่ (null/empty).");
            if (string.IsNullOrWhiteSpace(jwtAudience))
                throw new InvalidOperationException("JWT_AUDIENCE ถูกตั้งค่าไม่ถูกต้องหรือว่างอยู่ (null/empty).");
            if (!int.TryParse(jwtExpire, out var jwtExpireMinutes))
                throw new InvalidOperationException("JWT_EXPIRE_MINUTES ไม่ถูกต้อง (ไม่ใช่ตัวเลข) หรือว่างอยู่.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
        new Claim("firstname", user.FirstName),
        new Claim("lastname", user.LastName)
    };
            var roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>();
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var tokenDescriptor = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtExpireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

    }
}