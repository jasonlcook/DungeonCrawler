dungeon_crawler.main = {
    startup() {
        let hexagonHeight = dungeon_crawler.core.globals.hexHeight;
        let hexagonWidth = dungeon_crawler.core.globals.hexWidth;

        let stageCols = dungeon_crawler.core.globals.stageCols;
        let stageRows = dungeon_crawler.core.globals.stageRows;

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

            dungeon_crawler.main.drawHexagon(i, hexagonLeft, hexagonTop);
        }
    },

    drawHexagon(index, left, top) {
        $('#stage').append(`<div class="hexagon-tile" style="left: ${left}px; top: ${top}px"><span>${index}</span></div>`);
    }
};

window['dungeon_crawler_main_startup'] = dungeon_crawler.main.startup;