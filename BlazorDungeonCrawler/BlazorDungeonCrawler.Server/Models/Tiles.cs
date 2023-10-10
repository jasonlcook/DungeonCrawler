using BlazorDungeonCrawler.Database.Resources.Queries.Get;
using BlazorDungeonCrawler.Shared.Enumerators;

using SharedTile = BlazorDungeonCrawler.Shared.Models.Tile;
using SharedMonster = BlazorDungeonCrawler.Shared.Models.Monster;

namespace BlazorDungeonCrawler.Server.Models {
    public class Tiles {
        private List<Tile> _tiles;

        public List<Tile> GetTiles() {
            return _tiles;
        }

        public Tiles(List<SharedTile> tiles) {
            _tiles = new();

            Monster monster = new Monster();
            foreach (SharedTile tile in tiles) {
                List<Monster> monsters = new();

                if (tile.Monsters != null && tile.Monsters.Count > 0) {
                    foreach (SharedMonster sharedMonster in tile.Monsters) {
                        monsters.Add(monster.ServerModelMapper(sharedMonster));
                    }
                }

                _tiles.Add(new Tile() {
                    Id = tile.Id,
                    Row = tile.Row,
                    Column = tile.Column,
                    Type = tile.Type,
                    Current = tile.Current,
                    Hidden = tile.Hidden,
                    Selectable = tile.Selectable,
                    FightWon = tile.FightWon,
                    Monsters = monsters
                });
            }
        }

        public Tiles(int depth, int levelRows, int levelColumns) {
            _tiles = new();

            List<int> _tileIndexes = new();

            //      Initiate
            float overflow = (levelColumns + 1) / 2;
            int tileCount = (levelRows * levelColumns) - (int)Math.Ceiling(overflow);

            int row = -1, column = 0, tileType;
            for (int i = 0; i < tileCount; i++) {
                row += 1;
                if ((column % 2) == 1) {
                    //long
                    if (row > levelRows - 1) {
                        row = 0;
                        column += 1;
                    }
                } else {
                    //short
                    if (row > levelRows - 2) {
                        row = 0;
                        column += 1;
                    }
                }

                tileType = Dice.RollDSix();
                _tiles.Add(new Tile() {
                    Row = row,
                    Column = column,
                    Type = DungeonEvemt.GetType(tileType)
                });

                _tileIndexes.Add(i);
            }

            //      Overwrite
            //          Get level events for depth
            List<DungeonEvemts> additionalEvents = new List<DungeonEvemts>();
            switch (depth) {
                case 1:
                    additionalEvents.Add(DungeonEvemts.DungeonEntrance);
                    additionalEvents.Add(DungeonEvemts.StairsDescending);
                    break;
                case 10:
                    additionalEvents.Add(DungeonEvemts.StairsAscending);
                    additionalEvents.Add(DungeonEvemts.Macguffin);
                    break;
                default:
                    additionalEvents.Add(DungeonEvemts.StairsAscending);
                    additionalEvents.Add(DungeonEvemts.StairsDescending);
                    break;
            }

            Tile randomSelectedTile;
            int avalibleTileIndex;
            for (int i = 0; i < additionalEvents.Count; i++) {
                //get random tile 
                avalibleTileIndex = Dice.RandomNumber(0, _tileIndexes.Count - 1);
                randomSelectedTile = _tiles.ElementAt(_tileIndexes.ElementAt(avalibleTileIndex));

                //once selected remove from the avalible list
                _tileIndexes.RemoveAt(avalibleTileIndex);

                //retrive first event
                randomSelectedTile.Type = additionalEvents.ElementAt(i);

                //if it is a starting tile set it as current un visible
                if (randomSelectedTile.Type == DungeonEvemts.DungeonEntrance || randomSelectedTile.Type == DungeonEvemts.StairsAscending) {
                    randomSelectedTile.Hidden = false;
                    randomSelectedTile.Current = true;

                    SetSelectableTiles(randomSelectedTile.Row, randomSelectedTile.Column);
                }
            }
        }

        public void SetSelectableTiles(int currentRow, int currentColumn) {
            int previousTileRow, currentTileRow, nextTileRow, previousTileColumn, currentTileColumn, nextTileColumn;
            foreach (Tile tile in _tiles) {
                tile.Selectable = false;

                currentTileRow = tile.Row;
                previousTileRow = currentTileRow - 1;
                nextTileRow = currentTileRow + 1;

                currentTileColumn = tile.Column;
                previousTileColumn = currentTileColumn - 1;
                nextTileColumn = currentTileColumn + 1;

                if ((currentTileColumn % 2) == 1) {
                    //previous and next column
                    if (previousTileColumn == currentColumn || nextTileColumn == currentColumn) {
                        if (previousTileRow == currentRow || currentTileRow == currentRow) {
                            tile.Selectable = true;
                        }
                    }

                    //curent column
                    if (currentTileColumn == currentColumn) {
                        if (previousTileRow == currentRow || nextTileRow == currentRow) {
                            tile.Selectable = true;
                        }
                    }
                } else {
                    //previous and next column
                    if (previousTileColumn == currentColumn || nextTileColumn == currentColumn) {
                        if (currentTileRow == currentRow || nextTileRow == currentRow) {
                            tile.Selectable = true;
                        }
                    }

                    //curent column
                    if (currentTileColumn == currentColumn) {
                        if (previousTileRow == currentRow || nextTileRow == currentRow) {
                            tile.Selectable = true;
                        }
                    }
                }
            }
        }

        public List<SharedTile> SharedModelMapper() {
            List<SharedTile> sharedTiles = new();

            foreach (var tile in _tiles) {
                sharedTiles.Add(tile.SharedModelMapper());
            }

            return sharedTiles;
        }

        public void Unhide() {
            foreach (Tile tile in _tiles) {
                tile.Hidden = false;
            }
        }
    }
}
