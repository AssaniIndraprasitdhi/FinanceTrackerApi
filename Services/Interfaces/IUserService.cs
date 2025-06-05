using FinanceTracker.Api.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceTracker.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<bool> UpdateUserAsync(int id, UserDto dto);
        Task<bool> DeleteUserAsync(int id);
    }
}