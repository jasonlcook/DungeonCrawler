namespace BlazorDungeonCrawler.Shared.Models {
    public interface IDungeonManager {
        Task<Dungeon> NewGame();
    }
}
