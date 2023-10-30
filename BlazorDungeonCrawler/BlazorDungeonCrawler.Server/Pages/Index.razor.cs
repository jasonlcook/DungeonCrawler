using Microsoft.JSInterop;

namespace BlazorDungeonCrawler.Server.Pages {
    public partial class Index {
        private string DungeonIdValue { get; set; }
        private string TileIdValue { get; set; }

        public Index() {
            DungeonIdValue = string.Empty;
            TileIdValue = string.Empty;
        }

        private async Task GenerateNewDungon() {
            logger.LogInformation("API called Server page function GenerateNewDungon");

            string url = "./api/dungeon";
            await OpenTab(url);
        }

        private async Task RetrieveDungeon() {
            logger.LogInformation("API called Server page function RetrieveDungeon");
            string url = $"./api/dungeon/{DungeonIdValue}";
            await OpenTab(url);
        }

        private async Task AutomaticallyAdvanceDungeon() {
            logger.LogInformation("API called Server page function AutomaticallyAdvanceDungeon");
            string url = $"./api/dungeon/{DungeonIdValue}/automaticallyadvancedungeon";
            await OpenTab(url);
        }

        private async Task DescendDungeonStairs() {
            logger.LogInformation("API called Server page function DescendDungeonStairs");
            string url = $"./api/dungeon/{DungeonIdValue}/descendstairs";
            await OpenTab(url);
        }

        private async Task SelectDungeonTile() {
            logger.LogInformation("API called Server page function SelectDungeonTile");
            string url = $"./api/dungeon/{DungeonIdValue}/tile/{TileIdValue}";
            await OpenTab(url);
        }

        private async Task FleeFromTile() {
            logger.LogInformation("API called Server page function FleeFromTile");
            string url = $"./api/dungeon/{DungeonIdValue}/tile/{TileIdValue}/flee";
            await OpenTab(url);
        }

        private async Task FightAtTile() {
            logger.LogInformation("API called Server page function FightAtTile");
            string url = $"./api/dungeon/{DungeonIdValue}/tile/{TileIdValue}/fight";
            await OpenTab(url);
        }

        private async Task OpenTab(string url) {
            await this.JS.InvokeVoidAsync("open", url, "_blank");
        }
    }
}