using FinanceTracker.Api.Data;
using FinanceTracker.Api.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceTracker.Api.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(int id);
        Task<RoleDto> CreateRoleAsync(RoleDto dto);

        Task<bool> UpdateRoleAsync(int id, RoleDto dto);
        Task<bool> DeleteRoleAsync(int id);
    }
}
