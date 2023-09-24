namespace BlazorDungeonCrawler.Shared.Models {
    public interface IDungeonDataManager {
        Task<Dungeon> GenerateNewDungeon();
    }
}
