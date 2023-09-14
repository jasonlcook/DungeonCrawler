class Level {
    //todo access class properies via getters
    constructor(value) {
        this._level = value;
         
        let rows = 0;
        let cols = 0;

        switch (value) {
            case 1:
                rows = 3;
                cols = 3;
                break;
            case 2:
            case 3:
                rows = 5;
                cols = 5;
                break;
            case 4:
            case 5:
                rows = 7;
                cols = 7;
                break;
            case 7:
            case 8:
                rows = 9;
                cols = 7;
            case 9:
            case 10:
                rows = 11;
                cols = 7;
                break;
            default:
        }

        this._stageRows = rows;
        this._stageCols = cols;



        this._spawnLocationRow;
        this._spawnLocationColumn;

        if (this._stageRows == 0 || this._stageCols == 0) {
            dungeon_crawler.core.outputError(`No rows (${this._stageRows}) or columns (${this._stageCols}) for level ${value}.`);
        }

        this._availableEnemies = dungeon_crawler.core.enemies.getAvailableEnemies(this._level);

        this._endLevelTileDeployed = false;

        this._tiles = this.generateTiles();        

        //set spawn location
        this.setSpawn();
    }

    //Tiles
    //  Add
    getTiles() {
        return this._tiles;
    }

    //  Get by id
    getTileById(tileId) {
        return this._tiles.getById(tileId);
    }

    //  Set selectable
    setSelectableTiles() {
        this._tiles.setSelectables()
    }

    //  Set movement
    tilesMovement(selectedTile) {
        this._tiles.movement(selectedTile);
    }

    //Level
    //  Get
    getLevel() {
        return this._level;
    }

    //  Next
    getNextlevel() {
        return this._level + 1;
    }

    //  Previous
    getPreviouslevel() {
        return this._level - 1;
    }

    //StageRows
    getStageRows() {
        return this._stageRows;
    }

    //StageCols
    getStageCols() {
        return this._stageCols;
    }

    //End level tile deployed
    //  Set
    setsEndLevelTileAsDeployed() {
        this._endLevelTileDeployed = true;
    }

    //  Get
    isEndLevelTileDeployed() {
        return this._endLevelTileDeployed;
    }

    //Coordinates
    getSpawnCoordinates() {
        return { 'row': this._spawnLocationRow, 'column': this._spawnLocationColumn };
    }

    //Set spawn
    //  Get a random tile and set it as the current location
    setSpawn() {
        let spawnIndex = Math.floor(Math.random() * this._tiles.length);
        let spawnTile = this._tiles.get(spawnIndex);

        if (typeof spawnTile != 'undefined' && spawnTile != null) {
            spawnTile.setHidden(false);

            if (this.getLevel() > 1) {
                spawnTile.setType(dungeon_crawler.core.globals.tileTypes['stairs_ascending']);
            } else {
                spawnTile.setType(dungeon_crawler.core.globals.tileTypes['entrance']);
            }

            this._spawnLocationRow = spawnTile.getRow();
            this._spawnLocationColumn = spawnTile.getColumn();

            spawnTile.setCurrent(true);

            this._tiles.setCurrentIndex(spawnIndex);
        } else {
            dungeon_crawler.core.outputError(`Generate spawn tile with value ${spawnTile}`);
        }
    }

    generateTiles() {
        let hexagonHeight = dungeon_crawler.core.globals.hexHeight;
        let hexagonWidth = dungeon_crawler.core.globals.hexWidth;

        //set stage dimentions
        //  height
        let stageHeight = this.getStageCols() * hexagonHeight;
        dungeon_crawler.core.globals.stageHeight = stageHeight;

        //  width
        let hexWidthQuaters = hexagonWidth / 4;
        let stageWidth = this.getStageRows() * (hexWidthQuaters * 3) + hexWidthQuaters;
        dungeon_crawler.core.globals.stageWidth = stageWidth;

        //set board
        let hexagonLeft = 0, hexagonTop = 0, hexRow = -1, hexColumn = 0;

        //  due to the orientation of our board we miss one hex for every other grid row
        let tileCount = (this.getStageCols() * this.getStageRows()) - Math.ceil((this.getStageRows() + 1) / 2);

        hexagonTop -= hexagonHeight / 2;

        let tile, tiles = new Tiles();
        for (var i = 0; i < tileCount; i++) {
            hexagonTop += hexagonHeight;

            if (hexagonTop >= stageHeight - (hexagonWidth / 2)) {
                //move tile along one place
                hexagonLeft += (hexagonWidth / 4) * 3;

                //reset top
                if ((hexColumn % 2) == 1) {
                    //long
                    hexagonTop = hexagonHeight - (hexagonHeight / 2);
                } else {
                    //short
                    hexagonTop = 0;
                }

                hexRow = 0;

                //add column
                hexColumn += 1;
            } else {
                hexRow += 1;
            }

            tile = new Tile(i, dungeon_crawler.core.globals.tileTypes['unknown'], hexRow, hexColumn, hexagonLeft, hexagonTop)
            tiles.add(tile);
        }

        return tiles;
    }

    getEnemy() {
        let availableEnemyIndex = Math.floor(Math.random() * this._availableEnemies.length);
        return this._availableEnemies[availableEnemyIndex];
    }
}; 