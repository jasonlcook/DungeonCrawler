dungeon_crawler.armour = {
    armourType: dungeon_crawler.core.createEnum(['unknown', 'helmet', 'breastplate', 'gauntlet', 'greave', 'boots']),
    armourCondition: dungeon_crawler.core.createEnum(['unknown', 'rusty', 'tarnished', 'shiny']),

    getArmour() {
        let armourTypeValue = dungeon_crawler.main.roleDSix();
        let armourType = dungeon_crawler.armour.selectArmourType(armourTypeValue);

        let armourConditionValue = dungeon_crawler.main.roleDSix();
        let armourCondition = dungeon_crawler.armour.selectArmourCondition(armourConditionValue);

        let armourValue = dungeon_crawler.armour.getArmourValue(armourType, armourCondition);

        let currentArmourValue, keepArmour = false;

        switch (armourType) {
            case dungeon_crawler.armour.armourType['helmet']:
                currentArmourValue = dungeon_crawler.core.globals.adventurer.getArmourHelmet();

                if (armourValue > currentArmourValue) {
                    dungeon_crawler.core.globals.adventurer.setArmourHelmet(armourValue);
                    keepArmour = true;
                }

                break;
            case dungeon_crawler.armour.armourType['breastplate']:
                currentArmourValue = dungeon_crawler.core.globals.adventurer.getArmourBreastplate();

                if (armourValue > currentArmourValue) {
                    dungeon_crawler.core.globals.adventurer.setArmourBreastplate(armourValue);
                    keepArmour = true;
                }

                break;
            case dungeon_crawler.armour.armourType['gauntlet']:
                currentArmourValue = dungeon_crawler.core.globals.adventurer.getArmourGauntlet();

                if (armourValue > currentArmourValue) {
                    dungeon_crawler.core.globals.adventurer.setArmourGauntlet(armourValue);
                    keepArmour = true;
                }

                break;
            case dungeon_crawler.armour.armourType['greave']:
                currentArmourValue = dungeon_crawler.core.globals.adventurer.getArmourGreave();

                if (armourValue > currentArmourValue) {
                    dungeon_crawler.core.globals.adventurer.setArmourGreave(armourValue);
                    keepArmour = true;
                }

                break;
            case dungeon_crawler.armour.armourType['boots']:
                currentArmourValue = dungeon_crawler.core.globals.adventurer.getArmourBoots();

                if (armourValue > currentArmourValue) {
                    dungeon_crawler.core.globals.adventurer.setArmourBoots(armourValue);
                    keepArmour = true;
                }

                break;
            default:
                dungeon_crawler.core.outputError(`Unexpected armour type "${armourType}"`);
                armourTypevalue = 0;
                break;
        }

        if (keepArmour) {
            let logEntry = new LogEntry(dungeon_crawler.log_text.generateProtectionUseText(armourType, armourCondition));

            logEntry.addLogAction(new LogAction(0, `Armour type "${armourType}" (${armourTypeValue})`, [armourTypeValue]));
            logEntry.addLogAction(new LogAction(0, `Armour condition "${armourCondition}" (${armourConditionValue})`, [armourConditionValue]));

            dungeon_crawler.core.globals.logs.addEntry(logEntry);

            dungeon_crawler.main.updateAdventurerProtection();
        } else {
            let logEntry = new LogEntry(dungeon_crawler.log_text.generateProtectionDiscardText(armourType, armourCondition))

            dungeon_crawler.core.globals.logs.addEntry(logEntry);
        }
    },

    //Dice role
    //  Type
    //      Level 1 - 2
    //          1 - 5:  Greave
    //          6:      Boots

    //      Level 3 - 4
    //          1:      Greave
    //          2 - 5:  Boots
    //          6:      Gauntlet

    //      Level 5 +
    //          1:      Boots
    //          2 - 3:  Gauntlet
    //          4 - 5:  Helmet
    //          6:      Breastplate
    selectArmourType(value) {
        let dungeonLevel = dungeon_crawler.core.globals.currentLevel.getLevel();
        if (dungeonLevel > 4) {
            //Level 5 +
            switch (value) {
                case 1:
                    return dungeon_crawler.armour.armourType['boots'];
                    break;
                case 2:
                case 3:
                    return dungeon_crawler.armour.armourType['gauntlet'];
                    break;
                case 4:
                case 5:
                    return dungeon_crawler.armour.armourType['helmet'];
                    break;
                case 6:
                    return dungeon_crawler.armour.armourType['breastplate'];
                    break;
            }
        } else if (dungeonLevel > 2) {
            //Level 3 - 4
            switch (value) {
                case 1:
                    return dungeon_crawler.armour.armourType['greave'];
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                    return dungeon_crawler.armour.armourType['boots'];
                    break;
                case 6:
                    return dungeon_crawler.armour.armourType['gauntlet'];
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
                    return dungeon_crawler.armour.armourType['greave'];
                    break;
                case 6:
                    return dungeon_crawler.armour.armourType['boots'];
                    break;
            }
        }

        dungeon_crawler.core.outputError(`Unexpected armour type role "${value}" for level ${dungeonLevel}`);
        return dungeon_crawler.armour.armourType['unknown'];
    },

    //  Condition

    //    Level 1 - 4
    //        1 - 5:  Rusty
    //        6:      Tarnished

    //    Level 5 +
    //        1 - 2:  Rusty
    //        3 - 5:  Tarnished
    //        6:      Shiny
    selectArmourCondition(value) {
        let dungeonLevel = dungeon_crawler.core.globals.currentLevel.getLevel();
        if (dungeonLevel > 4) {
            switch (value) {
                case 1:
                    return dungeon_crawler.armour.armourCondition['rusty'];
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                    return dungeon_crawler.armour.armourCondition['tarnished'];
                    break;
                case 6:
                    return dungeon_crawler.armour.armourCondition['shiny'];
                    break;
            }
        } else {
            switch (value) {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    return dungeon_crawler.armour.armourCondition['rusty'];
                    break;
                case 6:
                    return dungeon_crawler.armour.armourCondition['tarnished'];
                    break;
            }
        }

        dungeon_crawler.core.outputError(`Unexpected armour condition role "${value}" for level ${dungeonLevel}`);
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
                armourConditionvalue = 1;
                break;
            case dungeon_crawler.armour.armourCondition['tarnished']:
                armourConditionvalue = 2;
                break;
            case dungeon_crawler.armour.armourCondition['shiny']:
                armourConditionvalue = 3;
                break;
            default:
                dungeon_crawler.core.outputError(`Unexpected armour condition "${armourCondition}"`);
                armourConditionvalue = 0;
                break;
        }

        return armourTypeValue * armourConditionvalue;
    }
}; 