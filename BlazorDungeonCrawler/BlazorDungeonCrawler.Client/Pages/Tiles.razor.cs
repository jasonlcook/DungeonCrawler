using BlazorDungeonCrawler.Shared.Enumerators;
using Microsoft.AspNetCore.Components;

using SharedTile = BlazorDungeonCrawler.Shared.Models.Tile;

namespace BlazorDungeonCrawler.Client.Pages {
    public partial class Tiles {
        [ParameterAttribute]
        public List<SharedTile> DungeonTiles { get; set; }

        [ParameterAttribute]
        public int DungeonDepth { get; set; }

        [ParameterAttribute]
        public int TileRows { get; set; }

        [ParameterAttribute]
        public int TileColumns { get; set; }

        [ParameterAttribute]
        public bool MacGuffinFound { get; set; }

        [Parameter]
        public EventCallback<Guid> OnClickCallback { get; set; }

        private readonly int tileWidth = 100;
        private readonly int nestedTileWidth = (100 / 4) * 3;
        private readonly int tileHeight = 90;

        public Tiles() {
            DungeonTiles = new();
            DungeonDepth = 0;
        }

        private async Task CallParentSelectedTileFunction(Guid tileId) {
            await OnClickCallback.InvokeAsync(tileId);
        }

        public string getDungeonPosition(int tileRows, int tileColumns) {
            int dungeonWidth = nestedTileWidth * tileRows + (tileWidth / 4);
            int dungeonHeight = tileHeight * tileColumns;

            return $"width: {dungeonWidth}px; height: {dungeonHeight}px;";
        }

        public string getColourClass(int dungeonDepth) {
            switch (dungeonDepth) {
                case 1:
                case 2:
                case 3:
                    return "hexagon-colour-red";
                case 4:
                case 5:
                case 6:
                case 7:
                    return "hexagon-colour-blue";
                case 8:
                    return "hexagon-colour-purple";
                case 9:
                    return "hexagon-colour-green";
                case 10:
                    return "hexagon-colour-pink";
                default:
                    return "hexagon-colour-unknown";
            }
        }

        public string getTileGroupPosition(int tileRow, int tileColumn) {
            int hexagonLeft = tileColumn * nestedTileWidth;

            int hexagonTop;
            if ((tileColumn % 2) == 1) {
                //long
                hexagonTop = tileRow * tileHeight;
            } else {
                //short
                hexagonTop = (tileRow * tileHeight) + (tileHeight / 2);
            }

            return $"top: {hexagonTop}px; left: {hexagonLeft}px;";
        }

        public string getTypeClass(bool macGuffinFound, DungeonEvents type) {
            switch (type) {
                case DungeonEvents.Empty:
                    return "hexagon-tile-empty";
                case DungeonEvents.DungeonEntrance:
                    if (macGuffinFound) {
                        return "hexagon-tile-entrance-active";
                    } else {
                        return "hexagon-tile-entrance";
                    }
                case DungeonEvents.StairsAscending:
                    if (macGuffinFound) {
                        return "hexagon-tile-stairs-ascending-active";
                    } else {
                        return "hexagon-tile-stairs-ascending";
                    }
                case DungeonEvents.StairsDescending:
                    return "hexagon-tile-stairs-descending";
                case DungeonEvents.Fight:
                    return "hexagon-tile-fight";
                case DungeonEvents.FightWon:
                    return "hexagon-tile-fight-won";
                case DungeonEvents.FightLost:
                    return "hexagon-tile-adventurer-death";
                case DungeonEvents.Chest:
                    return "hexagon-tile-chest";
                case DungeonEvents.FoundWeapon:
                    return "hexagon-tile-weapon";
                case DungeonEvents.FoundProtection:
                    return "hexagon-tile-protection";
                case DungeonEvents.FoundPotion:
                    return "hexagon-tile-potion";
                case DungeonEvents.TakenWeapon:
                    return "hexagon-tile-weapon";
                case DungeonEvents.TakenProtection:
                    return "hexagon-tile-protection";
                case DungeonEvents.TakenPotion:
                    return "hexagon-tile-potion";
                case DungeonEvents.Macguffin:
                    return "hexagon-tile-macguffin";
                default:
                    return "hexagon-tile-unknown";
            }
        }
    }
}