//**********************************************************************************************************************
//  Tiles
//  A collection of a dungeon's floor tiles

using BlazorDungeonCrawler.Shared.Enumerators;

using SharedTile = BlazorDungeonCrawler.Shared.Models.Tile;
using SharedMonster = BlazorDungeonCrawler.Shared.Models.Monster;

namespace BlazorDungeonCrawler.Server.Models {
    public class Tiles {
        //****************************
        //***************** Attributes
        private List<Tile> _tiles;                          //Collection of tiles

        //****************************
        //*************** Constructors

        public Tiles() {
            this._tiles = new();
        }

        public Tiles(int depth, int floorRows, int floorColumns) {
            this._tiles = new();

            List<int> _tileIndexes = new();

            //      Initiate
            float overflow = (floorColumns + 1) / 2;
            int tileCount = (floorRows * floorColumns) - (int)Math.Ceiling(overflow);

            int row = -1, column = 0, tileType;
            for (int i = 0; i < tileCount; i++) {
                row += 1;
                if ((column % 2) == 1) {
                    //long
                    if (row > floorRows - 1) {
                        row = 0;
                        column += 1;
                    }
                } else {
                    //short
                    if (row > floorRows - 2) {
                        row = 0;
                        column += 1;
                    }
                }

                tileType = Dice.RollDSix();
                this._tiles.Add(new Tile() {
                    Row = row,
                    Column = column,
                    Type = DungeonEvent.GetType(tileType)
                });

                _tileIndexes.Add(i);
            }

            //      Overwrite
            //          Get floor events for depth
            List<DungeonEvents> additionalEvents = new List<DungeonEvents>();
            switch (depth) {
                case 1:
                    additionalEvents.Add(DungeonEvents.DungeonEntrance);
                    additionalEvents.Add(DungeonEvents.StairsDescending);
                    break;
                case 10:
                    additionalEvents.Add(DungeonEvents.StairsAscending);
                    additionalEvents.Add(DungeonEvents.Macguffin);
                    break;
                default:
                    additionalEvents.Add(DungeonEvents.StairsAscending);
                    additionalEvents.Add(DungeonEvents.StairsDescending);
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
                if (randomSelectedTile.Type == DungeonEvents.DungeonEntrance || randomSelectedTile.Type == DungeonEvents.StairsAscending) {
                    randomSelectedTile.Visited = true;
                    randomSelectedTile.Hidden = false;
                    randomSelectedTile.Current = true;

                    SetSelectableTiles(randomSelectedTile.Row, randomSelectedTile.Column);
                }
            }
        }

        //******************** Mapping

        //  DB > Class
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
                    Visited = tile.Visited,
                    Hidden = tile.Hidden,
                    Selectable = tile.Selectable,
                    FightWon = tile.FightWon,
                    Monsters = monsters
                });
            }
        }

        //  Class > DB
        public List<SharedTile> SharedModelMapper() {
            List<SharedTile> sharedTiles = new();

            foreach (var tile in _tiles) {
                sharedTiles.Add(tile.SharedModelMapper());
            }

            return sharedTiles;
        }

        //****************************
        //****************** Accessors

        //********************** Tiles
        //  return count of child elements
        public int Count() {
            return _tiles.Count;
        }

        //  retrieve tiles
        public List<Tile> Get() {
            if (_tiles == null || _tiles.Count == 0) { throw new ArgumentNullException("Dungeon Floor tiles"); }

            return _tiles;
        }

        //  add a child element
        public void Add(Tile tile) {
            _tiles.Add(tile);
        }

        //  update a child element
        public void Update(Tile currentTile) {
            Tile tile;
            for (int i = 0; i < _tiles.Count; i++) {
                tile = _tiles[i];
                if (tile.Id == currentTile.Id) {
                    tile = currentTile;
                }
            }
        }

        //  retrieve selectable tiles
        public List<Tile> GetSelectable() {
            if (_tiles == null || _tiles.Count == 0) { throw new ArgumentNullException("Dungeon Floor tiles"); }

            return _tiles
                .Where(t => t.Selectable == true)
                .ToList();
        }

        //  retrieve hidden selectable tiles
        public List<Tile> GetHiddenSelectable() {
            if (_tiles == null || _tiles.Count == 0) { throw new ArgumentNullException("Dungeon Floor tiles"); }

            return _tiles.Where(t => t.Selectable == true).Where(t => t.Hidden == true).ToList();
        }

        //  retrieve selectable tiles except descending stairs
        public List<Tile> GetSelectableCurrentFloor() {
            if (_tiles == null || _tiles.Count == 0) { throw new ArgumentNullException("Dungeon Floor tiles"); }

            return _tiles.Where(t => t.Selectable == true)
                .Where(t => t.Type != DungeonEvents.StairsDescending)
                .ToList();
        }

        //  retrieve selectable tiles except stairs and entrance
        public List<Tile> GetSelectableUnhighlightable() {
            if (_tiles == null || _tiles.Count == 0) { throw new ArgumentNullException("Dungeon Floor tiles"); }

            return _tiles
                .Where(t => t.Selectable == true)
                .Where(t => t.Type != DungeonEvents.StairsDescending)
                .Where(t => t.Type != DungeonEvents.StairsAscending)
                .Where(t => t.Type != DungeonEvents.DungeonEntrance)
                .ToList();
        }

        //  retrieve hidden tiles
        public List<Tile> GetHidden() {
            if (_tiles == null || _tiles.Count == 0) { throw new ArgumentNullException("Dungeon Floor tiles"); }

            return _tiles.Where(t => t.Hidden == true).ToList();
        }

        //*********************** Tile

        //  retrieve a tile
        public Tile Get(Guid tileId) {
            if (_tiles == null || _tiles.Count == 0) { throw new ArgumentNullException("Dungeon Floor tiles"); }

            return _tiles.Where(t => t.Id == tileId).FirstOrDefault();
        }

        //  retrieve the current tile
        public Tile GetCurrent() {
            if (_tiles == null || _tiles.Count == 0) { throw new ArgumentNullException("Dungeon Floor tiles"); }

            return _tiles.FirstOrDefault(t => t.Current);
        }

        //  retrieve the dungeon entrance tile
        public Tile GetDungeonEntrance() {
            if (_tiles == null || _tiles.Count == 0) { throw new ArgumentNullException("Dungeon Floor tiles"); }

            return _tiles.FirstOrDefault(t => t.Type == DungeonEvents.DungeonEntrance);
        }

        //  retrieve the ascending stairs tile
        public Tile GetStairsAscending() {
            if (_tiles == null || _tiles.Count == 0) { throw new ArgumentNullException("Dungeon Floor tiles"); }

            return _tiles.FirstOrDefault(t => t.Type == DungeonEvents.StairsAscending);
        }

        //  retrieve the descending stairs tile
        public Tile GetStairsDescending() {
            if (_tiles == null || _tiles.Count == 0) { throw new ArgumentNullException("Dungeon Floor tiles"); }

            return _tiles.FirstOrDefault(t => t.Type == DungeonEvents.StairsDescending);
        }

        //  retrieve the macguffin tile
        public Tile GetMacguffin() {
            if (_tiles == null || _tiles.Count == 0) { throw new ArgumentNullException("Dungeon Floor tiles"); }

            return _tiles.FirstOrDefault(t => t.Type == DungeonEvents.Macguffin);
        }

        //****************************
        //****************** Operation

        //  Unhide all tiles
        public void Unhide() {
            foreach (Tile tile in _tiles) {
                tile.Hidden = false;
            }
        }

        //  Set the tiles surrounding the adventurers current location
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
    }
}
