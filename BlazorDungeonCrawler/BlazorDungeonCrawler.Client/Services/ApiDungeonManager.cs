using BlazorDungeonCrawler.Shared.Models;

using Newtonsoft.Json;

namespace BlazorDungeonCrawler.Client.Services {
    public class ApiDungeonManager : IDungeonDataManager {
        private readonly HttpClient httpClient;

        public ApiDungeonManager(HttpClient _httpClient) {
            httpClient = _httpClient;
        }

        public async Task<Dungeon> GenerateNewDungeon() {
            HttpResponseMessage result = await httpClient.GetAsync("https://localhost:7224/api/dungeon");

            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            var dungeonResponse = JsonConvert.DeserializeObject<DungeonResponse>(response);
            if (dungeonResponse != null && dungeonResponse.Success) {
                return dungeonResponse.Dungeon;
            }

            return new Dungeon();
        }

        public async Task<Dungeon> SelectDungeonTile(Guid dungeonId, Guid tileId) {
            var result = await httpClient.GetAsync($"https://localhost:7224/api/dungeon/{dungeonId}/tile/{tileId}");
            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            var dungeonResponse = JsonConvert.DeserializeObject<DungeonResponse>(response);
            if (dungeonResponse != null && dungeonResponse.Success) {
                return dungeonResponse.Dungeon;
            }

            return new Dungeon();
        }

        public async Task<Dungeon> MonsterFlee(Guid dungeonId, Guid tileId) {
            var result = await httpClient.GetAsync($"https://localhost:7224/api/dungeon/{dungeonId}/tile/{tileId}/flee");
            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            var dungeonResponse = JsonConvert.DeserializeObject<DungeonResponse>(response);
            if (dungeonResponse != null && dungeonResponse.Success) {
                return dungeonResponse.Dungeon;
            }

            return new Dungeon();
        }

        public async Task<Dungeon> MonsterFight(Guid dungeonId, Guid tileId) {
            var result = await httpClient.GetAsync($"https://localhost:7224/api/dungeon/{dungeonId}/tile/{tileId}/fight");
            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            var dungeonResponse = JsonConvert.DeserializeObject<DungeonResponse>(response);
            if (dungeonResponse != null && dungeonResponse.Success) {
                return dungeonResponse.Dungeon;
            }

            return new Dungeon();
        }   
    }
}
