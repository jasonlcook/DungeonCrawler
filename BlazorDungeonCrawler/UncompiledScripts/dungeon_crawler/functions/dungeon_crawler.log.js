dungeon_crawler.log = {
    //  Story
    //      Adventurer
    generateStartingAdventurerText(healthValue, damageValue, protectionValue) {
        return `You have been offerd as a sacrifices to the dungeon and are in ${healthValue} health and feel ${damageValue} wearing your ${protectionValue}.`;
    },

    //          Weapon
    //              Pickup
    generateWeaponValuenUseText(type, condition, weaponValue) {
        let message = 'You equip a';

        if (condition !== null) {
            message += ` ${condition}`;
        }

        message += ` ${type} (${weaponValue}).`;

        return message;
    },

    //              Discard
    generateWeaponDiscardText(type, condition, weaponValue) {
        let message = 'You discard a';

        if (condition !== null) {
            message += ` ${condition}`;
        }

        message += ` ${type} (${weaponValue}).`;

        return message;
    },

    //          Armour
    //              Pickup
    generateProtectionUseText(type, condition, armourValue) {
        return `You equip a ${condition} ${type} (${armourValue}).`;
    },

    //              Discard
    generateProtectionDiscardText(type, condition, armourValue) {
        return `You discard a ${condition} ${type} (${armourValue}).`;
    },

    //          Potion
    //              Use
    generateUsePotionText(potionType, potionSize, potionDuration) {
        return `You drink a ${potionSize} ${potionDuration} duration ${potionType} potion.`;
    },

    //              Heal
    generatePotionHealingText(regainedHealth) {
        return `You regained ${regainedHealth} health points.`;
    },

    //          Combat
    generateAdventurerAttackText(enemyType, adventurerRoll, adventurerDamage, adventurerAttackValue, enemyRoll, enemyProtection, enemyAvoidValue, wounds) {
        let message = `You attack the ${enemyType} with a < span class="attack-text" > ${adventurerAttackValue}</span > (${adventurerRoll} + ${adventurerDamage}).`;

        if (wounds != null) {
            message += `The ${enemyType} takes < span class="wounds-text" > ${wounds}</span > damage after failing to avoid you with <span class="attack-text">${enemyAvoidValue}</span> (${enemyRoll} + ${enemyProtection}).`;
        } else {
            message += `The ${enemyType} avoids your attack with a < span class="attack-text" > ${enemyAvoidValue}</span > (${enemyRoll} + ${enemyProtection}).`;
        }

        return message;
    },

    //          Death
    generateAdventurerDeathText(enemyType) {
        return `You died to a ${enemyType}.`;
    },

    //      Stairs
    //          Down
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

    //          Up
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

    //      MacGuffin
    //          Pickup
    generateMacGuffinText() {
        return 'You found the thing.  Now good luck getting back out.';
    },

    //          Exit without
    generateExitWithoutMacGuffinText() {
        return 'You can not leave without the MacGuffin.';
    },

    //          Exit with
    generateExitWithMacGuffinText() {
        return 'As you leave with the MacGuffin the real prize is the possible friends we made along the way.';
    },

    //      Monster
    //          Combat
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

    //          Attack
    generateEnemyAttackText(enemyType, enemyRoll, enemyDamage, enemyAttackValue, adventurerRoll, adventurerProtection, adventurerAvoidValue, wounds) {
        let message = `The ${enemyType} attacks with a < span class="attack-text" > ${enemyAttackValue}</span > (${enemyRoll} + ${enemyDamage}).`;

        if (wounds != null) {
            message += `You takes < span class="wounds-text" > ${wounds}</span > damage after failing to avoid with <span class="attack-text">${adventurerAvoidValue}</span> (${adventurerRoll} + ${adventurerProtection}).`;
        } else {
            message += `You avoid the ${enemyType} with a < span class="attack-text" > ${adventurerAvoidValue}</span > (${adventurerRoll} + ${adventurerProtection}).`;
        }

        return message;
    },

    //          Death
    generateEnemyDeathText(enemyType) {
        return `You killed the ${enemyType}.`;
    }
}; 