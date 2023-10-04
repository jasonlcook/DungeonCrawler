using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Shared.Interfaces
{
    public interface IDungeonDataManager {
        Task<Dungeon> GenerateNewDungeon();
        Task<Tile> SelectDungeonTile(Guid dungeonId, Guid tileId);
        Task<Dungeon> MonsterFlee(Guid dungeonId, Guid tileId);
        Task<Dungeon> MonsterFight(Guid dungeonId, Guid tileId);
    }
}
