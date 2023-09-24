﻿using BlazorDungeonCrawler.Shared.Models;

using Newtonsoft.Json;

namespace BlazorDungeonCrawler.Client.Services {
    public class ApiDungeonManager : IDungeonDataManager {
        private readonly HttpClient httpClient;

        public ApiDungeonManager(HttpClient _httpClient) {
            httpClient = _httpClient;
        }

        public async Task<Dungeon> GenerateNewDungeon() {
            var result = await httpClient.GetAsync("https://localhost:7224/api/dungeon");

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
