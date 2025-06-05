using FinanceTracker.Api.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceTracker.Api.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDto>> GetAllByUserAsync(int userId);
        Task<TransactionDto?> GetByIdAsync(int id, int userId);
        Task<TransactionDto> CreateAsync(int userId, TransactionDto dto);
        Task<bool> UpdateAsync(int id, int userId, TransactionDto dto);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
