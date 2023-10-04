using BlazorDungeonCrawler.Shared.Enumerators;

namespace BlazorDungeonCrawler.Server.Models {
    public static class DungeonEvemt {
        public static DungeonEvemts GetType(int value) {
            DungeonEvemts dungeonEvemts = DungeonEvemts.Unknown;

            switch (value) {
                case 1:
                case 2:
                    dungeonEvemts = DungeonEvemts.Fight;
                    break;
                case 3:
                case 4:
                case 5:
                    dungeonEvemts = DungeonEvemts.Empty;
                    break;
                case 6:
                    dungeonEvemts = DungeonEvemts.Chest;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("DungeonEvemts");
            }

            return dungeonEvemts;
        }
    }
}
