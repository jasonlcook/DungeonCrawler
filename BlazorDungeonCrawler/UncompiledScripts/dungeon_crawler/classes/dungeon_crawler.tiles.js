class Tiles {
    constructor() {
        this.tiles = [];

        this.explored = 0;

        this.currentIndex;
    }

    get length() {
        return this.tiles.length;
    }

    //element accessors 
    add(tile) {
        this.tiles.push(tile);
    }

    get(index) {
        if (index < this.tiles.length) {
            return this.tiles[index];
        }

        dungeon_crawler.core.outputError(`Index "${index}" not found`);
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

    movement(selectedTile) {
        //Potion effect
        //  Both attempt to decorment and retun true if it was needed
        if (dungeon_crawler.core.globals.adventurer.decrementAuraPotionDuration()) {
            dungeon_crawler.main.updateAdventurerHealth();
        }

        if (dungeon_crawler.core.globals.adventurer.decrementDamagePotionDuration()) {
            dungeon_crawler.main.updateAdventurerDamage();
        }

        if (dungeon_crawler.core.globals.adventurer.decrementShieldPotionDuration()) {
            dungeon_crawler.main.updateAdventurerProtection();
        }

        dungeon_crawler.core.globals.InCombat = false;

        dungeon_crawler.main.resetDiceValues();

        //deselect previous tile
        let previousIndex = this.currentIndex;
        let previousTile = this.get(previousIndex);
        previousTile.Current = false;

        this.currentIndex = selectedTile.Index;
        selectedTile.Current = true;
        selectedTile.Hidden = false;

        let selectedTileType = selectedTile.Type;

        if (selectedTileType == dungeon_crawler.core.globals.tileTypes['unknown']) {

            this.explored += 1;

            let nextTileType = this.checkEndLevelTileDeployed();
            if (nextTileType == null) {
                nextTileType = this.selectTileType();
            }

            switch (nextTileType) {
                case dungeon_crawler.core.globals.tileTypes['macguffin']:
                    dungeon_crawler.core.globals.macguffinFound = true;
                    dungeon_crawler.main.macGuffinText();
                    break;
                case dungeon_crawler.core.globals.tileTypes['chest']:

                    //todo: pause game on chest
                    nextTileType = dungeon_crawler.main.selectLoot()

                    switch (nextTileType) {
                        case dungeon_crawler.core.globals.tileTypes['potion']:
                            //todo: pause game on potion
                            dungeon_crawler.potion.getPotion();
                            break;
                        case dungeon_crawler.core.globals.tileTypes['protection']:
                            //todo: pause game on protection
                            dungeon_crawler.armour.getArmour();
                            break;
                        case dungeon_crawler.core.globals.tileTypes['weapon']:
                            //todo: pause game on protection
                            dungeon_crawler.weapon.getWeapon();
                            break;
                    }

                    break;
            }

            selectedTile.Type = nextTileType;
        } else {
            //reentering a tile
            if (selectedTileType == dungeon_crawler.core.globals.tileTypes['entrance']) {
                if (!dungeon_crawler.core.globals.macguffinFound) {
                    dungeon_crawler.main.exitWithoutMacGuffinText();
                } else {
                    dungeon_crawler.main.endGamge();
                    dungeon_crawler.main.exitWithMacGuffinText();
                    return;
                }
            } else if (selectedTileType == dungeon_crawler.core.globals.tileTypes['stairs_ascending']) {
                let previouslevel = dungeon_crawler.core.globals.currentLevel.getPreviouslevel(); 

                dungeon_crawler.main.generateLevel(previouslevel);

                dungeon_crawler.main.stairsUpText(previouslevel);
            } else if (selectedTileType == dungeon_crawler.core.globals.tileTypes['empty'] || selectedTileType == dungeon_crawler.core.globals.tileTypes['fight']) {
                //if tile has already been placed roll for monster encounter
                let repeatTile = this.getRepeatTileType();

                if (repeatTile != null) {
                    selectedTile.Type = repeatTile;
                }
            }
        }

        if (selectedTile.Type == dungeon_crawler.core.globals.tileTypes['stairs_descending']) {
            
            let nextlevel = dungeon_crawler.core.globals.currentLevel.getNextlevel();

            dungeon_crawler.main.generateLevel(nextlevel);
            dungeon_crawler.main.stairsDownText(nextlevel);
        }

        this.setSelectables();
        dungeon_crawler.main.setStage();

        dungeon_crawler.core.globals.eventBindings.unbindEvents();
        dungeon_crawler.core.globals.eventBindings.clearBoundEvents();

        dungeon_crawler.main.bindEvents();
    }

    checkEndLevelTileDeployed() {
        if (!dungeon_crawler.core.globals.currentLevel.isEndLevelTileDeployed()) {
            let quadsExplored = Math.floor(this.tiles.length / 4);

            if (this.explored > quadsExplored) {
                // 50/50 change of stairs being deplyed or if last tile then deply it
                if ((Math.floor(Math.random() * 2) == 0) || this.explored >= this.tiles.length) {
                    dungeon_crawler.core.globals.currentLevel.setsEndLevelTileAsDeployed();

                    if (dungeon_crawler.core.globals.currentLevel.getLevel() < dungeon_crawler.core.globals.lastLevel) {
                        return dungeon_crawler.core.globals.tileTypes['stairs_descending'];
                    } else {
                        return dungeon_crawler.core.globals.tileTypes['macguffin'];
                    }
                }
            }
        }

        return null;
    }

    //Dice role
    //  1 - 2:  Monster
    //  2 - 5:  Empty
    //  6:      Chest
    selectTileType() {
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
            case 2:
                dungeon_crawler.core.globals.InCombat = true;
                return dungeon_crawler.core.globals.tileTypes['fight'];
                break;
            case 3:
            case 4:
            case 5:
                return dungeon_crawler.core.globals.tileTypes['empty'];
                break;
            case 6:
                return dungeon_crawler.core.globals.tileTypes['chest'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected tile table role "${value}"`);
        return dungeon_crawler.core.globals.tileTypes['unknown'];
    }

    //  1, 2:   Monster
    //  3, 6:   No change
    getRepeatTileType() {
        //roll to see if tile populated
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
            case 2:
                dungeon_crawler.core.globals.InCombat = true;
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