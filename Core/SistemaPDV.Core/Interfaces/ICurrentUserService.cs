namespace SistemaPDV.Core.Interfaces
{
    public interface ICurrentUserService
    {
        int GetRestauranteId();
        Task<int> GetRestauranteIdAsync();
        int? GetUserId();
        string GetUserName();
        bool IsAuthenticated();
        string GetUserAgent();
        string GetUserIpAddress();
    }
}
