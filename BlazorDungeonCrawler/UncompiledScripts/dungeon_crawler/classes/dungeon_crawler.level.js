class Level {
    //todo access class properies via getters
    constructor() {
        this._tiles;

        this._level;

        this._stageRows;
        this._stageCols;

        this._availableEnemies;

        this._endLevelTileDeployed;
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

    //Tile
    addTile(tile) {
        this._tiles.add(tile);
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

    //Methods
    loadLevel(value) {
        this._tiles = new Tiles();

        this._level = value;

        switch (value) {
            case 1:
                this._stageRows = 3;
                this._stageCols = 3;
                break;
            case 2:
                this._stageRows = 4;
                this._stageCols = 4;
            case 3:
            case 4:
                this._stageRows = 5;
                this._stageCols = 5;
                break;
            case 5:
                this._stageRows = 6;
                this._stageCols = 6;
                break;
            case 6:
                this._stageRows = 7;
                this._stageCols = 7;
                break;
            case 7:
                this._stageRows = 8;
                this._stageCols = 7;
                break;
            case 8:
                this._stageRows = 9;
                this._stageCols = 7;
            case 9:
            case 10:
                this._stageRows = 10;
                this._stageCols = 7;
                break;
            default:
        }

        this._availableEnemies = dungeon_crawler.core.enemies.getAvailableEnemies(this._level);

        this._endLevelTileDeployed = false;
    }

    setSpawn(level) {
        let spawnIndex = Math.floor(Math.random() * this._tiles.length);
        let spawnTitle = this._tiles.get(spawnIndex);

        spawnTitle.setHidden(false);

        if (level > 1) {
            spawnTitle.setType(dungeon_crawler.core.globals.tileTypes['stairs_ascending']);
        } else {
            spawnTitle.setType(dungeon_crawler.core.globals.tileTypes['entrance']);
        }

        spawnTitle.setCurrent(true);

        this._tiles.setCurrentIndex(spawnIndex) ;
    }

    getEnemy() {
        let availableEnemyIndex = Math.floor(Math.random() * this._availableEnemies.length);
        return this._availableEnemies[availableEnemyIndex];
    }
}; 