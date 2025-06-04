using System;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.AspNetCore.Identity;

namespace FinanceTracker.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserRole>? UserRoles { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
    }
}