using System.Net;

using Newtonsoft.Json;

using BlazorDungeonCrawler.Shared.Interfaces;
using BlazorDungeonCrawler.Shared.Models;
using BlazorDungeonCrawler.Shared.Responses;

namespace BlazorDungeonCrawler.Client.Services {
    public class ApiDungeonManager : IDungeonDataManager {
        private readonly HttpClient httpClient;

        public ApiDungeonManager(HttpClient _httpClient) {
            httpClient = _httpClient;
        }

        public async Task<Dungeon> GenerateNewDungeon() {
            HttpResponseMessage? response = await httpClient.GetAsync("https://localhost:7224/api/dungeon");

            if (response != null) {
                response.EnsureSuccessStatusCode();

                string apiResponse = await response.Content.ReadAsStringAsync();
                if (apiResponse != null) {
                    DungeonResponse? dungeonResponse = JsonConvert.DeserializeObject<DungeonResponse>(apiResponse);
                    if (dungeonResponse != null && dungeonResponse.Success) {
                        return dungeonResponse.Dungeon;
                    }
                }
            }

            return new Dungeon();
        }

        public async Task<Tile> SelectDungeonTile(Guid dungeonId, Guid tileId) {
            //escape inputs 
            string safeDungeonId = WebUtility.HtmlEncode(dungeonId.ToString());
            string safeTileId = WebUtility.HtmlEncode(tileId.ToString());

            //parse url
            string url = $"https://localhost:7224/api/dungeon/{safeDungeonId}/tile/{safeTileId}";

            //get HTTP response
            HttpResponseMessage? response = await httpClient.GetAsync(url);

            if (response != null) {
                response.EnsureSuccessStatusCode();

                //read response results
                string? apiResponse = await response.Content.ReadAsStringAsync();

                if (apiResponse != null) {
                    //deserialize JSON string
                    TileResponse? tileResponse = JsonConvert.DeserializeObject<TileResponse>(apiResponse);

                    //check result
                    if (tileResponse != null && tileResponse.Success) {

                        //return safe result
                        return tileResponse.Tile;
                    }
                }
            }

            //if any of the HTTP elements are null return empty object
            return new Tile();
        }

        public async Task<Dungeon> MonsterFlee(Guid dungeonId, Guid tileId) {
            string safeDungeonId = WebUtility.HtmlEncode(dungeonId.ToString());
            string safeTileId = WebUtility.HtmlEncode(tileId.ToString());

            string url = $"https://localhost:7224/api/dungeon/{safeDungeonId}/tile/{safeTileId}/flee";

            HttpResponseMessage? response = await httpClient.GetAsync(url);

            if (response != null) {
                response.EnsureSuccessStatusCode();

                string apiResponse = await response.Content.ReadAsStringAsync();
                if (apiResponse != null) {
                    var dungeonResponse = JsonConvert.DeserializeObject<DungeonResponse>(apiResponse);
                    if (dungeonResponse != null && dungeonResponse.Success) {
                        return dungeonResponse.Dungeon;
                    }
                }
            } 
        
            return new Dungeon();
        }

        public async Task<Dungeon> MonsterFight(Guid dungeonId, Guid tileId) {
            //escape inputs
            string safeDungeonId = WebUtility.HtmlEncode(dungeonId.ToString());
            string safeTileId = WebUtility.HtmlEncode(tileId.ToString());

            HttpResponseMessage? response = await httpClient.GetAsync($"https://localhost:7224/api/dungeon/{safeDungeonId}/tile/{safeTileId}/fight");

            if (response != null) {
                response.EnsureSuccessStatusCode();

                string apiResponse = await response.Content.ReadAsStringAsync();
                var dungeonResponse = JsonConvert.DeserializeObject<DungeonResponse>(apiResponse);
                if (dungeonResponse != null && dungeonResponse.Success) {
                    return dungeonResponse.Dungeon;
                }
            }            

            return new Dungeon();
        }
    }
}
