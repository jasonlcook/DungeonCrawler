using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Data {
    public class DungeonManager {

        public async Task<Dungeon> Generate() {
            await Task.Delay(1);

            return new Dungeon() {
                Adventurer = GenerateAdventurer(),
                Level = GenerateLevel()
            };
        }

        public Adventurer GenerateAdventurer() {
            return new Adventurer() {
                HealthBase = 6,
                DamageBase = 6,
                ProtectionBase = 6,
                IsAlive = true
            };
        }

        public Level GenerateLevel() {
            return GenerateLevel(1);
        }

        public Level GenerateLevel(int depth) {
            Level level = new Level() {
                Depth = depth
            };

            switch (depth) {
                case 1:
                    level.Rows = 3;
                    level.Columns = 3;
                    break;
                case 2:
                case 3:
                    level.Rows = 5;
                    level.Columns = 5;
                    break;
                case 4:
                case 5:
                    level.Rows = 7;
                    level.Columns = 7;
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    level.Rows = 7;
                    level.Columns = 9;
                    break;
            }

            return level;
        }
    }
}
