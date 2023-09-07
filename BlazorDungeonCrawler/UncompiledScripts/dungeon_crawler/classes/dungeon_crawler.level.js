class Level {
    //todo access class properies via getters
    constructor() {
        this.tiles;

        this.level;
        this.difficulty;

        this.stageRows;
        this.stageCols;

        this.enemies;

        this.InCombat;
    }

    //todo: move to main and set dynamicly
    loadFirstLevel() {
        this.tiles = new Tiles();

        this.level = 1;
        this.difficulty = 1;

        this.stageRows = 9;
        this.stageCols = 7;

        this.availableEnemies = dungeon_crawler.core.enemies.getAvailableEnemies(this.level);

        this.InCombat = false;
    }

    setSpawn() {
        let spawnIndex = Math.floor(Math.random() * this.tiles.length);
        let spawnTitle = this.tiles.get(spawnIndex);

        spawnTitle.Hidden = false;
        spawnTitle.Type = dungeon_crawler.core.globals.tileTypes['entrance'];
        spawnTitle.Current = true;

        this.tiles.currentIndex = spawnIndex;
    }

    getEnemy() {
        let availableEnemyIndex = Math.floor(Math.random() * this.availableEnemies.length);
        return this.availableEnemies[availableEnemyIndex];
    }
}; 