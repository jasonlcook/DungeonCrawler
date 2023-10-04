using System.Data.Entity;
using System.Text.RegularExpressions;

using BlazorDungeonCrawler.Shared.Models;
namespace BlazorDungeonCrawler.Server.Data {
    public class DungeonManager {
        public async Task<Dungeon> Generate() {
            await Task.Delay(1);
            return new Dungeon();
        }
        
        public async Task<Dungeon> GetSelectedDungeonTile(Guid dungeonId, Guid tileId) {
            await Task.Delay(1);
            return new Dungeon();
        }

        public async Task<Dungeon> MonsterFlee(Guid dungeonId, Guid tileId) {
            await Task.Delay(1);
            return new Dungeon();
        }

        public async Task<Dungeon> MonsterFight(Guid dungeonId, Guid tileId) {
            await Task.Delay(1);
            return new Dungeon();
        }
    }
}
