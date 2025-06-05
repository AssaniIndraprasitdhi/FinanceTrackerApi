// Services/UserService.cs
using FinanceTracker.Api.Data;
using FinanceTracker.Api.DTOs;
using FinanceTracker.Api.Models;
using FinanceTracker.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Services
{
    public class UserService(FinanceTrackerDbContext dbContext) : IUserService
    {
        private readonly FinanceTrackerDbContext _dbContext = dbContext;

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _dbContext.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName
                })
                .ToListAsync();
        }
        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var u = await _dbContext.Users.FindAsync(id);
            if (u == null) return null;

            return new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName
            };
        }

        public async Task<bool> UpdateUserAsync(int id, UserDto dto)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null) return false;

            user.Username = dto.Username;
            user.Email = dto.Email;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.UpdatedAt = DateTime.UtcNow;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null) return false;

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
