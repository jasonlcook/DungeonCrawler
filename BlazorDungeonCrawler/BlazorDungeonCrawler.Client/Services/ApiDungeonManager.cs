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
            } else { 
                //todo: trap error
            }

            return new Dungeon();
        }

        public async Task<Dungeon> GetDungeon(Guid dungeonId) {
            string safeDungeonId = WebUtility.HtmlEncode(dungeonId.ToString());

            string url = $"https://localhost:7224/api/dungeon/{safeDungeonId}";

            HttpResponseMessage? response = await httpClient.GetAsync(url);

            if (response != null) {
                response.EnsureSuccessStatusCode();

                string apiResponse = await response.Content.ReadAsStringAsync();
                if (apiResponse != null) {
                    DungeonResponse? dungeonResponse = JsonConvert.DeserializeObject<DungeonResponse>(apiResponse);
                    if (dungeonResponse != null && dungeonResponse.Success) {
                        return dungeonResponse.Dungeon;
                    }
                }
            } else {
                //todo: trap error
            }

            return new Dungeon();
        }

        public async Task<Dungeon> SelectDungeonTile(Guid dungeonId, Guid tileId) {
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
                    var dungeonResponse = JsonConvert.DeserializeObject<DungeonResponse>(apiResponse);

                    //check result
                    if (dungeonResponse != null && dungeonResponse.Success) {

                        //return safe result
                        return dungeonResponse.Dungeon;
                    }
                }
            }

            //if any of the HTTP elements are null return empty object
            return new Dungeon();
        }

        public async Task<Dungeon> DescendStairs(Guid dungeonId) {
            string safeDungeonId = WebUtility.HtmlEncode(dungeonId.ToString());

            string url = $"https://localhost:7224/api/dungeon/{safeDungeonId}/descendstairs";

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
