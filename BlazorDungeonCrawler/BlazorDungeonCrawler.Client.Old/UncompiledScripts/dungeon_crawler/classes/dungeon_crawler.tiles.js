class Tiles {
    constructor() {
        this._tiles = [];
        this._explored = 0;
        this._currentIndex;
    }

    get length() {
        return this._tiles.length;
    }

    //element accessors 
    add(tile) {
        this._tiles.push(tile);
    }

    get(index) {
        if (index < this._tiles.length) {
            return this._tiles[index];
        }

        dungeon_crawler.core.outputError(`Index "${index}" not found`);
    }

    getById(id) {
        let tile;
        for (var i = 0; i < this._tiles.length; i++) {
            tile = this._tiles[i];

            if (tile.getId() == id) {
                return tile;
            }
        }
    }

    getCurrentIndex() {
        return this._currentIndex;
    }

    setCurrentIndex(value) {
        this._currentIndex = value;
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
        let previousIndex = this._currentIndex;
        let previousTile = this.get(previousIndex);
        previousTile.setCurrent(false);

        this._currentIndex = selectedTile.getIndex();
        selectedTile.setCurrent(true);
        selectedTile.setHidden(false);

        let selectedTileType = selectedTile.getType();

        if (selectedTileType == dungeon_crawler.core.globals.tileTypes['unknown']) {

            this._explored += 1;

            let nextTileType = this.checkEndLevelTileDeployed();
            if (nextTileType == null) {
                let tileTypeValue = dungeon_crawler.main.rollDSix();
                nextTileType = this.selectTileType(tileTypeValue);

                //let logEntry = new LogEntry(dungeon_crawler.log_text.generateTileText(nextTileType));
                //logEntry.addLogAction(new LogAction(0, `Tile type "${nextTileType}" (${tileTypeValue})`, [tileTypeValue]));

                //dungeon_crawler.core.globals.logs.addEntry(logEntry);
            }

            switch (nextTileType) {
                case dungeon_crawler.core.globals.tileTypes['macguffin']:
                    dungeon_crawler.core.globals.macguffinFound = true;
                    dungeon_crawler.core.globals.logs.addEntry(new LogEntry(dungeon_crawler.log_text.generateMacGuffinText()));
                    break;
                case dungeon_crawler.core.globals.tileTypes['chest']:

                    let lootValue = dungeon_crawler.main.rollDSix();
                    nextTileType = dungeon_crawler.main.selectLoot(lootValue);
                    //let logEntry;
                    switch (nextTileType) {
                        case dungeon_crawler.core.globals.tileTypes['potion']:
                            //logEntry = new LogEntry(dungeon_crawler.log_text.generateLootPotionText());
                            dungeon_crawler.potion.getPotion();
                            break;
                        //todo: update tile from protection to armour
                        case dungeon_crawler.core.globals.tileTypes['protection']:
                            //logEntry = new LogEntry(dungeon_crawler.log_text.generateLootArmourText());
                            dungeon_crawler.armour.getArmour();
                            break;
                        case dungeon_crawler.core.globals.tileTypes['weapon']:
                            //logEntry = new LogEntry(dungeon_crawler.log_text.generateLootWeaponText());
                            dungeon_crawler.weapon.getWeapon();
                            break;
                    }

                    //logEntry.addLogAction(new LogAction(0, `You find "${nextTileType}" (${lootValue})`, [lootValue]));

                    //dungeon_crawler.core.globals.logs.addEntry(logEntry);

                    break;
            }

            selectedTile.setType(nextTileType);
        } else {
            //reentering a tile
            if (selectedTileType == dungeon_crawler.core.globals.tileTypes['entrance']) {
                if (!dungeon_crawler.core.globals.macguffinFound) {
                    dungeon_crawler.core.globals.logs.addEntry(new LogEntry(dungeon_crawler.log_text.generateExitWithoutMacGuffinText()));
                } else {
                    dungeon_crawler.core.globals.logs.addEntry(new LogEntry(dungeon_crawler.log_text.generateExitWithMacGuffinText()));
                    dungeon_crawler.main.endGamge();
                    return;
                }
            } else if (selectedTileType == dungeon_crawler.core.globals.tileTypes['stairs_ascending']) {
                let previouslevel = dungeon_crawler.core.globals.currentLevel.getPreviouslevel();

                dungeon_crawler.main.generateLevel(previouslevel);
                dungeon_crawler.core.globals.logs.addEntry(new LogEntry(dungeon_crawler.log_text.generateStairsUpText(previouslevel)));

            } else if (selectedTileType == dungeon_crawler.core.globals.tileTypes['empty'] || selectedTileType == dungeon_crawler.core.globals.tileTypes['fight']) {
                //if tile has already been placed roll for monster encounter
                let repeatTileTypeValue = dungeon_crawler.main.rollDSix();
                let repeatTile = this.getRepeatTileType(repeatTileTypeValue);

                if (repeatTile != null) {
                    selectedTile.setType(repeatTile);
                }

                //let logEntry = new LogEntry(dungeon_crawler.log_text.generateTileText(repeatTile));
                //logEntry.addLogAction(new LogAction(0, `Tile type "${repeatTile}" (${repeatTileTypeValue})`, [repeatTileTypeValue]));

                //dungeon_crawler.core.globals.logs.addEntry(logEntry);
            }
        }

        if (dungeon_crawler.core.globals.InCombat) {
            selectedTile.setType(dungeon_crawler.main.combat());
        }

        if (selectedTile.getType() == dungeon_crawler.core.globals.tileTypes['stairs_descending']) {

            let nextlevel = dungeon_crawler.core.globals.currentLevel.getNextlevel();

            dungeon_crawler.main.generateLevel(nextlevel);

            dungeon_crawler.core.globals.logs.addEntry(new LogEntry(dungeon_crawler.log_text.generateStairsDownText(nextlevel)));

        }

        if (dungeon_crawler.core.globals.adventurer.isAlive()) {
            this.setSelectables();
        } else {
            this.unsetSelectables();
        }

        dungeon_crawler.main.setStage();

        dungeon_crawler.core.globals.eventBindings.unbindEvents();
        dungeon_crawler.core.globals.eventBindings.clearBoundEvents();

        dungeon_crawler.main.bindEvents();
    }

    checkEndLevelTileDeployed() {
        if (!dungeon_crawler.core.globals.currentLevel.isEndLevelTileDeployed()) {
            let quadsExplored = Math.floor(this._tiles.length / 4) * 2;

            if (this._explored > quadsExplored) {
                // 50/50 change of stairs being deplyed or if last tile then deply it
                if ((Math.floor(Math.random() * 2) == 0) || this._explored >= (this._tiles.length - 1)) {
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
    selectTileType(value) {
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
    getRepeatTileType(value) {
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

    getSelectables() {
        let tile, selectableTiles = [];
        for (var i = 0; i < this._tiles.length; i++) {
            tile = this._tiles[i];

            if (tile.isSelectable()) {
                selectableTiles.push(tile);
            }
        }

        return selectableTiles;
    }

    setSelectables() {
        let current = this._tiles[this._currentIndex];
        let currentRow = current.getRow();
        let currentColumn = current.getColumn();

        let tile, previousTileRow, currentTileRow, nextTileRow, previousTileColumn, currentTileColumn, nextTileColumn;
        for (var i = 0; i < this._tiles.length; i++) {
            tile = this._tiles[i];
            tile.setSelectable(false);

            currentTileRow = tile.getRow();
            previousTileRow = currentTileRow - 1;
            nextTileRow = currentTileRow + 1;

            currentTileColumn = tile.getColumn();
            previousTileColumn = currentTileColumn - 1;
            nextTileColumn = currentTileColumn + 1;

            if ((currentTileColumn % 2) == 1) {
                //previous and next column
                if (previousTileColumn == currentColumn || nextTileColumn == currentColumn) {
                    if (previousTileRow == currentRow || currentTileRow == currentRow) {
                        tile.setSelectable(true);
                    }
                }

                //curent column
                if (currentTileColumn == currentColumn) {
                    if (previousTileRow == currentRow || nextTileRow == currentRow) {
                        tile.setSelectable(true);
                    }
                }
            } else {
                //previous and next column
                if (previousTileColumn == currentColumn || nextTileColumn == currentColumn) {
                    if (currentTileRow == currentRow || nextTileRow == currentRow) {
                        tile.setSelectable(true);
                    }
                }

                //curent column
                if (currentTileColumn == currentColumn) {
                    if (previousTileRow == currentRow || nextTileRow == currentRow) {
                        tile.setSelectable(true);
                    }
                }
            }
        }
    }

    unsetSelectables() {
        let tile;
        for (var i = 0; i < this._tiles.length; i++) {
            tile = this._tiles[i];
            tile.setSelectable(false);
        }
    }
}; 