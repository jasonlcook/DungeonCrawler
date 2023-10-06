using BlazorDungeonCrawler.Server.Models;
using BlazorDungeonCrawler.Shared.Enumerators;

using BlazorDungeonCrawler.Database.Resources.Commands.Create;
using BlazorDungeonCrawler.Database.Resources.Commands.Update;

using BlazorDungeonCrawler.Database.Resources.Queries.Get;

using SharedDungeon = BlazorDungeonCrawler.Shared.Models.Dungeon;
using SharedAdventurer = BlazorDungeonCrawler.Shared.Models.Adventurer;
using SharedLevel = BlazorDungeonCrawler.Shared.Models.Level;
using SharedTile = BlazorDungeonCrawler.Shared.Models.Tile;
using SharedMessage = BlazorDungeonCrawler.Shared.Models.Message;

namespace BlazorDungeonCrawler.Server.Data {
    public class DungeonManager {

        public async Task<SharedDungeon> Generate() {
            await Task.Delay(1);

            Messages messages = new Messages();

            //Create
            //  Adventurer
            int health = Dice.RollDSix();
            messages.Add(new Message(0, $"ADVENTURER HEALTH {health}", health));

            int damage = Dice.RollDSix();
            messages.Add(new Message(1, $"ADVENTURER DAMAGE {damage}", damage));

            int protection = Dice.RollDSix();
            messages.Add(new Message(2, $"ADVENTURER PROTECTION {protection}", protection));

            Adventurer adventurer = new(health, damage, protection);
            SharedAdventurer sharedAdventurer = adventurer.SharedModelMapper();

            //  Level
            int depth = 1;
            Level level = new(depth);
            SharedLevel sharedLevel = level.SharedModelMapper();

            messages.Add(new Message(3, $"DUNGEON DEPTH {depth}"));

            //  Tiles
            Tiles tiles = new(level.Depth, level.Rows, level.Columns);
            List<SharedTile> sharedTiles = tiles.SharedModelMapper();

            sharedLevel.Tiles = sharedTiles;

            //  Messages
            List<SharedMessage> sharedMessages = messages.SharedModelMapper();

            //  Dungon
            string apiVersion = new Version(0, 2, 0).ToString();
            SharedDungeon sharedDungeon = new() {
                Id = Guid.NewGuid(),

                Adventurer = sharedAdventurer,
                Level = sharedLevel,
                Messages = sharedMessages,

                ApiVersion = apiVersion
            };

            DungeonCreate.Create(sharedDungeon);

            return sharedDungeon;
        }

        public async Task<SharedDungeon> RetrieveDungeon(Guid dungeonId) {
            await Task.Delay(1);

            SharedDungeon? sharedDungeon = DungeonQueries.Get(dungeonId);
            if (sharedDungeon == null || sharedDungeon.Id == Guid.Empty) {
                sharedDungeon = await Generate();
            }

            return sharedDungeon;
        }

        public async Task<List<SharedTile>> GetSelectedDungeonTiles(Guid dungeonId, Guid tileId) {
            await Task.Delay(1);

            List<SharedTile> sharedTiles = Tiles.GetSharedDungeonTiles(dungeonId);
            Tiles tiles = new Tiles(sharedTiles);

            Tile selectedTile = new();
            foreach (Tile tile in tiles.GetTiles()) { 
                tile.Current = false;

                if (tile.Id == tileId) {
                    selectedTile = tile;

                    selectedTile.Hidden = false;
                    selectedTile.Current = true;
                }
            }

            tiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);

            switch (selectedTile.Type) {
                case DungeonEvemts.Fight:
                case DungeonEvemts.Unknown:
                case DungeonEvemts.Empty:
                case DungeonEvemts.DungeonEntrance:
                case DungeonEvemts.StairsAscending:
                case DungeonEvemts.StairsDescending:
                case DungeonEvemts.FightWon:
                case DungeonEvemts.FightLost:
                case DungeonEvemts.Chest:
                case DungeonEvemts.FoundWeapon:
                case DungeonEvemts.FoundProtection:
                case DungeonEvemts.FoundPotion:
                case DungeonEvemts.TakenWeapon:
                case DungeonEvemts.TakenProtection:
                case DungeonEvemts.TakenPotion:
                case DungeonEvemts.Macguffin:
                default:
                    break;
            }

            sharedTiles = tiles.SharedModelMapper();

            TilesUpdate.Update(sharedTiles);

            return sharedTiles;
        }

        public async Task<SharedDungeon> MonsterFlee(Guid dungeonId, Guid tileId) {
            await Task.Delay(1);
            return new SharedDungeon();
        }

        public async Task<SharedDungeon> MonsterFight(Guid dungeonId, Guid tileId) {
            await Task.Delay(1);
            return new SharedDungeon();
        }
    }
}
