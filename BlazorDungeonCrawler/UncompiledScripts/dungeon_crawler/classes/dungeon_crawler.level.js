﻿class Level {
    constructor() {
        this.tiles;

        this.stageRows;
        this.stageCols;
    }

    loadFirstLevel() {
        this.tiles = new Tiles();

        this.stageRows = 9;
        this.stageCols = 7;
    }

    setSpawn() {
        let spawnIndex = Math.floor(Math.random() * this.tiles.length);
        let spawnTitle = this.tiles.get(spawnIndex);

        spawnTitle.Hidden = false;
        spawnTitle.Type = dungeon_crawler.core.globals.tileTypes['entrance'];
        spawnTitle.Current = true;

        this.tiles.currentIndex = spawnIndex;
    }
}; 