dungeon_crawler.log = {
    generateStartingAdventurerText(healthValue, damageValue, protectionValue) {
        let healthLevel = dungeon_crawler.log.getHealthText(healthValue);
        let damageLevel = dungeon_crawler.log.getDamageText(damageValue);
        let protectionLevel = dungeon_crawler.log.getProtectionText(protectionValue);

        return `You have been offerd as a sacrifices to the dungeon and are in ${healthLevel} health and feel ${damageLevel} wearing your ${protectionLevel}.`;
    },

    //todo: embiggenate text
    generateStairsDownText(level) {
        switch (level) {
            case 2:
                return 'You descend to the second level';
                break;
            case 3:
                return 'You descend to the third level';
                break;
            case 4:
                return 'You descend to the fourth level';
                break;
            case 5:
                return 'You descend to the fith level';
                break;
            case 6:
                return 'You descend to the sixth level';
                break;
            case 7:
                return 'You descend to the sevent level';
                break;
            case 8:
                return 'You descend to the eigth level';
                break;
            case 9:
                return 'You descend to the ninth level';
                break;
            case 10:
                return 'You descend to the final level';
                break;
        }

        dungeon_crawler.core.outputError(`Generate stairs descend text error with value ${level}`);
    },

    //todo: embiggenate text
    generateStairsUpText(level) {
        switch (level) {
            case 1:
                return 'You ascend to the first level';
                break;
            case 2:
                return 'You ascend to the second level';
                break;
            case 3:
                return 'You ascend to the third level';
                break;
            case 4:
                return 'You ascend to the fourth level';
                break;
            case 5:
                return 'You ascend to the fith level';
                break;
            case 7:
                return 'You ascend to the sixth level';
                break;
            case 8:
                return 'You ascend to the sevent level';
                break;
            case 9:
                return 'You ascend to the eigth level';
                break;
            case 10:
                return 'You ascend to the ninth level';
                break;
        }

        dungeon_crawler.core.outputError(`Generate stairs ascend text error with value ${level}`);
    },

    //todo: embiggenate text
    generateMacGuffinText() {
        return 'You found the thing.  Now good luck getting back out.';
    },

    //todo: embiggenate text
    generateExitWithoutMacGuffinText() {
        return 'You can not leave without the MacGuffin.';
    },

    //todo: embiggenate text
    generateExitWithMacGuffinText() {
        return 'As you leave with the MacGuffin the real prize is the possible friends we made along the way.';
    },

    //todo: move this to main
    getHealthText(healthValue) {
        switch (healthValue) {
            case 1:
            case 2:
            case 3:
                return 'poor';
                break;
            case 4:
            case 5:
            case 6:
            case 7:
                return 'good';
                break;
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
                return 'excellent';
                break;
        }

        dungeon_crawler.core.outputError(`Generate health text error with value ${healthValue}`);
    },

    //todo: move this to main
    getDamageText(damageValue) {
        switch (damageValue) {
            case 1:
            case 2:
            case 3:
                return 'vulnerable';
                break;
            case 4:
            case 5:
                return 'fine';
                break;
            case 6:
                return 'powerful';
                break;
        }

        dungeon_crawler.core.outputError(`Generate damage text error with value ${damageValue}`);
    },

    //todo: move this to main
    getProtectionText(protectionValue) {
        switch (protectionValue) {
            case 1:
            case 2:
            case 3:
                return 'rags';
                break;
            case 4:
            case 5:
                return 'clothes';
                break;
            case 6:
                //todo: add random armour parts
                return 'armour';
                break;
        }

        dungeon_crawler.core.outputError(`Generate protection text error with value ${protectionValue}`);
    },

    generateUsePotionText(potionType, potionSize, potionDuration) {
        return `You drink a ${potionSize} ${potionDuration} duration ${potionType} potion.`;
    },

    generateWeaponValuenUseText(type, condition, weaponValue) {
        let message = 'You equip a';

        if (condition !== null) {
            message += ` ${condition}`;
        }

        message += ` ${type} (${weaponValue}).`;

        return message;
    },

    generateWeaponDiscardText(type, condition, weaponValue) {
        let message = 'You discard a';

        if (condition !== null) {
            message += ` ${condition}`;
        }

        message += ` ${type} (${weaponValue}).`;

        return message;

    },

    generatePotionHealingText(regainedHealth) {
        return `You regained ${regainedHealth} health points.`;
    },

    generateProtectionUseText(type, condition, armourValue) {
        return `You equip a ${condition} ${type} (${armourValue}).`;
    },

    generateProtectionDiscardText(type, condition, armourValue) {
        return `You discard a ${condition} ${type} (${armourValue}).`;
    },

    //todo: use healt, damage and protection values in text
    generateMonsterEncounterText(adventurerInitiatesCombat, name, healthValue, damageValue, protectionValue) {
        //let healthLevel = dungeon_crawler.log.getHealthText(healthValue);
        //let damageLevel = dungeon_crawler.log.getDamageText(damageValue);
        //let protectionLevel = dungeon_crawler.log.getProtectionText(protectionValue);

        //return `You encounter a ${ damageLevel } ${ name } wearing ${ protectionLevel } in ${ healthLevel } health.`;

        let message = `A ${name} surprises you.`;
        if (adventurerInitiatesCombat) {
            message = `You surprise a ${name}.`;
        }

        return message;
    },

    generateAdventurerAttackText(enemyType, adventurerRoll, adventurerDamage, adventurerAttackValue, enemyRoll, enemyProtection, enemyAvoidValue, wounds) {
        let message = `You attack the ${enemyType} with a < span class="attack-text" > ${adventurerAttackValue}</span > (${adventurerRoll} + ${adventurerDamage}).`;

        if (wounds != null) {
            message += `The ${enemyType} takes < span class="wounds-text" > ${wounds}</span > damage after failing to avoid you with <span class="attack-text">${enemyAvoidValue}</span> (${enemyRoll} + ${enemyProtection}).`;
        } else {
            message += `The ${enemyType} avoids your attack with a < span class="attack-text" > ${enemyAvoidValue}</span > (${enemyRoll} + ${enemyProtection}).`;
        }

        return message;
    },

    generateAdventurerDeathText(enemyType) {
        return `You died to a ${enemyType}.`;
    },

    generateEnemyAttackText(enemyType, enemyRoll, enemyDamage, enemyAttackValue, adventurerRoll, adventurerProtection, adventurerAvoidValue, wounds) {
        let message = `The ${enemyType} attacks with a < span class="attack-text" > ${enemyAttackValue}</span > (${enemyRoll} + ${enemyDamage}).`;

        if (wounds != null) {
            message += `You takes < span class="wounds-text" > ${wounds}</span > damage after failing to avoid with <span class="attack-text">${adventurerAvoidValue}</span> (${adventurerRoll} + ${adventurerProtection}).`;
        } else {
            message += `You avoid the ${enemyType} with a < span class="attack-text" > ${adventurerAvoidValue}</span > (${adventurerRoll} + ${adventurerProtection}).`;
        }

        return message;
    },

    generateEnemyDeathText(enemyType) {
        return `You killed the ${enemyType}.`;
    }
}; 