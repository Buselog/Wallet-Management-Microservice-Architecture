using WM.Gateway.Dtos;

namespace WM.Gateway.Services.Abstracts
{
    public interface IDashboardService
    {
        Task<UserDashboardDto> GetUserSummaryAsync(string customerNo, string token);
    }
}
