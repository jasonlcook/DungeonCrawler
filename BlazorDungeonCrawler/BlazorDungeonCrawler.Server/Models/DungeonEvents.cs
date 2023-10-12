using BlazorDungeonCrawler.Shared.Enumerators;

namespace BlazorDungeonCrawler.Server.Models {
    public static class DungeonEvent {
        public static DungeonEvemts GetType(int value) {
            switch (value) {
                case 1:
                case 2:
                    return DungeonEvemts.Fight;
                case 3:
                case 4:
                case 5:
                    return DungeonEvemts.Empty;
                case 6:
                    return DungeonEvemts.Chest;
                default:
                    throw new ArgumentOutOfRangeException("DungeonEvemts");
            }
        }
    }
}
