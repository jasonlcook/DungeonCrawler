namespace BlazorDungeonCrawler.Shared.Models {
    public interface IDungeonDataManager {
        Task<Dungeon> GenerateNewDungeon();
        Task<Dungeon> SelectDungeonTile(Guid dungeonId, Guid tileId);
    }
}
