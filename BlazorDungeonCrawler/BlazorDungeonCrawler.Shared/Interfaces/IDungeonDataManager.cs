using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Shared.Interfaces
{
    public interface IDungeonDataManager {
        Task<Dungeon> GenerateNewDungeon();
        Task<Dungeon> GetDungeon(Guid dungeonId);
        Task<Dungeon> SelectDungeonTile(Guid dungeonId, Guid tileId);
        Task<Dungeon> AutomaticallyAdvanceDungeon(Guid dungeonId);        
        Task<Dungeon> DescendStairs(Guid dungeonId);        
        Task<Dungeon> MonsterFlee(Guid dungeonId, Guid tileId);
        Task<Dungeon> MonsterFight(Guid dungeonId, Guid tileId);
    }
}
