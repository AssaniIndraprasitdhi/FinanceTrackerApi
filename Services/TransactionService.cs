using FinanceTracker.Api.Data;
using FinanceTracker.Api.DTOs;
using FinanceTracker.Api.Models;
using FinanceTracker.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly FinanceTrackerDbContext _dbContext;

        public TransactionService(FinanceTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ดึง Transaction ทั้งหมด (Include User และ Category)
        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _dbContext.Transactions
                .Include(t => t.User)
                .Include(t => t.Category)
                .ToListAsync();
        }

        // ดึง Transaction ตาม Id (Include User และ Category)
        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _dbContext.Transactions
                .Include(t => t.User)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // สร้าง Transaction ใหม่
        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            // สมมติต้องการตรวจสอบก่อนว่ามี UserId ที่ส่งมาจริงหรือไม่
            var userExists = await _dbContext.Users.AnyAsync(u => u.Id == transaction.UserId);
            if (!userExists)
                throw new Exception($"User with Id={transaction.UserId} does not exist.");

            // ถ้า CategoryId ไม่เป็น null ก็ตรวจสอบว่ามี CategoryId ใน DB หรือไม่
            if (transaction.CategoryId.HasValue)
            {
                var catExists = await _dbContext.Categories.AnyAsync(c => c.Id == transaction.CategoryId.Value);
                if (!catExists)
                    throw new Exception($"Category with Id={transaction.CategoryId.Value} does not exist.");
            }

            // วันที่ TransactionDate, CreatedAt, UpdatedAt ถูกกำหนดแล้วจากฝั่ง client หรือ default
            await _dbContext.Transactions.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();
            return transaction;
        }

        // อัปเดต Transaction (บางฟิลด์ เช่น Amount, Description, CategoryId, TransactionDate)
        public async Task<bool> UpdateAsync(int id, Transaction updated)
        {
            var existing = await _dbContext.Transactions.FindAsync(id);
            if (existing == null)
                return false;

            // 1. อัปเดตจำนวนเงิน
            existing.Amount = updated.Amount;

            // 2. อัปเดตคำอธิบาย (อาจเป็น null ได้)
            existing.Description = updated.Description;

            // 3. อัปเดตวันที่ทำธุรกรรม
            existing.TransactionDate = updated.TransactionDate;

            // 4. อัปเดต CategoryId (int? → int?)
            existing.CategoryId = updated.CategoryId;
            //    ถ้า updated.CategoryId == null → existing.CategoryId จะถูกตั้งเป็น null
            //    ถ้า updated.CategoryId มีค่า ให้ตรวจสอบว่า Category มีอยู่จริงหรือไม่
            if (updated.CategoryId.HasValue)
            {
                var catExists = await _dbContext.Categories.AnyAsync(c => c.Id == updated.CategoryId.Value);
                if (!catExists)
                    throw new Exception($"Category with Id={updated.CategoryId.Value} does not exist.");
            }

            // 5. อัปเดต UserId (int → int)
            //    สมมติว่า User ไม่เปลี่ยน แตถ้าเปลี่ยน เราต้องตรวจสอบว่า User นั้นมีอยู่
            if (existing.UserId != updated.UserId)
            {
                var userExists = await _dbContext.Users.AnyAsync(u => u.Id == updated.UserId);
                if (!userExists)
                    throw new Exception($"User with Id={updated.UserId} does not exist.");
                existing.UserId = updated.UserId;
            }

            // 6. อัปเดต UpdatedAt
            existing.UpdatedAt = DateTime.UtcNow;

            _dbContext.Transactions.Update(existing);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // ลบ Transaction
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _dbContext.Transactions.FindAsync(id);
            if (existing == null)
                return false;

            _dbContext.Transactions.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public Task<IEnumerable<TransactionDto>> GetAllByUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionDto?> GetByIdAsync(int id, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionDto> CreateAsync(int userId, TransactionDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(int id, int userId, TransactionDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
