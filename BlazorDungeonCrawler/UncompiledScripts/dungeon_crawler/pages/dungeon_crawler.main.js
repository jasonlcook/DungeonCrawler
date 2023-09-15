dungeon_crawler.main = {
    startup() {
        dungeon_crawler.main.displayVersion();

        dungeon_crawler.main.generateAdventurer();
        dungeon_crawler.main.setAdventurerDetails();

        dungeon_crawler.main.generateLevel(1);
    },

    restart() {
        dungeon_crawler.core.globals.logs = new Logs();
        dungeon_crawler.core.globals.levels = [];
        $('#log').html('');
        dungeon_crawler.main.startup();
    },

    getVersion() {
        return `${dungeon_crawler.core.globals.versionMajor}.${dungeon_crawler.core.globals.versionMinor}.${dungeon_crawler.core.globals.versionPatch}`
    },

    displayVersion() {
        $('#version').html(dungeon_crawler.main.getVersion());
    },

    generateLevel(value) {
        let currentVisitedLevel = null;
        let visitedLevel, visitedLevels = dungeon_crawler.core.globals.levels;
        for (var i = 0; i < visitedLevels.length; i++) {
            visitedLevel = visitedLevels[i];
            if (visitedLevel.getLevel() == value) {
                dungeon_crawler.core.globals.currentLevel = currentVisitedLevel;
                break;
            }
        }

        if (currentVisitedLevel == null) {
            dungeon_crawler.core.globals.currentLevel = new Level(value);
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

        //Button
        //  Advance
        dispacter = $('#advance');
        type = 'click';
        handler = dungeon_crawler.main.advanceAdventurer;
        name = 'auto_advance_adventurer';

        dungeon_crawler.core.globals.eventBindings.addEventBinding(dispacter, type, handler, name);

        //log
        //  Entry
        //      Show log actions dice rolls
        dispacter = $('#log .log-entry .log-entry-message');
        type = 'mouseenter';
        handler = dungeon_crawler.main.showLogActionsDice;
        name = 'log_entry_action_dice';

        dungeon_crawler.core.globals.eventBindings.addEventBinding(dispacter, type, handler, name);

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

    advanceAdventurer(event) {
        if (event !== null && typeof event.target !== 'undefined' || event.target !== null) {
            if (dungeon_crawler.core.globals.adventurer.isAlive()) {
                let tiles = dungeon_crawler.core.globals.currentLevel.getTiles();

                let selectablesTiles = tiles.getSelectables();

                //If MacGuffin has not been found remove stairs ascending from selectable tiles
                if (!dungeon_crawler.core.globals.macguffinFound) {
                    let selectablesTile;
                    for (var i = 0; i < selectablesTiles.length; i++) {
                        selectablesTile = selectablesTiles[i];
                        if (selectablesTile.getType() == dungeon_crawler.core.globals.tileTypes['stairs_ascending']) {
                            selectablesTiles.splice(i, 1);
                        }
                    }
                }

                let selectedTile = null;

                if (!dungeon_crawler.core.globals.macguffinFound) {
                    //randomly selecte and hidden tile from the selectables
                    let selectablesTile, selectableHiddenTiles = [];
                    for (var i = 0; i < selectablesTiles.length; i++) {
                        selectablesTile = selectablesTiles[i];

                        if (selectablesTile.isHidden()) {
                            selectableHiddenTiles.push(selectablesTile);
                        }
                    }

                    selectablesTileIndex = Math.floor(Math.random() * (selectablesTiles.length));
                    if (selectableHiddenTiles.length > 0) {
                        selectablesTileIndex = Math.floor(Math.random() * (selectableHiddenTiles.length));
                        selectedTile = selectableHiddenTiles[selectablesTileIndex];
                    }
                } else {
                    //plot a course between current position and stairs
                    let spawnCoordinates = dungeon_crawler.core.globals.currentLevel.getSpawnCoordinates();
                    let spawnRow = spawnCoordinates['row'];
                    let spawnColumn = spawnCoordinates['column'];

                    let currentTile = tiles.get(tiles.getCurrentIndex());
                    let currentRow = currentTile.getRow();
                    let currentColumn = currentTile.getColumn();

                    let directionOfTravel = dungeon_crawler.main.getDirectionOfTravel(spawnRow, spawnColumn, currentRow, currentColumn);

                    console.log('directionOfTravel: ' + directionOfTravel);

                    let selectablesTile, selectablesTileRow, selectablesTileColumn;
                    for (var i = 0; i < selectablesTiles.length; i++) {
                        selectablesTile = selectablesTiles[i];

                        selectablesTileRow = selectablesTile.getRow();
                        selectablesTileColumn = selectablesTile.getColumn();

                        if (dungeon_crawler.main.matchesDirectionOfTravel(directionOfTravel, selectablesTileRow, selectablesTileColumn, currentRow, currentColumn)) {
                            selectedTile = selectablesTile;
                            break;
                        }
                    }
                }

                //if no desirable tile is slected just travel at random
                if (selectedTile == null) {
                    selectablesTileIndex = Math.floor(Math.random() * (selectablesTiles.length));
                    selectedTile = selectablesTiles[selectablesTileIndex];
                }

                dungeon_crawler.main.moveToTile(selectedTile.getId());
            } else {
                dungeon_crawler.main.restart();
            }
        }
    },

    getDirectionOfTravel(spawnRow, spawnColumn, currentRow, currentColumn) {
        if (currentColumn == spawnColumn) {
            if (currentRow > spawnRow) {
                return 'N';
            } else {
                return 'S';
            }
        } else if (currentColumn < spawnColumn) {
            if (currentColumn % 2) {
                //long one
                if (currentRow > spawnRow) {
                    return 'NE';
                } else {
                    return 'SE';
                }
            } else {
                //short one
                if (currentRow >= spawnRow) {
                    return 'NE';
                } else {
                    return 'SE';
                }
            }
        } else if (currentColumn > spawnColumn) {
            if (currentColumn % 2) {
                //long one
                if (currentRow > spawnRow) {
                    return 'NW';
                } else {
                    return 'SW';
                }
            } else {
                //short one
                if (currentRow >= spawnRow) {
                    return 'NW';
                } else {
                    return 'SW';
                }
            }
        }
    },

    matchesDirectionOfTravel(directionOfTravel, tileRow, tileColumn, currentRow, currentColumn) {
        switch (directionOfTravel) {
            case 'N':
                if (currentColumn == tileColumn && currentRow > tileRow) {
                    return true;
                }
                break;
            case 'NE':
                if (currentColumn < tileColumn) {
                    if (currentColumn % 2) {
                        //long one
                        if (currentRow > tileRow) {
                            return true;
                        }
                    } else {
                        //short one
                        if (currentRow == tileRow) {
                            return true;
                        }
                    }
                }
                break;
            case 'SE':
                if (currentColumn < tileColumn) {
                    if (currentColumn % 2) {
                        //long one
                        if (currentRow == tileRow) {
                            return true;
                        }

                    } else {
                        //short one
                        if (currentRow < tileRow) {
                            return true;
                        }
                    }
                }
                break;
            case 'S':
                if (currentColumn == tileColumn && currentRow < tileRow) {
                    return true;
                }
                break;
            case 'SW':
                if (currentColumn > tileColumn) {
                    if (currentColumn % 2) {
                        //long one
                        if (currentRow == tileRow) {
                            return true;
                        }
                    } else {
                        //short one
                        if (currentRow < tileRow) {
                            return true;
                        }
                    }
                }
                break;
            case 'NW':
                if (currentColumn > tileColumn) {
                    if (currentColumn % 2) {
                        //long one
                        if (currentRow > tileRow) {
                            return true;
                        }
                    } else {
                        //short one
                        if (currentRow == tileRow) {
                            return true;
                        }
                    }
                }
                break;
        }
    },

    showLogActionsDice(event) {
        if (event !== null && typeof event.target !== 'undefined' || event.target !== null) {
            dungeon_crawler.main.resetDiceValues();

            let logEntryId = $(event.target).parents('.log-entry').attr('data-identity');
            let logEntry = dungeon_crawler.core.globals.logs.getLogEntryFromId(logEntryId);

            let logAction, logActions = logEntry.getLogActions();
            for (var i = 0; i < logActions.length; i++) {
                logAction = logActions[i];

                //Dice
                //  Safe
                logActionSafeDice = logAction.getSafeDice();
                if (typeof logActionSafeDice != 'undefined' && logActionSafeDice != null && logActionSafeDice.length > 0) {
                    dungeon_crawler.main.setSafeDice(logActionSafeDice);
                }

                //  Danger
                logActionDangerDice = logAction.getDangerDice();
                if (typeof logActionDangerDice != 'undefined' && logActionDangerDice != null && logActionDangerDice.length > 0) {
                    dungeon_crawler.main.setDangerDice(logActionDangerDice);
                }
            }
        }
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
            dungeon_crawler.main.moveToTile(tileId);
        }
    },

    moveToTile(tileId) {
        let selectedTile = dungeon_crawler.core.globals.currentLevel.getTileById(tileId);

        if (selectedTile.isSelectable()) {
            dungeon_crawler.core.globals.currentLevel.tilesMovement(selectedTile);
        }
    },

    combat() {
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

        let adventurerWoundsInflicted = 0, adventurerWoundsReceived = 0;
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

                wounds = 0;
                attackValue = adventurerRollValue + adventurerDamage;
                avoidValue = enemyRollValue + enemyProtection;
                if (attackValue > avoidValue) {
                    wounds = attackValue - avoidValue;

                    if (wounds < 0) {
                        wounds = 0;
                    }

                    currentEnemy.reciveWounds(wounds);
                }

                adventurerAttackAction = dungeon_crawler.log_text.generateAdventurerAttackText(enemyType, adventurerRollValue, adventurerDamage, attackValue, enemyRollValue, enemyProtection, avoidValue, wounds, currentEnemy.getHealth());
                logEntry.addLogAction(new LogAction(round, adventurerAttackAction, adventurerRolls, enemyRolls));

                adventurerWoundsInflicted += wounds;
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

                wounds = 0;
                if (attackValue > avoidValue) {
                    wounds = attackValue - avoidValue;

                    if (wounds < 0) {
                        wounds = 0;
                    }

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

                adventurerWoundsReceived += wounds;
            }

            adventurerInitiatesCombat = true;

        } while (dungeon_crawler.core.globals.adventurer.isAlive() && currentEnemy.isAlive());

        //add round for event
        round += 1;

        let deathText, battleTileResult;
        if (dungeon_crawler.core.globals.adventurer.isAlive()) {
            deathText = dungeon_crawler.log_text.generateEnemyDeathText(enemyType);
            logEntry.setTitle(deathText + ` Damage received ${adventurerWoundsReceived}.  Damage inflicted ${adventurerWoundsInflicted}`);

            battleTileResult = dungeon_crawler.core.globals.tileTypes['fight_won'];
        } else {
            deathText = dungeon_crawler.log_text.generateAdventurerDeathText(enemyType);
            logEntry.setTitle(deathText + ` Damage received ${adventurerWoundsReceived}.  Damage inflicted ${adventurerWoundsInflicted}`);

            battleTileResult = dungeon_crawler.core.globals.tileTypes['adventurer_death'];
        }

        dungeon_crawler.core.globals.logs.addEntry(logEntry);

        return battleTileResult;
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
    roleDSix() {
        return Math.floor(Math.random() * (6)) + 1;
    },

    setStage() {
        $('#stage').html('').css({ 'height': `${dungeon_crawler.core.globals.stageHeight}px`, 'width': `${dungeon_crawler.core.globals.stageWidth}px` });

        let tile, tileTypeClass, tiles = dungeon_crawler.core.globals.currentLevel.getTiles();

        let colourClass;
        switch (dungeon_crawler.core.globals.currentLevel.getLevel()) {
            case 1:
            case 2:
            case 3:
                colourClass = 'hexagon-colour-red';
                break;
            case 4:
            case 5:
            case 6:
            case 7:
                colourClass = 'hexagon-colour-blue';
                break;
            case 8:
                colourClass = 'hexagon-colour-purple';
                break;
            case 9:
                colourClass = 'hexagon-colour-green';
                break;
            case 10:
                colourClass = 'hexagon-colour-pink';
                break;
        }

        for (var i = 0; i < tiles.length; i++) {
            tileTypeClass = 'hexagon-tile-hidden';

            tileSelectableClass = '';

            tile = tiles.get(i);

            if (!tile.isHidden()) {
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
                        if (dungeon_crawler.core.globals.macguffinFound) {
                            tileTypeClass = 'hexagon-tile-stairs-ascending-active';
                        }
                        break;
                    case dungeon_crawler.core.globals.tileTypes['stairs_descending']:
                        tileTypeClass = 'hexagon-tile-stairs-descending';
                        if (!dungeon_crawler.core.globals.macguffinFound) {
                            tileTypeClass = 'hexagon-tile-stairs-descending-active';
                        }
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

            $('#stage').append(`<div class="hexagon-tile ${colourClass} hexagon-tile-base" style="left: ${tile.getX()}px; top: ${tile.getY()}px"><span></span></div>`);

            if (tile.isHidden()) {
                $('#stage').append(`<div class="hexagon-tile hexagon-tile-hidden" style="left: ${tile.getX()}px; top: ${tile.getY()}px"><span></span></div>`);
            } else {
                $('#stage').append(`<div class="hexagon-tile ${colourClass} ${tileTypeClass}" style="left: ${tile.getX()}px; top: ${tile.getY()}px"><span></span></div>`);

                if (!tile.getCurrent()) {
                    $('#stage').append(`<div class="hexagon-tile hexagon-for-of-war" style="left: ${tile.getX()}px; top: ${tile.getY()}px"><span></span></div>`);
                }
            }    

            if (tile.isSelectable()) {
                $('#stage').append(`<div data-identity="${tile.getId()}" class="hexagon-tile ${colourClass} hexagon-tile-selectable" style="left: ${tile.getX()}px; top: ${tile.getY()}px"><span></span></div>`);
            }
        }
    },

    //Adventurer
    generateAdventurer() {
        let adventurer = new Adventurer();

        //Health
        let healthValue = 0, healthRolls = [];

        healthRolls.push(dungeon_crawler.main.roleDSix());
        healthRolls.push(dungeon_crawler.main.roleDSix());

        for (var i = 0; i < healthRolls.length; i++) {
            healthValue += healthRolls[i]
        }

        adventurer.setInitialHealth(healthValue);

        //Damage
        let damageValue = 0, damageRolls = [];

        damageRolls.push(dungeon_crawler.main.roleDSix());

        for (var i = 0; i < damageRolls.length; i++) {
            damageValue += damageRolls[i]
        }

        adventurer.setDamage(damageValue);

        //Protection
        let protectionValue = 0, protectionRolls = [];

        protectionRolls.push(dungeon_crawler.main.roleDSix());

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

    setSafeDice(values) {
        let value;
        for (var i = 0; i < values.length; i++) {
            value = values[i];
            dungeon_crawler.main.setSafeDieValue(value);
        }
    },

    setSafeDieValue(value) {
        let dieHTML = dungeon_crawler.main.getDieHTML(value, 'safe');
        $('.dice').append(dieHTML);
    },

    setDangerDice(values) {
        let value;
        for (var i = 0; i < values.length; i++) {
            value = values[i];
            dungeon_crawler.main.setDangerDieValue(value);
        }
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



        return `<div class="die ${diceClass} ${type}" style="rotate: ${Math.floor(Math.random() * (180))}deg"></div>`;
    }
};

window['dungeon_crawler_main_startup'] = dungeon_crawler.main.startup;