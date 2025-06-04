using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FinanceTracker.Api.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserRole>? UserRoles { get; set; }
    }
}