dungeon_crawler.core.globals = {
    //Consts
    hexHeight: 90,
    hexWidth: 100,

    tileTypes: dungeon_crawler.core.createEnum(['unknown', 'entrance', 'exit', 'stairs_ascending', 'stairs_descending', 'fight', 'loot', 'protection', 'empty', 'potion']),

    //Vars
    stageHeight: null,
    stageWidth: null,

    eventBindings: null,
}; 