dungeon_crawler.potion = {
    potionType: dungeon_crawler.core.createEnum(['unknown', 'aura', 'damage', 'sheild']),
    potionSize: dungeon_crawler.core.createEnum(['unknown', 'vial', 'flask', 'bottle']),
    potionDuration: dungeon_crawler.core.createEnum(['unknown', 'short', 'medium', 'long']),

    getPotion() {
        let potionTypeValue = dungeon_crawler.main.roleDSix();
        let potionType = dungeon_crawler.potion.selectPotionType(potionTypeValue);

        let potionSizeValue = dungeon_crawler.main.roleDSix();
        let potionSize = dungeon_crawler.potion.selectPotionSize(potionSizeValue);

        let potionDurationValue = dungeon_crawler.main.roleDSix();
        let potionDuration = dungeon_crawler.potion.selectPotionDuration(potionDurationValue);

        dungeon_crawler.potion.usePotion(potionType, potionSize, potionDuration);

        let logEntry = new LogEntry(dungeon_crawler.log_text.generateUsePotionText(potionType, potionSize, potionDuration));
        logEntry.addLogAction(new LogAction(0, `Potion type "${potionType}" (${potionTypeValue})`, [potionTypeValue]));
        logEntry.addLogAction(new LogAction(0, `Potion size "${potionSize}" (${potionSizeValue})`, [potionSizeValue]));
        logEntry.addLogAction(new LogAction(0, `Potion duration "${potionDuration}" (${potionDurationValue})`, [potionDurationValue]));

        dungeon_crawler.core.globals.logs.addEntry(logEntry);
    },

    //Dice role
    //  Type
    //      1 - 2:  Sheild (Protection)
    //      3 - 4:  Damage
    //      5 - 6:  Aura (Health)
    selectPotionType(value) {
        switch (value) {
            case 1:
            case 2:
                return dungeon_crawler.potion.potionType['sheild'];
                break;
            case 3:
            case 4:
                return dungeon_crawler.potion.potionType['damage'];
                break;
            case 5:
            case 6:
                return dungeon_crawler.potion.potionType['aura'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected potion table role "${value}"`);
        return dungeon_crawler.potion.potionType['unknown'];
    },

    //  Type
    //      Level 1 - 4
    //          1 - 5:  Vial
    //          6:      Flask

    //      Level 5 +
    //          1:      Vial
    //          2 - 5:  Flask
    //          6:      Bottle
    selectPotionSize(value) {
        let dungeonLevel = dungeon_crawler.core.globals.currentLevel.getLevel();
        if (dungeonLevel > 4) {
            //Level 5 +
            switch (value) {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    return dungeon_crawler.weapon.weaponType['vial'];
                    break;
                case 6:
                    return dungeon_crawler.weapon.weaponType['flask'];
                    break;
            }
        } else {
            switch (value) {
                case 1:
                    return dungeon_crawler.potion.potionSize['vial'];
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                    return dungeon_crawler.potion.potionSize['flask'];
                    break;
                case 6:
                    return dungeon_crawler.potion.potionSize['bottle'];
                    break;
            }
        }

        dungeon_crawler.core.outputError(`Unexpected potion size role "${value}" for level ${dungeonLevel}`);
        return dungeon_crawler.potion.potionType['unknown'];
    },


    //  Duration
    //      1 - 2:  Short
    //      3 - 4:  Medium
    //      5 - 6:  Long
    selectPotionDuration(value) {
        switch (value) {
            case 1:
            case 2:
                return dungeon_crawler.potion.potionDuration['short'];
                break;
            case 3:
            case 4:
                return dungeon_crawler.potion.potionDuration['medium'];
                break;
            case 5:
            case 6:
                return dungeon_crawler.potion.potionDuration['long'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected potion duration role "${value}"`);
        return dungeon_crawler.potion.potionDuration['unknown'];
    },

    //Value
    usePotion(potionType, potionSize, potionDuration) {
        let sizeValue = 0;
        switch (potionSize) {
            case dungeon_crawler.potion.potionSize['vial']:
                sizeValue = 6;
                break;
            case dungeon_crawler.potion.potionSize['flask']:
                sizeValue = 12;
                break;
            case dungeon_crawler.potion.potionSize['bottle']:
                sizeValue = 18;
                break;
            default:
                dungeon_crawler.core.outputError(`Unexpected potion size "${potionSize}"`);
                sizeValue = 0;
                break;
        }

        let durationValue = 0;
        switch (potionDuration) {
            case dungeon_crawler.potion.potionDuration['short']:
                durationValue = 10;
                break;
            case dungeon_crawler.potion.potionDuration['medium']:
                durationValue = 20;
                break;
            case dungeon_crawler.potion.potionDuration['long']:
                durationValue = 30;
                break;
            default:
                dungeon_crawler.core.outputError(`Unexpected potion duration "${potionDuration}"`);
                durationValue = 0;
                break;
        }

        switch (potionType) {
            case dungeon_crawler.potion.potionType['aura']:
                let regainedHealth = dungeon_crawler.core.globals.adventurer.setAuraPotion(sizeValue);
                if (regainedHealth > 0) {
                    let logEntry = new LogEntry(dungeon_crawler.log_text.generatePotionHealingText(regainedHealth));
                    dungeon_crawler.core.globals.logs.addEntry(logEntry);
                }

                dungeon_crawler.core.globals.adventurer.setAuraPotionDuration(durationValue);
                dungeon_crawler.main.updateAdventurerHealth();
                break;
            case dungeon_crawler.potion.potionType['damage']:
                dungeon_crawler.core.globals.adventurer.setDamagePotion(sizeValue);
                dungeon_crawler.core.globals.adventurer.setDamagePotionDuration(durationValue);
                dungeon_crawler.main.updateAdventurerDamage();
                break;
            case dungeon_crawler.potion.potionType['sheild']:
                dungeon_crawler.core.globals.adventurer.setShieldPotion(sizeValue);
                dungeon_crawler.core.globals.adventurer.setShieldPotionDuration(durationValue);
                dungeon_crawler.main.updateAdventurerProtection();
                break;
            default:
                dungeon_crawler.core.outputError(`Unexpected potion type "${potionType}"`);
                durationValue = 0;
                break;
        }
    }
}; 