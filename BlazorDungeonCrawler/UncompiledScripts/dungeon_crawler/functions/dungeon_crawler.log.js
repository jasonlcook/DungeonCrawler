dungeon_crawler.log = {
    generateStartingAdventurerText(healthValue, strengthValue, protectionValue) {
        let healthLevel = dungeon_crawler.log.getHealthText(healthValue);
        let strengthLevel = dungeon_crawler.log.getStrengthText(strengthValue);
        let protectionLevel = dungeon_crawler.log.getProtectionText(protectionValue);

        return `You have been offerd as a sacrifices to the dungeon and are in ${healthLevel} health and feel ${strengthLevel} wearing your ${protectionLevel}.`;
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

    getStrengthText(strengthValue) {
        switch (strengthValue) {
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

        dungeon_crawler.core.outputError(`Generate strength text error with value ${strengthValue}`);
    },

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
                return 'armour';
                break;
        }

        dungeon_crawler.core.outputError(`Generate protection text error with value ${protectionValue}`);
    },

    generateUsePotionText(potionType, potionSize, potionDuration) {
        return `You drink a ${potionSize} ${potionDuration} duration ${potionType} potion.`;
    },

    generatePotionHealingText(regainedHealth) {
        return `You regained ${regainedHealth} health points.`;
    },

    //todo: use healt, strength and protection values in text
    generateMonsterEncounterText(adventurerInitiatesCombat, name, healthValue, strengthValue, protectionValue) {
        //let healthLevel = dungeon_crawler.log.getHealthText(healthValue);
        //let strengthLevel = dungeon_crawler.log.getStrengthText(strengthValue);
        //let protectionLevel = dungeon_crawler.log.getProtectionText(protectionValue);

        //return `You encounter a ${strengthLevel} ${name} wearing ${protectionLevel} in ${healthLevel} health.`;

        let message = `A ${name} surprises you.`;
        if (adventurerInitiatesCombat) {
            message = `You surprise a ${name}.`;
        } 

        return message;
    },

    generateAdventurerAttackText(enemyType, adventurerRoll, adventurerStrength, adventurerAttackValue, enemyRoll, enemyProtection, enemyAvoidValue, wounds) {
        let message = `You attack the ${enemyType} with a <span style="font-weight: bold;">${adventurerAttackValue}</span> (${adventurerRoll} + ${adventurerStrength}). `;

        if (wounds != null) {
            message += `The ${enemyType} takes <span style="color:red;">${wounds}</span> damage after failing to avoid you with <span style="font-weight: bold;">${enemyAvoidValue}</span> (${enemyRoll} + ${enemyProtection}).`;
        } else {
            message += `The ${enemyType} avoids your attack with a <span style="font-weight: bold;">${enemyAvoidValue}</span> (${enemyRoll} + ${enemyProtection}).`;
        }

        return message;
    },

    generateAdventurerDeathText(enemyType) {
        return `You died to a ${enemyType}.`;
    },

    generateEnemyAttackText(enemyType, enemyRoll, enemyStrength, enemyAttackValue, adventurerRoll, adventurerProtection, adventurerAvoidValue, wounds) {
        let message = `The ${enemyType} attacks with a <span style="font-weight: bold;">${enemyAttackValue}</span> (${enemyRoll} + ${enemyStrength}). `;

        if (wounds != null) {
            message += `You takes <span style="color:red;">${wounds}</span> damage after failing to avoid with <span style="font-weight: bold;">${adventurerAvoidValue}</span> (${adventurerRoll} + ${adventurerProtection}).`;
        } else {
            message += `You avoid the ${enemyType} with a <span style="font-weight: bold;">${adventurerAvoidValue}</span> (${adventurerRoll} + ${adventurerProtection}).`;
        }

        return message;
    },

    generateEnemyDeathText(enemyType) {
        return `You killed the ${enemyType}.`;
    }
}; 