using System.Net;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using BlazorDungeonCrawler.Shared.Interfaces;
using BlazorDungeonCrawler.Shared.Responses;

namespace BlazorDungeonCrawler.Client.Services {
    public class ApiAccessTokenManager : IAccessTokenDataManager {
        private readonly string _baseUrl;

        private readonly HttpClient _httpClient;

        public ApiAccessTokenManager(HttpClient httpClient, IOptions<ConfigurationElements> config) {
            this._httpClient = httpClient;

            this._baseUrl = config.Value.BaseUrl;
        }

        private async Task<TokenResponse> ParseServerResponse(HttpResponseMessage? result) {
            if (result != null) {
                string response = await result.Content.ReadAsStringAsync();
                if (response != null) {
                    TokenResponse? tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(response);
                    if (tokenResponse != null) {
                        if (tokenResponse.Success) {
                            return tokenResponse;
                        } else {
                            throw new Exception(tokenResponse.ErrorMessages);
                        }
                    } else {
                        throw new Exception("Badly formed access token response from server.");
                    }
                } else {
                    throw new Exception("Badly formed access token result from server.");
                }
            } else {
                throw new Exception("access token response from server was unexpectedly empty.");
            }
        }

        public async Task<TokenResponse> GenerateNewAccessToken(string name) {
            string safeName = WebUtility.HtmlEncode(name.ToString());

            HttpResponseMessage? result = await _httpClient.GetAsync($"{_baseUrl}/api/accesstoken/new/{safeName}");
            return await ParseServerResponse(result);
        }

        public async Task<TokenResponse> RetrieveAccessToken(string token) {
            string safeToken = WebUtility.HtmlEncode(token.ToString());

            HttpResponseMessage? result = await _httpClient.GetAsync($"{_baseUrl}/api/accesstoken/{safeToken}");
            return await ParseServerResponse(result);
        }
    }
}
