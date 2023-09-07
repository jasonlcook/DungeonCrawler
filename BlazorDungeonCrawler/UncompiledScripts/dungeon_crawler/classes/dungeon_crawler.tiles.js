class Tiles {
    constructor() {
        this.tiles = [];
        this.currentIndex;
    }

    get length() {
        return this.tiles.length;
    }

    add(tile) {
        this.tiles.push(tile);
    }

    get(index) {
        if (index < this.tiles.length) {
            return this.tiles[index];
        } else {
            console.log(`Index "${index}" not found`);
        }
    }

    getById(id) {
        let tile;
        for (var i = 0; i < this.tiles.length; i++) {
            tile = this.tiles[i];

            if (tile.Id == id) {
                return tile;
            }           
        }
    }

    //Tile select
    //  1 - 3:  Monster
    //  4, 5:   Empty
    //  6:      Loot
    getNextTileType() {
        //roll to see if tile populated
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
            case 2:
            case 3:
                dungeon_crawler.core.globals.currentLevel.InCombat = true;
                return dungeon_crawler.core.globals.tileTypes['fight'];
                break;
            case 6:
                return dungeon_crawler.main.selectLoot();
                break;
            case 4:
            case 5:
                return dungeon_crawler.core.globals.tileTypes['empty'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected tile table role "${value}"`);
        return dungeon_crawler.core.globals.tileTypes['unknown'];
    }

    //Tile select
    //  1, 2:   Monster
    //  3, 6:   No change
    getRepeatTileType() {
        //roll to see if tile populated
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
            case 2:
                dungeon_crawler.core.globals.currentLevel.InCombat = true;
                return dungeon_crawler.core.globals.tileTypes['fight'];
                break;
            case 3:
            case 4:
            case 5:
            case 6:
                return null;
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected tile table role "${value}"`);
        return dungeon_crawler.core.globals.tileTypes['unknown'];
    }

    setSelectables() {
        let current = this.tiles[this.currentIndex];
        let currentRow = current.Row;
        let currentColumn = current.Column;

        let tile, previousTileRow, currentTileRow, nextTileRow, previousTileColumn, currentTileColumn, nextTileColumn;

        for (var i = 0; i < this.tiles.length; i++) {
            tile = this.tiles[i];
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

    unsetSelectables() {
        let tile;
        for (var i = 0; i < this.tiles.length; i++) {
            tile = this.tiles[i];
            tile.Selectable = false;
        }
    }
}; 