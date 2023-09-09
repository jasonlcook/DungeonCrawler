dungeon_crawler.core.globals = {
    //Consts
    hexHeight: 90,
    hexWidth: 100,

    tileTypes: dungeon_crawler.core.createEnum(['unknown', 'entrance', 'exit', 'stairs_ascending', 'stairs_descending', 'fight', 'chest', 'protection', 'empty', 'potion', 'macguffin']),

    armourType: dungeon_crawler.core.createEnum(['unknown', 'helmet', 'breastplate', 'vambrace', 'gauntlet', 'greave', 'boots']),
    armourCondition: dungeon_crawler.core.createEnum(['unknown', 'rusty', 'tarnished', 'shiny']),

    potionType: dungeon_crawler.core.createEnum(['unknown', 'aura', 'damage', 'sheild']),
    potionSize: dungeon_crawler.core.createEnum(['unknown', 'vial', 'flask', 'bottle']),
    potionDuration: dungeon_crawler.core.createEnum(['unknown', 'short', 'medium', 'long']),

    //Vars
    stageHeight: null,
    stageWidth: null,

    eventBindings: null,

    currentLevel: null,
    adventurer: null,
    currentEnemy: null,

    InCombat: false,

    macguffinFound: false,

    levels: [],
    lastLevel: 10
}; 