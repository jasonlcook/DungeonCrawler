﻿using BlazorDungeonCrawler.Server.Models;

using BlazorDungeonCrawler.Database.Resources.Commands.Create;
using BlazorDungeonCrawler.Database.Resources.Commands.Update;

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
            AdventurerCreate.Create(sharedAdventurer);

            //  Level
            int depth = 1;
            Level level = new(depth);
            SharedLevel sharedLevel = level.SharedModelMapper();
            LevelCreate.Create(sharedLevel);

            messages.Add(new Message(3, $"DUNGEON DEPTH {depth}"));

            //  Tiles
            Tiles tiles = new(level.Depth, level.Rows, level.Columns);
            List<SharedTile> sharedTiles = tiles.SharedModelMapper();
            TilesCreate.Create(sharedTiles);

            sharedLevel.Tiles = sharedTiles;

            //  Messages
            List<SharedMessage> sharedMessages = messages.SharedModelMapper();
            MessagesCreate.Create(sharedMessages);

            //  Dungon
            SharedDungeon sharedDungeon = new() {
                ApiVersion = new Version(0, 2, 0).ToString()
            };
            //DungeonCreate.Create(sharedDungeon);

            //Update
            sharedDungeon.Adventurer = sharedAdventurer;
            sharedDungeon.Level = sharedLevel;
            sharedDungeon.Messages = sharedMessages;
            //DungeonUpdate.Update(sharedDungeon);

            return sharedDungeon;
        }

        public async Task<SharedTile> GetSelectedDungeonTile(Guid dungeonId, Guid tileId) {
            await Task.Delay(1);
            return new SharedTile();
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
