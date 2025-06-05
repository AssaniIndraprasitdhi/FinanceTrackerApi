using FinanceTracker.Api.Data;
using FinanceTracker.Api.DTOs;
using FinanceTracker.Api.Models;
using FinanceTracker.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceTracker.Api.Services
{
    public class RoleService : IRoleService
    {
        private readonly FinanceTrackerDbContext _dbContext;

        public RoleService(FinanceTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            return await _dbContext.Roles
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                })
                .ToListAsync();
        }

        public async Task<RoleDto?> GetRoleByIdAsync(int id)
        {
            var r = await _dbContext.Roles.FindAsync(id);
            if (r == null) return null;

            return new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            };
        }

        public async Task<RoleDto> CreateRoleAsync(RoleDto dto)
        {
            var entity = new Role
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Roles.Add(entity);
            await _dbContext.SaveChangesAsync();

            dto.Id = entity.Id; // เซ็ต Id ที่ได้จากฐานข้อมูลกลับเข้า dto
            return dto;
        }

        public async Task<bool> UpdateRoleAsync(int id, RoleDto dto)
        {
            var entity = await _dbContext.Roles.FindAsync(id);
            if (entity == null) return false;

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.UpdatedAt = DateTime.UtcNow;

            _dbContext.Roles.Update(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var entity = await _dbContext.Roles.FindAsync(id);
            if (entity == null) return false;

            _dbContext.Roles.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
