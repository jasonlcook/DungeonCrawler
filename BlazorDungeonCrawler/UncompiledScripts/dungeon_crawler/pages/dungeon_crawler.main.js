dungeon_crawler.main = {
    startup() {
        let level = new Level();
        level.loadFirstLevel();

        dungeon_crawler.core.globals.currentLevel = level;

        dungeon_crawler.main.setTiles(level.stageCols, level.stageRows);

        dungeon_crawler.core.globals.currentLevel.setSpawn();

        dungeon_crawler.core.globals.currentLevel.tiles.setSelectables();

        dungeon_crawler.main.setStage();

        dungeon_crawler.main.bindEvents();
    },

    bindEvents() {
        if (typeof dungeon_crawler.core.globals.eventBindings == 'undefined' || dungeon_crawler.core.globals.eventBindings == null) {
            dungeon_crawler.core.globals.eventBindings = new EventBindings();
        }

        let dispacter, type, handler, name;

        dispacter = $('#stage .hexagon-tile');
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
                //deselect previous tile
                let previousIndex = dungeon_crawler.core.globals.currentLevel.tiles.currentIndex;
                let previousTile = dungeon_crawler.core.globals.currentLevel.tiles.get(previousIndex);
                previousTile.Current = false;

                dungeon_crawler.core.globals.currentLevel.tiles.currentIndex = selectedTile.Index;
                selectedTile.Current = true;
                selectedTile.Hidden = false;
                selectedTile.Type = dungeon_crawler.main.getNextTileType();

                dungeon_crawler.core.globals.currentLevel.tiles.setSelectables();
                dungeon_crawler.main.setStage();

                dungeon_crawler.core.globals.eventBindings.unbindEvents();
                dungeon_crawler.core.globals.eventBindings.clearBoundEvents();

                dungeon_crawler.main.bindEvents();
            }
        }
    },

    getNextTileType() {
        return dungeon_crawler.core.globals.tileTypes['unknown'];
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

            dungeon_crawler.core.globals.currentLevel.tiles.add(new Tile(i, dungeon_crawler.core.globals.tileTypes[0], hexRow, hexColumn, hexagonLeft, hexagonTop))
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
    }
};

window['dungeon_crawler_main_startup'] = dungeon_crawler.main.startup;