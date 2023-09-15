dungeon_crawler.core.globals = {
    versionMajor: 0,
    versionMinor: 1,
    versionPatch: 3,

    //Consts
    hexHeight: 90,
    hexWidth: 100,

    tileTypes: dungeon_crawler.core.createEnum(['unknown', 'entrance', 'exit', 'stairs_ascending', 'stairs_descending', 'fight', 'fight_won', 'chest', 'protection', 'empty', 'potion', 'macguffin', 'adventurer_death']),
    
    //Vars
    stageHeight: null,
    stageWidth: null,

    eventBindings: null,

    logs: new Logs(),

    currentLevel: null,
    adventurer: null,
    currentEnemy: null,

    InCombat: false,

    macguffinFound: false,

    levels: [],
    lastLevel: 10
}; 