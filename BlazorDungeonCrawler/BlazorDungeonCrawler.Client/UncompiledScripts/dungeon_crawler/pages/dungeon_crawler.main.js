dungeon_crawler.main = {
    startup(args) {
        if (typeof args == 'undefined' || args == null) {
            dungeon_crawler.main.addErrorMessage("No args were passed to startup.");
            return false;
        }

        let properties = JSON.parse(args);
        dungeon_crawler.main.repositionHexGroups(properties['rows'], properties['columns']);

        return true;
    },

    addErrorMessage(message) {
        $('#messages').append(`<div class="message message-error">${message}</div>`);
    },

    addInformationMessage() {
        $('#messages').append(`<div class="message message-info">${message}</div>`);
    },

    repositionHexGroups(rows, columns) {
        let hexWidth = dungeon_crawler.core.hexWidth;
        let hexHeight = dungeon_crawler.core.hexHeight;

        let dungeonWidth = hexWidth * rows;
        let dungeonHeight = hexHeight * columns;

        let stageWidth = $('#dungeon').width();
        let stageHeight = $('#dungeon').height();

        let leftStart = (stageWidth / 2) - (dungeonWidth / 2);
        let topStart = (stageHeight / 2) - (dungeonHeight / 2);

        let hexGroupRow, hexGroupColumn, hexagonLeft, hexagonTop;
        $('.hexagon-tile-group').each(function (index) {
            let $hexGroup = $(this);

            hexagonLeft = leftStart;
            hexagonTop = topStart;

            hexGroupRow = $hexGroup.attr('data-row');
            hexGroupColumn = $hexGroup.attr('data-column');

            hexagonLeft += hexGroupColumn * (hexWidth / 4) * 3;

            if ((hexGroupColumn % 2) == 1) {
                //long
                hexagonTop += hexGroupRow * hexHeight;
            } else {
                //short
                hexagonTop += (hexGroupRow * hexHeight) + (hexHeight / 2);
            }

            $hexGroup.attr('style', `top: ${hexagonTop}px; left: ${hexagonLeft}px`);
        });
    }
};