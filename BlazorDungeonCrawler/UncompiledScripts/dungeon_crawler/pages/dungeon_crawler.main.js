dungeon_crawler.main = {
    startup() {


        dungeon_crawler.main.generateAdventurer();
        dungeon_crawler.main.setAdventurerDetails();

        dungeon_crawler.main.generateLevel(1);
    },

    generateLevel(value) {
        let currentVisitedLevel = null;
        let visitedLevel, visitedLevels = dungeon_crawler.core.globals.levels;
        for (var i = 0; i < visitedLevels.length; i++) {
            visitedLevel = visitedLevels[i];
            if (visitedLevel.getLevel() == value) {
                currentVisitedLevel = visitedLevel;
            }
        }

        let level;
        if (currentVisitedLevel == null) {
            level = new Level();
            level.loadLevel(value);

            dungeon_crawler.core.globals.levels.push(level);
            dungeon_crawler.core.globals.currentLevel = level;

            //set tiles
            dungeon_crawler.main.setTiles(level.getStageCols(), level.getStageRows());

            //set spawn location
            dungeon_crawler.core.globals.currentLevel.setSpawn(level.getLevel());
        } else {
            dungeon_crawler.core.globals.currentLevel = currentVisitedLevel;
            level = currentVisitedLevel;
        }

        dungeon_crawler.main.setLevelDetails();

        dungeon_crawler.core.globals.currentLevel.setSelectableTiles();

        dungeon_crawler.main.setStage();

        //
        if (dungeon_crawler.core.globals.eventBindings != null) {
            dungeon_crawler.core.globals.eventBindings.unbindEvents();
            dungeon_crawler.core.globals.eventBindings.clearBoundEvents();
        }

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
            let selectedTile = dungeon_crawler.core.globals.currentLevel.getTileById(id);

            if (selectedTile.getSelectable()) {
                dungeon_crawler.core.globals.currentLevel.tilesMovement(selectedTile);
            }
        }
    },

    combat() {
        let monsterDifficulty = dungeon_crawler.main.selectMonsterDifficulty();

        let enemy = dungeon_crawler.core.globals.currentLevel.getEnemy();

        //todo: use monster difficulty role to generate monsters
        let currentEnemy = new Enemy();
        currentEnemy.generateEnemy(enemy.type, enemy.healthDice, enemy.damageDice, enemy.protectionDice);

        dungeon_crawler.main.resetDiceValues();
        let adventurerScore = dungeon_crawler.main.roleSafeDie();
        let monsterScore = dungeon_crawler.main.roleDangerDie();

        //If Adventurer wins the roll they starts combat
        let adventurerInitiatesCombat = false;
        if (adventurerScore > monsterScore) {
            adventurerInitiatesCombat = true;
        }

        let enemyType = currentEnemy.getType();

        dungeon_crawler.main.monsterEncounterText(adventurerInitiatesCombat, enemyType, currentEnemy.getHealth(), currentEnemy.getDamage(), currentEnemy.getProtection());

        let adventurerDamage = dungeon_crawler.core.globals.adventurer.getDamage();
        let adventurerProtection = dungeon_crawler.core.globals.adventurer.getProtection();

        let enemyDamage = currentEnemy.getDamage();
        let enemyProtection = currentEnemy.getProtection();

        let adventurerRoll, enemyRoll, attackValue, avoidValue, wounds;
        do {
            //Adventurer fight
            dungeon_crawler.main.resetDiceValues();
            wounds = null;
            if (adventurerInitiatesCombat) {
                adventurerRoll = dungeon_crawler.main.roleSafeDie();
                attackValue = adventurerRoll + adventurerDamage;

                enemyRoll = dungeon_crawler.main.roleDangerDie();
                avoidValue = enemyRoll + enemyProtection;

                if (attackValue > avoidValue) {
                    wounds = attackValue - avoidValue;
                    currentEnemy.reciveWounds(wounds);
                }

                dungeon_crawler.main.adventurerAttackText(enemyType, adventurerRoll, adventurerDamage, attackValue, enemyRoll, enemyProtection, avoidValue, wounds);
            }

            //Monster fight
            dungeon_crawler.main.resetDiceValues();
            wounds = null;
            if (currentEnemy.isAlive()) {
                enemyRoll = dungeon_crawler.main.roleDangerDie();
                attackValue = enemyRoll + enemyDamage;

                adventurerRoll = dungeon_crawler.main.roleSafeDie();
                avoidValue = adventurerRoll + adventurerProtection;

                if (attackValue > avoidValue) {
                    wounds = attackValue - avoidValue;

                    //Damage will be dealt to Shield potion (if avalible), then Aura potion (if avalible) and finaly the Adventurer.  
                    //The function will return the number of wounds taken by the Adventurer.
                    let adventurerWounds = dungeon_crawler.core.globals.adventurer.reciveWounds(wounds);
                    wounds = adventurerWounds;

                    dungeon_crawler.main.updateAdventurerHealth();

                    //as the Shield potion may have taken dammage it too is updated
                    dungeon_crawler.main.updateAdventurerProtection();
                }

                dungeon_crawler.main.enemyAttackText(enemyType, enemyRoll, enemyDamage, attackValue, adventurerRoll, adventurerProtection, avoidValue, wounds);
            }

            adventurerInitiatesCombat = true;

        } while (dungeon_crawler.core.globals.adventurer.isAlive() && currentEnemy.isAlive());

        if (dungeon_crawler.core.globals.adventurer.isAlive()) {
            dungeon_crawler.main.enemyDeathText(enemyType);
            return dungeon_crawler.core.globals.tileTypes['fight_won'];
        } else {
            dungeon_crawler.main.adventurerDeathText(enemyType);
            return dungeon_crawler.core.globals.tileTypes['adventurer_death'];
        }
    },

    endGamge() {
        dungeon_crawler.core.globals.currentLevel.setUnselectableTiles();
        dungeon_crawler.main.setStage();

        dungeon_crawler.core.globals.eventBindings.unbindEvents();
        dungeon_crawler.core.globals.eventBindings.clearBoundEvents();
    },

    //Monster difficulty selection
    selectMonsterDifficulty() {
        return dungeon_crawler.main.roleDangerDie();
    },

    //Loot
    //  1 - 2:  Potion
    //  3 - 4:  Weapon
    //  5 - 6:  Protection
    selectLoot() {
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
            case 2:
                return dungeon_crawler.core.globals.tileTypes['potion'];
                break;
            case 3:
            case 4:
                return dungeon_crawler.core.globals.tileTypes['weapon'];
                break;
            case 5:
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

            dungeon_crawler.core.globals.currentLevel.addTile(new Tile(i, dungeon_crawler.core.globals.tileTypes['unknown'], hexRow, hexColumn, hexagonLeft, hexagonTop));
        }
    },

    setStage() {
        $('#stage').html('').css({ 'height': `${dungeon_crawler.core.globals.stageHeight}px`, 'width': `${dungeon_crawler.core.globals.stageWidth}px` });

        let tile, tileTypeClass, tileSelectableClass, tileText, tiles = dungeon_crawler.core.globals.currentLevel.getTiles();

        for (var i = 0; i < tiles.length; i++) {
            tileTypeClass = 'hexagon-tile-hidden';
            tileSelectableClass = '';

            tile = tiles.get(i);

            tileText = `${tile.getRow()} - ${tile.getColumn()}`;

            if (!tile.getHidden()) {
                switch (tile.getType()) {
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
                    case dungeon_crawler.core.globals.tileTypes['fight_won']:
                        tileTypeClass = 'hexagon-tile-fight-won';
                        break;
                    case dungeon_crawler.core.globals.tileTypes['chest']:
                        tileTypeClass = 'hexagon-tile-chest';
                        break;
                    case dungeon_crawler.core.globals.tileTypes['weapon']:
                        tileTypeClass = 'hexagon-tile-weapon';
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
                    case dungeon_crawler.core.globals.tileTypes['macguffin']:
                        tileTypeClass = 'hexagon-tile-macguffin';
                        break;
                    case dungeon_crawler.core.globals.tileTypes['adventurer_death']:
                        tileTypeClass = 'hexagon-tile-adventurer-death';
                        break;
                    default:
                    case dungeon_crawler.core.globals.tileTypes['unknown']:
                        tileTypeClass = 'hexagon-tile-unknown';
                        break;
                }
            }

            if (tile.getSelectable()) {
                tileSelectableClass = 'hexagon-tile-selectable';
            }

            $('#stage').append(`<div data-identity="${tile.getId()}" class="hexagon-tile ${tileTypeClass} ${tileSelectableClass}" style="left: ${tile.getX()}px; top: ${tile.getY()}px"><span>${tileText}</span></div>`);
        }
    },

    //Adventurer
    generateAdventurer() {
        let adventurer = new Adventurer();
        adventurer.rollInitialHealth();
        adventurer.rollInitialDamage();
        adventurer.rollInitialProtection();

        dungeon_crawler.core.globals.adventurer = adventurer;

        dungeon_crawler.main.startingAdventurerText();
    },

    setLevelDetails() {
        $('#current-level').html(dungeon_crawler.core.globals.currentLevel.getLevel());
    },

    setAdventurerDetails() {
        dungeon_crawler.main.updateAdventurerHealth();
        dungeon_crawler.main.updateAdventurerDamage();
        dungeon_crawler.main.updateAdventurerProtection();
    },

    updateAdventurerHealth() {
        $('#current-health').html(dungeon_crawler.core.globals.adventurer.getHealthDescription());
    },

    updateAdventurerDamage() {
        $('#current-damage').html(dungeon_crawler.core.globals.adventurer.getDamageDescription());
    },

    updateAdventurerProtection() {
        $('#current-protection').html(dungeon_crawler.core.globals.adventurer.getProtectionDescription());
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
    startingAdventurerText() {
        let message = dungeon_crawler.log_text.generateStartingAdventurerText(dungeon_crawler.core.globals.adventurer.getHealthText(), dungeon_crawler.core.globals.adventurer.getDamageText(), dungeon_crawler.core.globals.adventurer.getProtectionText());
        dungeon_crawler.core.globals.logs.addEntry(new LogEntry(message));
    },

    //  Adventurer
    //      Weapon
    //          Pickup
    setWeaponUseText(type, condition, weaponValue) {
        let message = dungeon_crawler.log_text.generateWeaponValuenUseText(type, condition, weaponValue);
        dungeon_crawler.core.globals.logs.addEntry(new LogEntry(message));
    },

    //          Discard
    setWeaponDiscardText(type, condition, weaponValue) {
        let message = dungeon_crawler.log_text.generateWeaponDiscardText(type, condition, weaponValue);
        dungeon_crawler.core.globals.logs.addEntry(new LogEntry(message));
    },

    //      Armour
    //          Pickup
    setProtectionUseText(type, condition, armourValue) {
        let message = dungeon_crawler.log_text.generateProtectionUseText(type, condition, armourValue);
        dungeon_crawler.core.globals.logs.addEntry(new LogEntry(message));
    },

    //          Discard
    setProtectionDiscardText(type, condition, armourValue) {
        let message = dungeon_crawler.log_text.generateProtectionDiscardText(type, condition, armourValue);
        dungeon_crawler.core.globals.logs.addEntry(new LogEntry(message));
    },

    //      Potion
    //          Use
    usePotionText(potionType, potionSize, potionDuration) {
        let message = dungeon_crawler.log_text.generateUsePotionText(potionType, potionSize, potionDuration);
        dungeon_crawler.core.globals.logs.addEntry(new LogEntry(message));
    },

    //          Heal
    usePotionHealingText(regainedHealth) {
        let message = dungeon_crawler.log_text.generatePotionHealingText(regainedHealth);
        dungeon_crawler.core.globals.logs.addEntry(new LogEntry(message));
    },

    //      Combat
    adventurerAttackText(enemyType, adventurerRoll, adventurerDamage, adventurerAttackValue, enemyRoll, enemyProtection, enemyAvoidValue, wounds) {
        let message = dungeon_crawler.log_text.generateAdventurerAttackText(enemyType, adventurerRoll, adventurerDamage, adventurerAttackValue, enemyRoll, enemyProtection, enemyAvoidValue, wounds);
        dungeon_crawler.core.globals.logs.addEntry(new LogEntry(message));
    },

    //      Death
    adventurerDeathText(enemyType) {
        let message = dungeon_crawler.log_text.generateAdventurerDeathText(enemyType);
        dungeon_crawler.core.globals.logs.addEntry(new LogEntry(message));
    },

    //  Stairs
    //      Down
    stairsDownText(level) {
        let message = dungeon_crawler.log_text.generateStairsDownText(level);
        dungeon_crawler.core.globals.logs.addEntry(new LogEntry(message));
    },

    //      Up
    stairsUpText(level) {
        let message = dungeon_crawler.log_text.generateStairsUpText(level);
        dungeon_crawler.core.globals.logs.addEntry(new LogEntry(message));
    },

    //  MacGuffin
    //      Pickup
    macGuffinText() {
        let message = dungeon_crawler.log_text.generateMacGuffinText();
        dungeon_crawler.core.globals.logs.addEntry(new LogEntry(message));
    },

    //      Exit without
    exitWithoutMacGuffinText() {
        let message = dungeon_crawler.log_text.generateExitWithoutMacGuffinText();
        dungeon_crawler.core.globals.logs.addEntry(new LogEntry(message));
    },

    //      Exit with
    exitWithMacGuffinText() {
        let message = dungeon_crawler.log_text.generateExitWithMacGuffinText();
        dungeon_crawler.core.globals.logs.addEntry(new LogEntry(message));
    },

    //  Monster
    //      Combat
    monsterEncounterText(adventurerInitiatesCombat, enemyType, health, damage, protection) {
        let message = dungeon_crawler.log_text.generateMonsterEncounterText(adventurerInitiatesCombat, enemyType, health, damage, protection);
        dungeon_crawler.core.globals.logs.addEntry(new LogEntry(message));
    },

    //      Attack
    enemyAttackText(enemyType, enemyRoll, enemyDamage, attackValue, adventurerRoll, adventurerProtection, avoidValue, wounds) {
        let message = dungeon_crawler.log_text.generateEnemyAttackText(enemyType, enemyRoll, enemyDamage, attackValue, adventurerRoll, adventurerProtection, avoidValue, wounds);
        dungeon_crawler.core.globals.logs.addEntry(new LogEntry(message));
    },

    //      Death
    enemyDeathText(enemyType) {
        let message = dungeon_crawler.log_text.generateEnemyDeathText(enemyType);
        dungeon_crawler.core.globals.logs.addEntry(new LogEntry(message));
    }
};

window['dungeon_crawler_main_startup'] = dungeon_crawler.main.startup;