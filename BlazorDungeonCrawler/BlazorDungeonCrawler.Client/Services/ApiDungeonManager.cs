using System.Net;

using Newtonsoft.Json;

using BlazorDungeonCrawler.Shared.Interfaces;
using BlazorDungeonCrawler.Shared.Models;
using BlazorDungeonCrawler.Shared.Responses;
using Microsoft.Extensions.Options;

namespace BlazorDungeonCrawler.Client.Services {
    public class ApiDungeonManager : IDungeonDataManager {
        private readonly string _baseUrl;

        private readonly HttpClient _httpClient;

        public ApiDungeonManager(HttpClient httpClient, IOptions<ConfigurationElements> config) {
            this._httpClient = httpClient;

            this._baseUrl = config.Value.BaseUrl;
        }

        private async Task<Dungeon> ParseServerResponse(HttpResponseMessage? result) {
            if (result != null) {
                string response = await result.Content.ReadAsStringAsync();
                if (response != null) {
                    DungeonResponse? dungeonResponse = JsonConvert.DeserializeObject<DungeonResponse>(response);
                    if (dungeonResponse != null) {
                        if (dungeonResponse.Success) {
                            return dungeonResponse.Dungeon;
                        } else {
                            throw new Exception(dungeonResponse.ErrorMessages);
                        }
                    } else {
                        throw new Exception("Badly formed Dungeon response from server.");
                    }
                } else {
                    throw new Exception("Badly formed Dungeon result from server.");
                }
            } else {
                throw new Exception("Dungeon response from server was unexpectedly empty.");
            }
        }

        public async Task<Dungeon> GenerateNewDungeon() {
            HttpResponseMessage? result = await _httpClient.GetAsync($"{_baseUrl}/api/dungeon");
            return await ParseServerResponse(result);
        }

        public async Task<Dungeon> GetDungeon(Guid dungeonId) {
            string safeDungeonId = WebUtility.HtmlEncode(dungeonId.ToString());

            string url = $"{_baseUrl}/api/dungeon/{safeDungeonId}";

            HttpResponseMessage? result = await _httpClient.GetAsync(url);
            return await ParseServerResponse(result);
        }

        public async Task<Dungeon> SelectDungeonTile(Guid dungeonId, Guid tileId) {
            //escape inputs 
            string safeDungeonId = WebUtility.HtmlEncode(dungeonId.ToString());
            string safeTileId = WebUtility.HtmlEncode(tileId.ToString());

            //parse url
            string url = $"{_baseUrl}/api/dungeon/{safeDungeonId}/tile/{safeTileId}";

            //get HTTP response
            HttpResponseMessage? result = await _httpClient.GetAsync(url);
            return await ParseServerResponse(result);
        }

        public async Task<Dungeon> AutomaticallyAdvanceDungeon(Guid dungeonId) {
            //escape inputs 
            string safeDungeonId = WebUtility.HtmlEncode(dungeonId.ToString());

            //parse url
            string url = $"{_baseUrl}/api/dungeon/{safeDungeonId}/automaticallyadvancedungeon";

            //get HTTP response
            HttpResponseMessage? result = await _httpClient.GetAsync(url);
            return await ParseServerResponse(result);
        }


        public async Task<Dungeon> DescendStairs(Guid dungeonId) {
            string safeDungeonId = WebUtility.HtmlEncode(dungeonId.ToString());

            string url = $"{_baseUrl}/api/dungeon/{safeDungeonId}/descendstairs";

            HttpResponseMessage? response = await _httpClient.GetAsync(url);
            return await ParseServerResponse(response);
        }

        public async Task<Dungeon> MonsterFlee(Guid dungeonId, Guid tileId) {
            string safeDungeonId = WebUtility.HtmlEncode(dungeonId.ToString());
            string safeTileId = WebUtility.HtmlEncode(tileId.ToString());

            string url = $"{_baseUrl}/api/dungeon/{safeDungeonId}/tile/{safeTileId}/flee";

            HttpResponseMessage? result = await _httpClient.GetAsync(url);
            return await ParseServerResponse(result);
        }

        public async Task<Dungeon> MonsterFight(Guid dungeonId, Guid tileId) {
            //escape inputs
            string safeDungeonId = WebUtility.HtmlEncode(dungeonId.ToString());
            string safeTileId = WebUtility.HtmlEncode(tileId.ToString());

            HttpResponseMessage? result = await _httpClient.GetAsync($"{_baseUrl}/api/dungeon/{safeDungeonId}/tile/{safeTileId}/fight");
            return await ParseServerResponse(result);
        }
    }
}
