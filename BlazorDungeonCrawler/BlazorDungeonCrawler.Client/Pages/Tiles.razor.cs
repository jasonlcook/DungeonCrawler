using Microsoft.AspNetCore.Components;

using BlazorDungeonCrawler.Shared.Enumerators;

using SharedTile = BlazorDungeonCrawler.Shared.Models.Tile;

namespace BlazorDungeonCrawler.Client.Pages {
    public class TilePosition {
        public int Width { get; set; }
        public int Height { get; set; }

        public TilePosition(int width, int height) {
            Width = width;
            Height = height;
        }

        public string ToTileGroupStyle() {
            return $"top: {Height}px; left: {Width}px;";
        }

        public string ToDungonStyle() {
            int tileHeight = Tiles.TileHeight / 2;
            int tileWidth = Tiles.TileWidth / 2;
            return $"top: -{Height + tileHeight}px; left: -{Width + tileWidth}px;";
        }
    }

    public partial class Tiles {
        [ParameterAttribute]
        public List<SharedTile> DungeonTiles { get; set; }

        [ParameterAttribute]
        public int DungeonDepth { get; set; }

        [ParameterAttribute]
        public bool HighlightAscending { get; set; }

        [Parameter]
        public EventCallback<Guid> OnClickCallback { get; set; }

        public SharedTile currentTile;

        public static readonly int TileHeight = 90;
        public static readonly int TileWidth = 100;
        private readonly int nestedTileWidth = (100 / 4) * 3;

        public Tiles() {
            DungeonTiles = new();

            DungeonDepth = 0;

            HighlightAscending = false;

            OnClickCallback = new();

            currentTile = new();
        }

        private async Task CallParentSelectedTileFunction(Guid tileId) {
            await OnClickCallback.InvokeAsync(tileId);
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

        public TilePosition getTileGroupPosition(int tileRow, int tileColumn) {
            int hexagonLeft = tileColumn * nestedTileWidth;

            int hexagonTop;
            if ((tileColumn % 2) == 1) {
                //long
                hexagonTop = tileRow * TileHeight;
            } else {
                //short
                hexagonTop = (tileRow * TileHeight) + (TileHeight / 2);
            }

            return new TilePosition(hexagonLeft, hexagonTop);
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
                    if (!macGuffinFound) {
                        return "hexagon-tile-stairs-descending-active";
                    } else {
                        return "hexagon-tile-stairs-descending";
                    }
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
                    if (!macGuffinFound) {
                        return "hexagon-tile-macguffin-active";
                    } else {
                        return "hexagon-tile-macguffin";
                    }
                default:
                    return "hexagon-tile-unknown";
            }
        }
    }
}