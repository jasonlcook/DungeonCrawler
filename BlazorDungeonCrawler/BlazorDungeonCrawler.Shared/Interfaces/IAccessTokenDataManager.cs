using BlazorDungeonCrawler.Shared.Responses;

namespace BlazorDungeonCrawler.Shared.Interfaces
{
    public interface IAccessTokenDataManager {
        Task<TokenResponse> GenerateNewAccessToken(string name);
        Task<TokenResponse> RetrieveAccessToken(string token);
    }
}
