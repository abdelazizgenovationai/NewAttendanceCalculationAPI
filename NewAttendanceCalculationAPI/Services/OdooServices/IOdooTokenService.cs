using NewAttendanceCalculationAPI.Services.OdooServices.Dto;

namespace NewAttendanceCalculationAPI.Services.OdooServices
{
    public interface IOdooTokenService
    {
               internal  Task EnsureTokenAsync();
       internal  Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
