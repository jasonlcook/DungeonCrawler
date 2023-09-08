class Level {
    //todo access class properies via getters
    constructor() {
        this.tiles;

        this.level;

        this.stageRows;
        this.stageCols;

        this.enemies;

        this.stairsDeployed;
    }

    loadLevel(value) {
        this.tiles = new Tiles();

        this.level = value;

        switch (value) {
            case 1:
                this.stageRows = 3;
                this.stageCols = 3;
                break;
            case 2:
                this.stageRows = 4;
                this.stageCols = 5;
            case 3:
                this.stageRows = 6;
                this.stageCols = 7;
                break;
            case 4:
                this.stageRows = 8;
                this.stageCols = 9;
                break;
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
                this.stageRows = 9;
                this.stageCols = 11;
                break;
            case 10:
                this.stageRows = 10;
                this.stageCols = 13;
                break;
            default:
        }

        this.availableEnemies = dungeon_crawler.core.enemies.getAvailableEnemies(this.level);

        this.stairsDeployed = false;
    }

    setSpawn(level) {
        let spawnIndex = Math.floor(Math.random() * this.tiles.length);
        let spawnTitle = this.tiles.get(spawnIndex);

        spawnTitle.Hidden = false;

        if (level > 1) {
            spawnTitle.Type = dungeon_crawler.core.globals.tileTypes['stairs_ascending'];
        } else {
            spawnTitle.Type = dungeon_crawler.core.globals.tileTypes['entrance'];
        }

        spawnTitle.Current = true;

        this.tiles.currentIndex = spawnIndex;
    }

    getEnemy() {
        let availableEnemyIndex = Math.floor(Math.random() * this.availableEnemies.length);
        return this.availableEnemies[availableEnemyIndex];
    }
}; 