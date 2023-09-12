﻿dungeon_crawler.main = {
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

        //log
        //  Entity (expand log entry's actions)
        //      expand log actions
        dispacter = $('#log .log-entry .log-entry-message');
        type = 'click';
        handler = dungeon_crawler.main.addActionsToLogEntry;
        name = 'log_entry_action_expand';

        dungeon_crawler.core.globals.eventBindings.addEventBinding(dispacter, type, handler, name);

        //      remove log actions
        dispacter = $('#log .log-entry');
        type = 'mouseleave';
        handler = dungeon_crawler.main.removeActionsFromLogEntry;
        name = 'log_entry_action_remove';

        dungeon_crawler.core.globals.eventBindings.addEventBinding(dispacter, type, handler, name);

        //  Action (show action's dice roll)
        //      show dice
        dispacter = $('#log .log-entry .log-actions .log-action-message');
        type = 'mouseenter';
        handler = dungeon_crawler.main.showActionRoll;
        name = 'log_entry_action_dice_show';

        dungeon_crawler.core.globals.eventBindings.addEventBinding(dispacter, type, handler, name);

        //      remove dice
        dispacter = $('#log .log-entry .log-actions');
        type = 'mouseleave';
        handler = dungeon_crawler.main.hideActionRoll;
        name = 'log_entry_action_dice_hide';

        dungeon_crawler.core.globals.eventBindings.addEventBinding(dispacter, type, handler, name);

        //Tile
        dispacter = $('#stage .hexagon-tile span');
        type = 'click';
        handler = dungeon_crawler.main.tileClick;
        name = 'tile_click';

        dungeon_crawler.core.globals.eventBindings.addEventBinding(dispacter, type, handler, name);

        dungeon_crawler.core.globals.eventBindings.bindEvents();

    },

    addActionsToLogEntry(event) {
        if (event !== null && typeof event.target !== 'undefined' || event.target !== null) {
            //check if actions are already shown
            let $logActions = $(event.target).siblings('.log-actions');
            if ($logActions.length > 0) {
                $logActions.show().removeAttr('hidden');
            }
        }
    },

    //hide all log actions
    removeActionsFromLogEntry(event) {
        if (event !== null && typeof event.target !== 'undefined' || event.target !== null) {
            $('#log .log-entry ol.log-actions').hide().attr("hidden", true);
        }
    },

    showActionRoll(event) {
        if (event !== null && typeof event.target !== 'undefined' || event.target !== null) {
            let logEntryId = $(event.target).parents('.log-entry').attr('data-identity');
            let logActionId = $(event.target).parents('li').attr('data-identity');

            let logEntry = dungeon_crawler.core.globals.logs.getLogEntryFromId(logEntryId);
            if (logEntry != null) {
                dungeon_crawler.main.resetDiceValues();

                let LogAction = logEntry.getLogActionFromId(logActionId);
                if (LogAction != null) {
                    let safeDice = LogAction.getSafeDice();

                    if (safeDice != null) {
                        let safeDie;
                        for (var i = 0; i < safeDice.length; i++) {
                            safeDie = safeDice[i];
                            dungeon_crawler.main.setSafeDieValue(safeDie);
                        }
                    }

                    let dangerDice = LogAction.getDangerDice();

                    if (dangerDice != null) {
                        let dangerDie;
                        for (var i = 0; i < dangerDice.length; i++) {
                            dangerDie = dangerDice[i];
                            dungeon_crawler.main.setDangerDieValue(dangerDie);
                        }
                    }
                }
            }
        }
    },

    hideActionRoll(event) {
        dungeon_crawler.main.resetDiceValues();
    },

    tileClick(event) {
        if (event !== null && typeof event.target !== 'undefined' || event.target !== null) {
            let tileId = $(event.target.parentElement).attr('data-identity');
            let selectedTile = dungeon_crawler.core.globals.currentLevel.getTileById(tileId);

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
        let adventurerScore = dungeon_crawler.main.roleDSix();
        let monsterScore = dungeon_crawler.main.roleDSix();

        //If Adventurer wins the roll they starts combat
        let adventurerInitiatesCombat = false;
        if (adventurerScore > monsterScore) {
            adventurerInitiatesCombat = true;
        }

        let enemyType = currentEnemy.getType();

        let round = 0;
        let logEntry = new LogEntry();

        let encounterText = dungeon_crawler.log_text.generateMonsterEncounterText(adventurerInitiatesCombat, enemyType);
        logEntry.addLogAction(new LogAction(round, encounterText, [adventurerScore], [monsterScore]));

        let adventurerDamage = dungeon_crawler.core.globals.adventurer.getDamage();
        let adventurerProtection = dungeon_crawler.core.globals.adventurer.getProtection();

        let enemyDamage = currentEnemy.getDamage();
        let enemyProtection = currentEnemy.getProtection();

        let adventurerRollValue, enemyRollValue, attackValue, avoidValue, wounds;
        let adventurerAttackAction, enemyAttackAction;
        let adventurerRolls, enemyRolls;

        do {
            //Adventurer fight
            round += 1;

            adventurerRollValue = 0;
            enemyRollValue = 0;
            attackValue = 0;
            avoidValue = 0;
            adventurerRolls = [];
            enemyRolls = [];

            if (adventurerInitiatesCombat) {
                adventurerRolls.push(dungeon_crawler.main.roleDSix());
                for (var i = 0; i < adventurerRolls.length; i++) {
                    adventurerRollValue += adventurerRolls[i]
                }

                enemyRolls.push(dungeon_crawler.main.roleDSix());
                for (var i = 0; i < enemyRolls.length; i++) {
                    enemyRollValue += enemyRolls[i]
                }

                attackValue = adventurerRollValue + adventurerDamage;
                avoidValue = enemyRollValue + enemyProtection;
                if (attackValue > avoidValue) {
                    wounds = attackValue - avoidValue;
                    currentEnemy.reciveWounds(wounds);
                }

                adventurerAttackAction = dungeon_crawler.log_text.generateAdventurerAttackText(enemyType, adventurerRollValue, adventurerDamage, attackValue, enemyRollValue, enemyProtection, avoidValue, wounds, currentEnemy.getHealth());
                logEntry.addLogAction(new LogAction(round, adventurerAttackAction, adventurerRolls, enemyRolls));
            }

            //Monster fight
            adventurerRollValue = 0;
            enemyRollValue = 0;
            attackValue = 0;
            avoidValue = 0;
            adventurerRolls = [];
            enemyRolls = [];

            if (currentEnemy.isAlive()) {
                enemyRolls.push(dungeon_crawler.main.roleDSix());
                for (var i = 0; i < enemyRolls.length; i++) {
                    enemyRollValue += enemyRolls[i]
                }
                attackValue = enemyRollValue + enemyDamage;

                adventurerRolls.push(dungeon_crawler.main.roleDSix());
                for (var i = 0; i < adventurerRolls.length; i++) {
                    adventurerRollValue += adventurerRolls[i]
                }
                avoidValue = adventurerRollValue + adventurerProtection;

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

                enemyAttackAction = dungeon_crawler.log_text.generateEnemyAttackText(enemyType, enemyRollValue, enemyDamage, attackValue, adventurerRollValue, adventurerProtection, avoidValue, wounds, dungeon_crawler.core.globals.adventurer.getHealth());
                logEntry.addLogAction(new LogAction(round, enemyAttackAction, enemyRolls, adventurerRolls));
            }

            adventurerInitiatesCombat = true;

        } while (dungeon_crawler.core.globals.adventurer.isAlive() && currentEnemy.isAlive());

        //add round for event
        round += 1;

        let deathText, battleTileResult;
        if (dungeon_crawler.core.globals.adventurer.isAlive()) {
            deathText = dungeon_crawler.log_text.generateEnemyDeathText(enemyType);
            logEntry.setTitle(deathText);

            battleTileResult = dungeon_crawler.core.globals.tileTypes['fight_won'];
        } else {
            deathText = dungeon_crawler.log_text.generateAdventurerDeathText(enemyType);
            logEntry.setTitle(deathText);

            battleTileResult = dungeon_crawler.core.globals.tileTypes['adventurer_death'];
        }

        dungeon_crawler.core.globals.logs.addEntry(logEntry);

        return battleTileResult;
    },

    //Monster difficulty selection
    selectMonsterDifficulty() {
        return dungeon_crawler.main.roleDangerDie();
    },

    //Loot
    //  1 - 2:  Potion
    //  3 - 4:  Weapon
    //  5 - 6:  Protection
    selectLoot(value) {
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

        let tile, tileTypeClass, tileSelectableClass, tiles = dungeon_crawler.core.globals.currentLevel.getTiles();

        for (var i = 0; i < tiles.length; i++) {
            tileTypeClass = 'hexagon-tile-hidden';
            tileSelectableClass = '';

            tile = tiles.get(i);

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

            $('#stage').append(`<div data-identity="${tile.getId()}" class="hexagon-tile ${tileTypeClass} ${tileSelectableClass}" style="left: ${tile.getX()}px; top: ${tile.getY()}px"><span></span></div>`);
        }
    },

    //Adventurer
    generateAdventurer() {
        let adventurer = new Adventurer();

        //Health
        let healthValue = 0, healthRolls = [];

        healthRolls.push(dungeon_crawler.main.roleSafeDie());
        healthRolls.push(dungeon_crawler.main.roleSafeDie());

        for (var i = 0; i < healthRolls.length; i++) {
            healthValue += healthRolls[i]
        }

        adventurer.setInitialHealth(healthValue);

        //Damage
        let damageValue = 0, damageRolls = [];

        damageRolls.push(dungeon_crawler.main.roleSafeDie());

        for (var i = 0; i < damageRolls.length; i++) {
            damageValue += damageRolls[i]
        }

        adventurer.setDamage(damageValue);

        //Protection
        let protectionValue = 0, protectionRolls = [];

        protectionRolls.push(dungeon_crawler.main.roleSafeDie());

        for (var i = 0; i < protectionRolls.length; i++) {
            protectionValue += protectionRolls[i]
        }

        adventurer.setProtection(protectionValue);

        dungeon_crawler.core.globals.adventurer = adventurer;

        let health = dungeon_crawler.core.globals.adventurer.getHealthBase();
        let protection = dungeon_crawler.core.globals.adventurer.getProtectionBase();
        let damage = dungeon_crawler.core.globals.adventurer.getDamage();

        let logEntry = new LogEntry(dungeon_crawler.log_text.generateStartingAdventurerText(dungeon_crawler.core.globals.adventurer.getHealthText(), dungeon_crawler.core.globals.adventurer.getDamageText(), dungeon_crawler.core.globals.adventurer.getProtectionText()));

        logEntry.addLogAction(new LogAction(0, `Health roll ${health}`, healthRolls));
        logEntry.addLogAction(new LogAction(0, `Protection roll ${protection}`, protectionRolls));
        logEntry.addLogAction(new LogAction(0, `Damage roll ${damage}`, damageRolls));

        dungeon_crawler.core.globals.logs.addEntry(logEntry);
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
        $('.dice').html('');
    },

    setSafeDieValue(value) {
        let dieHTML = dungeon_crawler.main.getDieHTML(value, 'safe');
        $('.dice').append(dieHTML);
    },

    setDangerDieValue(value) {
        let dieHTML = dungeon_crawler.main.getDieHTML(value, 'danger');
        $('.dice').append(dieHTML);
    },

    getDieHTML(value, type) {
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

        return `<div class="die ${diceClass} ${type}"></div>`;
    }
};

window['dungeon_crawler_main_startup'] = dungeon_crawler.main.startup;