﻿namespace BlazorDungeonCrawler.Shared.Models {
    public interface IDungeonDataManager {
        Task<Dungeon> GenerateNewDungeon();
        Task<Dungeon> SelectDungeonTile(Guid dungeonId, Guid tileId);
        Task<Dungeon> MonsterFlee(Guid dungeonId, Guid tileId);
        Task<Dungeon> MonsterFight(Guid dungeonId, Guid tileId);
    }
}
