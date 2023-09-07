dungeon_crawler.main = {
    startup() {
        //generate level
        let level = new Level();

        level.loadLevel(5);

        dungeon_crawler.core.globals.currentLevel = level;

        //generate adventurer
        dungeon_crawler.main.generateAdventurer();

        //set level and adventurer details
        dungeon_crawler.main.setDetails();

        //set tiles
        dungeon_crawler.main.setTiles(level.stageCols, level.stageRows);

        //set spawn location
        dungeon_crawler.core.globals.currentLevel.setSpawn();


        dungeon_crawler.core.globals.currentLevel.tiles.setSelectables();


        dungeon_crawler.main.setStage();

        //
        dungeon_crawler.main.bindEvents();
    },

    bindEvents() {
        if (typeof dungeon_crawler.core.globals.eventBindings == 'undefined' || dungeon_crawler.core.globals.eventBindings == null) {
            dungeon_crawler.core.globals.eventBindings = new EventBindings();
        }

        let dispacter, type, handler, name;

        dispacter = $('#stage .hexagon-tile span');
        type = 'click';
        handler = dungeon_crawler.main.tileClick;
        name = 'tile_click';

        dungeon_crawler.core.globals.eventBindings.addEventBinding(dispacter, type, handler, name);

        dungeon_crawler.core.globals.eventBindings.bindEvents();
    },

    tileClick(event) {
        if (event !== null && typeof event.target !== 'undefined' || event.target !== null) {
            let id = $(event.target.parentElement).attr('data-identity');
            let selectedTile = dungeon_crawler.core.globals.currentLevel.tiles.getById(id);

            if (selectedTile.Selectable) {
                dungeon_crawler.main.movement(selectedTile);
            }
        }
    },

    movement(selectedTile) {
        dungeon_crawler.core.globals.currentLevel.InCombat = false;

        dungeon_crawler.main.resetDiceValues();

        //deselect previous tile
        let previousIndex = dungeon_crawler.core.globals.currentLevel.tiles.currentIndex;
        let previousTile = dungeon_crawler.core.globals.currentLevel.tiles.get(previousIndex);
        previousTile.Current = false;

        dungeon_crawler.core.globals.currentLevel.tiles.currentIndex = selectedTile.Index;
        selectedTile.Current = true;
        selectedTile.Hidden = false;

        if (selectedTile.Type == dungeon_crawler.core.globals.tileTypes['unknown']) {
            //if new tile then roll for content
            selectedTile.Type = dungeon_crawler.core.globals.currentLevel.tiles.getNextTileType();
        } else {
            if (selectedTile.Type == dungeon_crawler.core.globals.tileTypes['empty'] || selectedTile.Type == dungeon_crawler.core.globals.tileTypes['fight']) {
                //if tile has already been placed roll for monster encounter
                let repeatTile = dungeon_crawler.core.globals.currentLevel.tiles.getRepeatTileType();

                if (repeatTile != null) {
                    selectedTile.Type = repeatTile;
                }
            }
        }

        dungeon_crawler.core.globals.currentLevel.tiles.setSelectables();
        dungeon_crawler.main.setStage();

        dungeon_crawler.core.globals.eventBindings.unbindEvents();
        dungeon_crawler.core.globals.eventBindings.clearBoundEvents();

        dungeon_crawler.main.bindEvents();

        if (dungeon_crawler.core.globals.currentLevel.InCombat) {
            let monsterDifficulty = dungeon_crawler.main.selectMonsterDifficulty();

            let enemy = dungeon_crawler.core.globals.currentLevel.getEnemy();

            //todo: use monster difficulty role to generate monsters
            let currentEnemy = new Enemy();
            currentEnemy.generateEnemy(enemy.type, enemy.healthDice, enemy.strengthDice, enemy.armourDice);

            dungeon_crawler.main.resetDiceValues();
            let adventurerScore = dungeon_crawler.main.roleSafeDie();
            let monsterScore = dungeon_crawler.main.roleDangerDie();

            //If Adventurer wins the roll they starts combat
            let adventurerInitiatesCombat = false;
            if (adventurerScore > monsterScore) {
                adventurerInitiatesCombat = true;
            }

            let enemyType = currentEnemy.getType();

            dungeon_crawler.main.monsterEncounterText(adventurerInitiatesCombat, enemyType, currentEnemy.getHealth(), currentEnemy.getStrength(), currentEnemy.getArmour());

            let adventurerStrength = dungeon_crawler.core.globals.adventurer.getStrength();
            let adventurerArmour = dungeon_crawler.core.globals.adventurer.getArmour();

            let enemyStrength = currentEnemy.getStrength();
            let enemyArmour = currentEnemy.getArmour();

            let adventurerRoll, enemyRoll, attackValue, avoidValue, wounds;
            do {
                //Adventurer fight
                dungeon_crawler.main.resetDiceValues();
                wounds = null;
                if (adventurerInitiatesCombat) {
                    adventurerRoll = dungeon_crawler.main.roleSafeDie();
                    attackValue = adventurerRoll + adventurerStrength;

                    enemyRoll = dungeon_crawler.main.roleDangerDie();
                    avoidValue = enemyRoll + enemyArmour;

                    if (attackValue > avoidValue) {
                        wounds = attackValue - avoidValue;
                        currentEnemy.reciveWounds(wounds);
                    }

                    dungeon_crawler.main.adventurerAttackText(enemyType, adventurerRoll, adventurerStrength, attackValue, enemyRoll, enemyArmour, avoidValue, wounds);
                }

                //Monster fight
                dungeon_crawler.main.resetDiceValues();
                wounds = null;
                if (currentEnemy.isAlive()) {
                    enemyRoll = dungeon_crawler.main.roleDangerDie();
                    attackValue = enemyRoll + enemyStrength;

                    adventurerRoll = dungeon_crawler.main.roleSafeDie();
                    avoidValue = adventurerRoll + adventurerArmour;

                    if (attackValue > avoidValue) {
                        wounds = attackValue - avoidValue;
                        dungeon_crawler.core.globals.adventurer.reciveWounds(wounds);
                        dungeon_crawler.main.updateAdventurerHealth();
                    }

                    dungeon_crawler.main.enemyAttackText(enemyType, enemyRoll, enemyStrength, attackValue, adventurerRoll, adventurerArmour, avoidValue, wounds);
                }

                adventurerInitiatesCombat = true;

            } while (dungeon_crawler.core.globals.adventurer.isAlive() && currentEnemy.isAlive());

            if (dungeon_crawler.core.globals.adventurer.isAlive()) {
                dungeon_crawler.main.enemyDeathText(enemyType);
            } else {
                dungeon_crawler.main.adventurerDeathText(enemyType);
                dungeon_crawler.main.endGamge();
            }
        }
    },

    endGamge() {
        dungeon_crawler.core.globals.currentLevel.tiles.unsetSelectables();
        dungeon_crawler.main.setStage();

        dungeon_crawler.core.globals.eventBindings.unbindEvents();
        dungeon_crawler.core.globals.eventBindings.clearBoundEvents();
    },

    //Monster difficulty selection
    selectMonsterDifficulty() {
        return dungeon_crawler.main.roleDangerDie();
    },

    //Loot select
    //  1 - 3:  Loot
    //  4 - 5:  Potion
    //  6:      Protection
    selectLoot() {
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
            case 2:
            case 3:
                return dungeon_crawler.core.globals.tileTypes['loot'];
            case 4:
            case 5:
                return dungeon_crawler.core.globals.tileTypes['potion'];
            case 6:
                return dungeon_crawler.core.globals.tileTypes['protection'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected loot table role "${value}"`);
        return dungeon_crawler.core.globals.tileTypes['unknown'];
    },

    //Dice
    roleSafeDie() {
        let value = dungeon_crawler.main.roleDSix();
        dungeon_crawler.main.setSafeDieValue(value);
        return value;
    },

    roleDangerDie() {
        let value = dungeon_crawler.main.roleDSix();
        dungeon_crawler.main.setDangerDieValue(value);
        return value;
    },

    roleDSix() {
        return dungeon_crawler.main.roleDie(6);
    },

    roleDTwenty() {
        return dungeon_crawler.main.roleDie(20);
    },

    roleDie(max) {
        return Math.floor(Math.random() * (max)) + 1;
    },

    setTiles(stageCols, stageRows) {
        let hexagonHeight = dungeon_crawler.core.globals.hexHeight;
        let hexagonWidth = dungeon_crawler.core.globals.hexWidth;

        //set stage dimentions
        //  height
        let stageHeight = stageCols * hexagonHeight;
        dungeon_crawler.core.globals.stageHeight = stageHeight;

        //  width
        let hexWidthQuaters = hexagonWidth / 4;
        let stageWidth = stageRows * (hexWidthQuaters * 3) + hexWidthQuaters;
        dungeon_crawler.core.globals.stageWidth = stageWidth;

        //set board
        let hexagonLeft = 0, hexagonTop = 0, hexRow = -1, hexColumn = 0;

        //  due to the orientation of our board we miss one hex for every other grid row
        let tileCount = (stageCols * stageRows) - Math.ceil((stageRows + 1) / 2);

        hexagonTop -= hexagonHeight / 2;
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

            dungeon_crawler.core.globals.currentLevel.tiles.add(new Tile(i, dungeon_crawler.core.globals.tileTypes['unknown'], hexRow, hexColumn, hexagonLeft, hexagonTop))
        }
    },

    setStage() {
        $('#stage').html('').css({ 'height': `${dungeon_crawler.core.globals.stageHeight}px`, 'width': `${dungeon_crawler.core.globals.stageWidth}px` });

        let tileTypeClass, tileSelectableClass, tileText, tiles = dungeon_crawler.core.globals.currentLevel.tiles;

        for (var i = 0; i < tiles.length; i++) {
            tileTypeClass = 'hexagon-tile-hidden';
            tileSelectableClass = '';

            tile = tiles.get(i);

            tileText = `${tile.Row} - ${tile.Column}`;

            if (!tile.Hidden) {
                if (typeof tile.Hidden == 'undefined' || tile.Hidden == null) {
                    tileTypeClass = 'hexagon-tile-unknown';
                } else {
                    switch (tile.Type) {
                        //entrance
                        case dungeon_crawler.core.globals.tileTypes['entrance']:
                            tileTypeClass = 'hexagon-tile-entrance';
                            break;
                        //exit
                        case dungeon_crawler.core.globals.tileTypes['exit']:
                            tileTypeClass = 'hexagon-tile-exit';
                            break;
                        case dungeon_crawler.core.globals.tileTypes['stairs_ascending']:
                            tileTypeClass = 'hexagon-tile-stairs-ascending';
                            break;
                        case dungeon_crawler.core.globals.tileTypes['stairs_descending']:
                            tileTypeClass = 'hexagon-tile-stairs-descending';
                            break;
                        case dungeon_crawler.core.globals.tileTypes['fight']:
                            tileTypeClass = 'hexagon-tile-fight';
                            break;
                        case dungeon_crawler.core.globals.tileTypes['loot']:
                            tileTypeClass = 'hexagon-tile-loot';
                            break;
                        case dungeon_crawler.core.globals.tileTypes['protection']:
                            tileTypeClass = 'hexagon-tile-protection';
                            break;
                        case dungeon_crawler.core.globals.tileTypes['empty']:
                            tileTypeClass = 'hexagon-tile-empty';
                            break;
                        case dungeon_crawler.core.globals.tileTypes['potion']:
                            tileTypeClass = 'hexagon-tile-potion';
                            break;
                        default:
                        case dungeon_crawler.core.globals.tileTypes['unknown']:
                            tileTypeClass = 'hexagon-tile-unknown';
                            break;
                    }
                }
            }

            if (tile.Selectable) {
                tileSelectableClass = 'hexagon-tile-selectable';
            }

            $('#stage').append(`<div data-identity="${tile.Id}" class="hexagon-tile ${tileTypeClass} ${tileSelectableClass}" style="left: ${tile.X}px; top: ${tile.Y}px"><span>${tileText}</span></div>`);
        }
    },

    //Adventurer
    generateAdventurer() {
        let health = dungeon_crawler.main.roleSafeDie() + dungeon_crawler.main.roleSafeDie();
        let strength = dungeon_crawler.main.roleSafeDie();
        let armour = dungeon_crawler.main.roleSafeDie();

        dungeon_crawler.main.startingAdventurerText(health, strength, armour);

        dungeon_crawler.core.globals.adventurer = new Adventurer(health, strength, armour);
    },

    setDetails() {
        $('#current-level').html(dungeon_crawler.core.globals.currentLevel.level);

        dungeon_crawler.main.updateAdventurerHealth();
        dungeon_crawler.main.updateAdventurerStrength();
        dungeon_crawler.main.updateAdventurerArmour();
    },

    updateAdventurerHealth() {
        let health = dungeon_crawler.core.globals.adventurer.getHealth();
        $('#current-health').html(health);
    },

    updateAdventurerStrength(value) {
        let strength = dungeon_crawler.core.globals.adventurer.getStrength();
        $('#current-strength').html(strength);
    },

    updateAdventurerArmour(value) {
        let armour = dungeon_crawler.core.globals.adventurer.getArmour();
        $('#current-armour').html(armour);
    },

    resetDiceValues() {
        dungeon_crawler.main.resetSafeDieValue();
        dungeon_crawler.main.resetDangerDieValue();
    },

    resetSafeDieValue() {
        $('#current-dice-safe').html('');
    },

    setSafeDieValue(value) {
        let dieHTML = dungeon_crawler.main.getDieHTML(value);
        $('#current-dice-safe').append(dieHTML);
    },

    resetDangerDieValue() {
        $('#current-dice-danger').html('');
    },

    setDangerDieValue(value) {
        let dieHTML = dungeon_crawler.main.getDieHTML(value);
        $('#current-dice-danger').append(dieHTML);
    },

    getDieHTML(value) {
        let diceClass = "empty";

        switch (value) {
            case 1:
                diceClass = "one";
                break;
            case 2:
                diceClass = "two";
                break;
            case 3:
                diceClass = "three";
                break;
            case 4:
                diceClass = "four";
                break;
            case 5:
                diceClass = "five";
                break;
            case 6:
                diceClass = "six";
                break;
        }

        return `<div class="die ${diceClass}"></div>`;
    },

    //Log
    //  Story
    //      Adventurer
    startingAdventurerText(health, strength, armour) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateStartingAdventurerText(health, strength, armour));
    },

    //  Combat
    monsterEncounterText(adventurerInitiatesCombat, enemyType, health, strength, armour) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateMonsterEncounterText(adventurerInitiatesCombat, enemyType, health, strength, armour));
    },

    adventurerAttackText(enemyType, adventurerRoll, adventurerStrength, adventurerAttackValue, enemyRoll, enemyArmour, enemyAvoidValue, wounds) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateAdventurerAttackText(enemyType, adventurerRoll, adventurerStrength, adventurerAttackValue, enemyRoll, enemyArmour, enemyAvoidValue, wounds));
    },

    adventurerDeathText(enemyType) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateAdventurerDeathText(enemyType));
    },

    enemyAttackText(enemyType, enemyRoll, enemyStrength, attackValue, adventurerRoll, adventurerArmour, avoidValue, wounds) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateEnemyAttackText(enemyType, enemyRoll, enemyStrength, attackValue, adventurerRoll, adventurerArmour, avoidValue, wounds));
    },

    enemyDeathText(enemyType) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateEnemyDeathText(enemyType));
    },

    //      Health

    //      Strength

    //      Armour

    //  Monster

    //      Health


    setLog(message) {
        $('#log').prepend(`<div class="entry">${message}</div>`);
    }
};

window['dungeon_crawler_main_startup'] = dungeon_crawler.main.startup;