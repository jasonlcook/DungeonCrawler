dungeon_crawler.main = {
    startup() {
        console.log("Hello from startup");

        dungeon_crawler.main.repositionHexGroups();
    },

    repositionHexGroups() {
        $('.hexagon-tile-group').each(function (index) {
            debugger;
        });
    } 
};