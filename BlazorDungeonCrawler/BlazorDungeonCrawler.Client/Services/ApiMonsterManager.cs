using BlazorDungeonCrawler.Shared.Models;

using Newtonsoft.Json;

namespace BlazorDungeonCrawler.Client.Services {
    public class ApiMonsterManager : IMonsterDataManager {
        private readonly HttpClient httpClient;

        public ApiMonsterManager(HttpClient _httpClient) {
            httpClient = _httpClient;
        }

        public async Task<List<Monster>> GetAllMonsters() {
            var result = await httpClient.GetAsync("https://localhost:7224/api/monster");

            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            var MonstersResponse = JsonConvert.DeserializeObject<MonstersResponse>(response);
            if (MonstersResponse.Success) {
                return MonstersResponse.Monsters;
            } else {
                return new List<Monster>();
            }
        }

        public Task<Monster> GetMonster(int Id) {
            throw new NotImplementedException();
        }

        public Task<List<Monster>> SearchMonsters(string Name) {
            throw new NotImplementedException();
        }
    }
}
