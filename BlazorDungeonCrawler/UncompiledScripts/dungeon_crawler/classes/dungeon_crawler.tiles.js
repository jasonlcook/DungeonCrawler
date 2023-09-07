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