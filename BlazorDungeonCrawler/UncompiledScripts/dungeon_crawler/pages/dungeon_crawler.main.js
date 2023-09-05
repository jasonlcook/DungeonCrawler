dungeon_crawler.main = {
    startup() {
        let level = new Level();
        level.loadFirstLevel();

        dungeon_crawler.core.globals.currentLevel = level;

        dungeon_crawler.main.setTiles(level.stageCols, level.stageRows);

        dungeon_crawler.main.setStage();
    },

    setTiles(stageCols, stageRows) {
        let hexagonHeight = dungeon_crawler.core.globals.hexHeight;
        let hexagonWidth = dungeon_crawler.core.globals.hexWidth;

        //set stage dimentions
        //  height
        let stageHeight = stageCols * hexagonHeight;

        //  width
        let hexWidthQuaters = hexagonWidth / 4;
        let stageWidth = stageRows * (hexWidthQuaters * 3) + hexWidthQuaters;

        $('#stage').css({ 'height': `${stageHeight}px`, 'width': `${stageWidth}px` });

        //set board
        let hexagonLeft = 0, hexagonTop = 0, hexColumn = 0;

        //  due to the orientation of our board we miss one hex for every other grid row
        let tileCount = (stageCols * stageRows) - Math.ceil((stageRows + 1) / 2);

        dungeon_crawler.core.globals.currentLevel.tiles = new Tiles();

        hexagonTop -= hexagonHeight / 2;
        for (var i = 0; i < tileCount; i++) {
            hexagonTop += hexagonHeight;

            if (hexagonTop >= stageHeight - (hexagonWidth / 2)) {
                //move tile along one place
                hexagonLeft += (hexagonWidth / 4) * 3;

                //reset top
                if ((hexColumn % 2) == 1) {
                    hexagonTop = hexagonHeight - (hexagonHeight / 2);
                } else {
                    hexagonTop = 0;
                }

                //add column
                hexColumn += 1;
            }            

            dungeon_crawler.core.globals.currentLevel.tiles.add(new Tile(i, dungeon_crawler.core.globals.tileTypes[0], hexagonLeft, hexagonTop))
        }
    },

    setStage() {
        let tile, tiles = dungeon_crawler.core.globals.currentLevel.tiles;

        for (var i = 0; i < tiles.length; i++) {
            tile = tiles.get(i);

            $('#stage').append(`<div class="hexagon-tile" style="left: ${tile.X}px; top: ${tile.Y}px"><span>${tile.Index}</span></div>`);
        }
    }
};

window['dungeon_crawler_main_startup'] = dungeon_crawler.main.startup;