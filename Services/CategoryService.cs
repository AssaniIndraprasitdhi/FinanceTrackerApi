using FinanceTracker.Api.Data;
using FinanceTracker.Api.DTOs;
using FinanceTracker.Api.Models;
using FinanceTracker.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Services
{
    public class CategoryService(FinanceTrackerDbContext dbContext) : ICategoryService
    {
        private readonly FinanceTrackerDbContext _dbContext = dbContext;

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            return await _dbContext.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsExpense = c.IsExpense
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.Categories.FindAsync(id);
            if (entity == null) return null;

            return new CategoryDto
            {
                Id = entity.Id,
                Name = entity.Name,
                IsExpense = entity.IsExpense
            };
        }

        public async Task<CategoryDto> CreateAsync(CategoryDto dto)
        {
            var entity = new Category
            {
                Name = dto.Name,
                IsExpense = dto.IsExpense,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Categories.Add(entity);
            await _dbContext.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, CategoryDto dto)
        {
            var entity = await _dbContext.Categories.FindAsync(id);
            if (entity == null) return false;

            entity.Name = dto.Name;
            entity.IsExpense = dto.IsExpense;
            entity.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbContext.Categories.FindAsync(id);
            if (entity == null) return false;

            _dbContext.Categories.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
