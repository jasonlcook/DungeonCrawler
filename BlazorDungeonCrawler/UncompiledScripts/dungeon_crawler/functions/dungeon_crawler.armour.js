dungeon_crawler.armour = {
    armourType: dungeon_crawler.core.createEnum(['unknown', 'helmet', 'breastplate', 'vambrace', 'gauntlet', 'greave', 'boots']),
    armourCondition: dungeon_crawler.core.createEnum(['unknown', 'rusty', 'tarnished', 'shiny']),

    //Dice role
    //  Type
    //      1:      Boots
    //      2:      Greave
    //      3:      Vambrace
    //      4:      Gauntlet
    //      5:      Helmet
    //      6:      Breastplate
    selectArmourType() {
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
                return dungeon_crawler.armour.armourType['boots'];
                break;
            case 2:
                return dungeon_crawler.armour.armourType['greave'];
                break;
            case 3:
                return dungeon_crawler.armour.armourType['vambrace'];
                break;
            case 4:
                return dungeon_crawler.armour.armourType['gauntlet'];
                break;
            case 5:
                return dungeon_crawler.armour.armourType['helmet'];
                break;
            case 6:
                return dungeon_crawler.armour.armourType['breastplate'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected armour type role "${value}"`);
        return dungeon_crawler.armour.armourType['unknown'];
    },

    //  Condition
    //      1 - 2:  Rusty
    //      3 - 5:  Tarnished
    //      6:      Shiny
    selectArmourCondition() {
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
            case 2:
                return dungeon_crawler.armour.armourCondition['rusty'];
                break;
            case 3:
            case 4:
            case 5:
                return dungeon_crawler.armour.armourCondition['tarnished'];
                break;
            case 6:
                return dungeon_crawler.armour.armourCondition['shiny'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected armour condition role "${value}"`);
        return dungeon_crawler.armour.armourCondition['unknown'];
    },

    //Value
    getArmourValue(armourType, armourCondition) {
        let armourTypeValue = 0;
        switch (armourType) {
            case dungeon_crawler.armour.armourType['helmet']:
                armourTypeValue = 4;
                break;
            case dungeon_crawler.armour.armourType['breastplate']:
                armourTypeValue = 4;
                break;
            case dungeon_crawler.armour.armourType['vambrace']:
                armourTypeValue = 2;
                break;
            case dungeon_crawler.armour.armourType['gauntlet']:
                armourTypeValue = 3;
                break;
            case dungeon_crawler.armour.armourType['greave']:
                armourTypeValue = 1;
                break;
            case dungeon_crawler.armour.armourType['boots']:
                armourTypeValue = 1;
                break;
            default:
                dungeon_crawler.core.outputError(`Unexpected armour type "${armourType}"`);
                armourTypeValue = 0;
                break;
        }

        let armourConditionvalue = 0;
        switch (armourCondition) {
            case dungeon_crawler.armour.armourCondition['rusty']:
                armourConditionvalue = 2;
                break;
            case dungeon_crawler.armour.armourCondition['tarnished']:
                armourConditionvalue = 3;
                break;
            case dungeon_crawler.armour.armourCondition['shiny']:
                armourConditionvalue = 4;
                break;
            default:
                dungeon_crawler.core.outputError(`Unexpected armour condition "${armourCondition}"`);
                armourConditionvalue = 0;
                break;
        }

        return armourTypeValue * armourConditionvalue;
    }
}; 