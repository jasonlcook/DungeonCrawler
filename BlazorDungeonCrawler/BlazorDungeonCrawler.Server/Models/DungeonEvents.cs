//**********************************************************************************************************************
//  DungeonEvent
//  Return the dungon tile tile from a simulated D6 dice roll

using BlazorDungeonCrawler.Shared.Enumerators;

namespace BlazorDungeonCrawler.Server.Models {
    public static class DungeonEvent {
        public static DungeonEvents GetType(int value) {
            switch (value) {
                case 1:
                case 2:
                    return DungeonEvents.Fight;
                case 3:
                case 4:
                case 5:
                    return DungeonEvents.Empty;
                case 6:
                    return DungeonEvents.Chest;
                default:
                    throw new ArgumentOutOfRangeException("DungeonEvemts");
            }
        }
    }
}
