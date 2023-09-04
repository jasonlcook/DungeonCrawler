dungeon_crawler.main = {
    startup() {
        let hexagonHeight = dungeon_crawler.core.globals.hexHeight;
        let hexagonWidth = dungeon_crawler.core.globals.hexWidth;

        //set stage dimentions
        let stageHeight = dungeon_crawler.core.globals.stageCols * hexagonHeight;
        let stageWidth = dungeon_crawler.core.globals.stageRows * hexagonWidth;

        $('#stage').css({ 'height': `${stageHeight}px`, 'width': `${stageWidth}px` });

        //set board
        let hexagonTop, hexagonLeft;

        //middle
        hexagonLeft = (stageWidth / 2) - (hexagonWidth / 2);
        for (var i = 1; i < 11; i++) {
            hexagonTop = stageHeight - hexagonHeight * i;
            dungeon_crawler.main.drawHexagon(hexagonLeft, hexagonTop);

        }

        //left
        for (var i = 1; i < 10; i++) {
            hexagonTop = stageHeight - ((hexagonHeight / 2) + hexagonHeight * i);
            hexagonLeft = (stageWidth / 2) - (hexagonWidth + (hexagonWidth / 4));

            dungeon_crawler.main.drawHexagon(hexagonLeft, hexagonTop);
        }

        //right
        for (var i = 1; i < 10; i++) {
            hexagonTop = stageHeight - ((hexagonHeight / 2) + hexagonHeight * i);
            hexagonLeft = ((stageWidth / 2) + (hexagonWidth / 4));

            dungeon_crawler.main.drawHexagon(hexagonLeft, hexagonTop);
        }
    },

    drawHexagon(left, top) {
        $('#stage').append(`<div class="hexagon-tile" style="left: ${left}px; top: ${top}px"></div>`);
    }
};

window['dungeon_crawler_main_startup'] = dungeon_crawler.main.startup;