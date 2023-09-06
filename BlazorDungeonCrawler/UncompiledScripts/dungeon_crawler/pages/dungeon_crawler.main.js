dungeon_crawler.main = {
    startup() {
        let level = new Level();
        level.loadFirstLevel();

        dungeon_crawler.core.globals.currentLevel = level;

        dungeon_crawler.main.setTiles(level.stageCols, level.stageRows);

        dungeon_crawler.core.globals.currentLevel.setSpawn();

        dungeon_crawler.core.globals.currentLevel.tiles.setSelectables();

        dungeon_crawler.main.setDetails();

        dungeon_crawler.main.setStage();

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
                dungeon_crawler.main.resetSafeDieValue();
                dungeon_crawler.main.resetDangerDieValue();

                //deselect previous tile
                let previousIndex = dungeon_crawler.core.globals.currentLevel.tiles.currentIndex;
                let previousTile = dungeon_crawler.core.globals.currentLevel.tiles.get(previousIndex);
                previousTile.Current = false;

                dungeon_crawler.core.globals.currentLevel.tiles.currentIndex = selectedTile.Index;
                selectedTile.Current = true;
                selectedTile.Hidden = false;

                if (selectedTile.Type == dungeon_crawler.core.globals.tileTypes['unknown']) {
                    selectedTile.Type = dungeon_crawler.main.getNextTileType();
                }

                dungeon_crawler.core.globals.currentLevel.tiles.setSelectables();
                dungeon_crawler.main.setStage();

                dungeon_crawler.core.globals.eventBindings.unbindEvents();
                dungeon_crawler.core.globals.eventBindings.clearBoundEvents();

                dungeon_crawler.main.bindEvents();
            }
        }
    },

    //Tile select
    //  1 - 3:  Monster
    //  4, 5:   Empty
    //  6:      Loot
    getNextTileType() {
        //roll to see if tile populated
        let score = dungeon_crawler.main.roleSafeDie();

        switch (score) {
            case 1:
            case 2:
            case 3:
                return dungeon_crawler.main.selectMonster();
                break;
            case 6:
                return dungeon_crawler.main.selectLoot();
                break;
            case 4:
            case 5:
                return dungeon_crawler.core.globals.tileTypes['empty'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected tile table role "${score}"`);
        return dungeon_crawler.core.globals.tileTypes['unknown'];
    },

    //Monster difficulty selection
    selectMonster() {
        let score = dungeon_crawler.main.roleDangerDie();

        switch (score) {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
                //add ememy 
                return dungeon_crawler.core.globals.tileTypes['fight'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected monster table role "${score}"`);
        return dungeon_crawler.core.globals.tileTypes['unknown'];
    },

    //Loot select
    //  1 - 3:  Loot
    //  4 - 5:  Potion
    //  6:      Protection
    selectLoot() {
        let score = dungeon_crawler.main.roleAdditionalSafeDie();

        switch (score) {
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

        dungeon_crawler.core.outputError(`Unexpected loot table role "${score}"`);
        return dungeon_crawler.core.globals.tileTypes['unknown'];
    },

    //Dice
    roleSafeDie() {
        let value = dungeon_crawler.main.roleDSix();
        dungeon_crawler.main.setSafeDieValue(value);
        return value;
    },

    roleAdditionalSafeDie() {
        let value = dungeon_crawler.main.roleDSix();
        dungeon_crawler.main.setAdditionalSafeDieValue(value);
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

    setDetails() {
        $('#current-level').html(`${dungeon_crawler.core.globals.currentLevel.level} (${dungeon_crawler.core.globals.currentLevel.difficulty})`);
    },


    resetSafeDieValue() {
        dungeon_crawler.main.setSafeDieValue(0);
    },

    setSafeDieValue(value) {
        $('#current-dice-safe').html(value);
    },

    setAdditionalSafeDieValue(value) {
        $('#current-dice-safe').append(`, ${value}`);
    },

    resetDangerDieValue() {
        dungeon_crawler.main.setDangerDieValue(0);
    },

    setDangerDieValue(value) {
        $('#current-dice-danger').html(value);
    }
};

window['dungeon_crawler_main_startup'] = dungeon_crawler.main.startup;