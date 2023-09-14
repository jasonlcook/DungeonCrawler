dungeon_crawler.weapon = {
    weaponType: dungeon_crawler.core.createEnum(['unknown', 'rock', 'club', 'mace', 'axe', 'sword']),
    weaponCondition: dungeon_crawler.core.createEnum(['unknown', 'broken', 'rusty', 'chipped', 'sharp', 'flaming']),

    getWeapon() {
        let weponTypeValue = dungeon_crawler.main.roleDSix();
        let weponType = dungeon_crawler.weapon.selectWeaponType(weponTypeValue);
        let weponCondition = null;
        let weponConditionValue = 0;

        if (weponType !== dungeon_crawler.weapon.weaponType['rock'] && weponType !== dungeon_crawler.weapon.weaponType['club']) {
            weponConditionValue = dungeon_crawler.main.roleDSix();
            weponCondition = dungeon_crawler.weapon.selectWeaponCondition(weponConditionValue);
        }

        let weaponValue = dungeon_crawler.weapon.getWeaponValue(weponType, weponCondition);
        let currentWeaponValue = dungeon_crawler.core.globals.adventurer.getWeapon();

        if (weaponValue > currentWeaponValue) {
            dungeon_crawler.core.globals.adventurer.setWeapon(weaponValue);

            let logEntry = new LogEntry(dungeon_crawler.log_text.generateWeaponValuenUseText(weponType, weponCondition));

            logEntry.addLogAction(new LogAction(0, `Wepon type "${weponType}" (${weponTypeValue})`, [weponTypeValue]));

            if (weponConditionValue > 0) {
                logEntry.addLogAction(new LogAction(0, `Wepon condition "${weponCondition}" (${weponConditionValue})`, [weponConditionValue]));
            }

            dungeon_crawler.core.globals.logs.addEntry(logEntry);

            dungeon_crawler.main.updateAdventurerDamage();
        } else {
            let logEntry = new LogEntry(dungeon_crawler.log_text.generateWeaponDiscardText(weponType, weponCondition));
            dungeon_crawler.core.globals.logs.addEntry(logEntry);
        }
    },

    //Dice role

    //  Type
    //      Level 1 - 2
    //          1 - 5:  Rock
    //          6:      Club

    //      Level 3 - 4
    //          1:      Rock
    //          2 - 5:  Club
    //          6:      Mace

    //      Level 5 +
    //          1:      Club
    //          2 - 3:  Mace
    //          4 - 5:  Axe
    //          6:      Sword
    selectWeaponType(value) {
        let dungeonLevel = dungeon_crawler.core.globals.currentLevel.getLevel();
        if (dungeonLevel > 4) {
            //Level 5 +
            switch (value) {
                case 1:
                    return dungeon_crawler.weapon.weaponType['club'];
                    break;
                case 2:
                case 3:
                    return dungeon_crawler.weapon.weaponType['mace'];
                    break;
                case 4:
                case 5:
                    return dungeon_crawler.weapon.weaponType['axe'];
                    break;
                case 6:
                    return dungeon_crawler.weapon.weaponType['sword'];
                    break;
            }
        } else if (dungeonLevel > 2) {
            //Level 3 - 4
            switch (value) {
                case 1:
                    return dungeon_crawler.weapon.weaponType['rock'];
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                    return dungeon_crawler.weapon.weaponType['club'];
                    break;
                case 6:
                    return dungeon_crawler.weapon.weaponType['mace'];
                    break;
            }
        } else {
            //      Level 1 - 2
            switch (value) {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    return dungeon_crawler.weapon.weaponType['rock'];
                    break;
                case 6:
                    return dungeon_crawler.weapon.weaponType['club'];
                    break;
            }
        }

        dungeon_crawler.core.outputError(`Unexpected wepon type role "${value}" for level ${dungeonLevel}`);
        return dungeon_crawler.adventurer.armour.armourType['unknown'];
    },

    //  Type
    //      Level 1 - 2
    //          1 - 5:  Broken
    //          6:      Rusty

    //      Level 3 - 4
    //          1:      Broken
    //          2 - 5:  Rusty
    //          6:      Chipped

    //      Level 5 +
    //          1:      Rusty
    //          2 - 3:  Chipped
    //          4 - 5:  Sharp
    //          6:      Flaming

    selectWeaponCondition(value) {
        let dungeonLevel = dungeon_crawler.core.globals.currentLevel.getLevel();
        if (dungeonLevel > 4) {
            //Level 5 +
            switch (value) {
                case 1:
                    return dungeon_crawler.weapon.weaponType['rusty'];
                    break;
                case 2:
                case 3:
                    return dungeon_crawler.weapon.weaponType['chipped'];
                    break;
                case 4:
                case 5:
                    return dungeon_crawler.weapon.weaponType['sharp'];
                    break;
                case 6:
                    return dungeon_crawler.weapon.weaponType['flaming'];
                    break;
            }
        } else if (dungeonLevel > 2) {
            //Level 3 - 4
            switch (value) {
                case 1:
                    return dungeon_crawler.weapon.weaponType['broken'];
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                    return dungeon_crawler.weapon.weaponType['rusty'];
                    break;
                case 6:
                    return dungeon_crawler.weapon.weaponType['chipped'];
                    break;
            }
        } else {
            //      Level 1 - 2
            switch (value) {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    return dungeon_crawler.weapon.weaponType['broken'];
                    break;
                case 6:
                    return dungeon_crawler.weapon.weaponType['rusty'];
                    break;
            }
        }

        dungeon_crawler.core.outputError(`Unexpected wepon condition role "${value}" for level ${dungeonLevel}`);
        return dungeon_crawler.adventurer.armour.armourCondition['unknown'];
    },

    //Value
    getWeaponValue(weponType, weponCondition) {
        let weponTypeValue = 0;
        switch (weponType) {
            case dungeon_crawler.weapon.weaponType['rock']:
                weponTypeValue = 1;
                break;
            case dungeon_crawler.weapon.weaponType['club']:
                weponTypeValue = 2;
                break;
            case dungeon_crawler.weapon.weaponType['mace']:
                weponTypeValue = 3;
                break;
            case dungeon_crawler.weapon.weaponType['axe']:
                weponTypeValue = 4;
                break;
            case dungeon_crawler.weapon.weaponType['sword']:
                weponTypeValue = 5;
                break;
            default:
                dungeon_crawler.core.outputError(`Unexpected wepon type "${weponType}"`);
                weponTypeValue = 0;
                break;
        }

        let weponConditionValue = 1;
        if (weponCondition != null) {
            switch (weponCondition) {
                case dungeon_crawler.weapon.weaponCondition['broken']:
                    weponConditionValue = 1;
                    break;
                case dungeon_crawler.weapon.weaponCondition['rusty']:
                    weponConditionValue = 2;
                    break;
                case dungeon_crawler.weapon.weaponCondition['chipped']:
                    weponConditionValue = 3;
                    break;
                case dungeon_crawler.weapon.weaponCondition['sharp']:
                    weponConditionValue = 4;
                    break;
                case dungeon_crawler.weapon.weaponCondition['flaming']:
                    weponConditionValue = 10;
                    break;
                default:
                    dungeon_crawler.core.outputError(`Unexpected wepon condition "${weponCondition}"`);
                    weponConditionValue = 0;
                    break;
            }
        }

        return weponTypeValue * weponConditionValue;
    }
}; 