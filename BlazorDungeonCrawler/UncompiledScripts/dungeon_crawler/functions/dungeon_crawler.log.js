dungeon_crawler.log = {
    generateStartingAdventurerText(healthValue, strengthValue, armourValue) {
        let healthLevel = dungeon_crawler.log.getStrengthText(healthValue);
        let strengthLevel = dungeon_crawler.log.getStrengthText(strengthValue);
        let armourLevel = dungeon_crawler.log.getArmourText(armourValue);

        return `You have been offerd as a sacrifices to the dungeon and are in ${healthLevel} health and feel ${strengthLevel} wearing your ${armourLevel}.`;
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
                return 'good';
                break;
            case 6:
                return 'excellent';
                break;
        }

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
    },

    getArmourText(armourValue) {
        switch (armourValue) {
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
    },

    //todo: use healt, strength and armour values in text
    generateMonsterEncounterText(adventurerInitiatesCombat, name, healthValue, strengthValue, armourValue) {
        //let healthLevel = dungeon_crawler.log.getHealthText(healthValue);
        //let strengthLevel = dungeon_crawler.log.getStrengthText(strengthValue);
        //let armourLevel = dungeon_crawler.log.getArmourText(armourValue);

        //return `You encounter a ${strengthLevel} ${name} wearing ${armourLevel} in ${healthLevel} health.`;

        let message = `A ${name} surprises you.`;
        if (adventurerInitiatesCombat) {
            message = `You surprise a ${name}.`;
        } 

        return message;
    },

    generateAdventurerAttackText(enemyType, adventurerRoll, adventurerStrength, adventurerAttackValue, enemyRoll, enemyArmour, enemyAvoidValue, wounds) {
        let message = `You attack the ${enemyType} with a <span style="font-weight: bold;">${adventurerAttackValue}</span> (${adventurerRoll} + ${adventurerStrength}). `;

        if (wounds != null) {
            message += `The ${enemyType} takes <span style="color:red;">${wounds}</span> damage after failing to avoid you with <span style="font-weight: bold;">${enemyAvoidValue}</span> (${enemyRoll} + ${enemyArmour}).`;
        } else {
            message += `The ${enemyType} avoids your attack with a <span style="font-weight: bold;">${enemyAvoidValue}</span> (${enemyRoll} + ${enemyArmour}).`;
        }

        return message;
    },

    generateAdventurerDeathText(enemyType) {
        return `You died to a ${enemyType}.`;
    },

    generateEnemyAttackText(enemyType, enemyRoll, enemyStrength, enemyAttackValue, adventurerRoll, adventurerArmour, adventurerAvoidValue, wounds) {
        let message = `The ${enemyType} attacks with a <span style="font-weight: bold;">${enemyAttackValue}</span> (${enemyRoll} + ${enemyStrength}). `;

        if (wounds != null) {
            message += `You takes <span style="color:red;">${wounds}</span> damage after failing to avoid with <span style="font-weight: bold;">${adventurerAvoidValue}</span> (${adventurerRoll} + ${adventurerArmour}).`;
        } else {
            message += `You avoid the ${enemyType} with a <span style="font-weight: bold;">${adventurerAvoidValue}</span> (${adventurerRoll} + ${adventurerArmour}).`;
        }

        return message;
    },

    generateEnemyDeathText(enemyType) {
        return `You killed the ${enemyType}.`;
    }
}; 