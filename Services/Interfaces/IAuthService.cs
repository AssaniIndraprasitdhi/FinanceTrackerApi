using FinanceTracker.Api.DTOs.Auth;
using System.Threading.Tasks;

namespace FinanceTracker.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}