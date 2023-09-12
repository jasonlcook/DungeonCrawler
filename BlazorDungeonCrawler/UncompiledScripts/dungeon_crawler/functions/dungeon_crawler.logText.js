dungeon_crawler.log_text = {
    //  Story
    //      Adventurer
    generateStartingAdventurerText(healthValue, damageValue, protectionValue) {
        return `You have been offerd as a sacrifices to the dungeon and are in ${healthValue} health and feel ${damageValue} wearing your ${protectionValue}.`;
    },

    //      Combat
    generateMonsterEncounterText(adventurerInitiatesCombat, name) {
        let message = `A ${name} surprises you.`;
        if (adventurerInitiatesCombat) {
            message = `You surprise a ${name}.`;
        }

        return message;
    },

    //      Tile
    generateTileText(nextTileType) {
        return `INSERT TILE TYPE (${nextTileType})`;
    },

    //          Weapon
    generateLootWeaponText() {
        return 'You find a weapon';
    },

    //              Pickup
    generateWeaponValuenUseText(type, condition) {
        let message = 'You equip a';

        if (condition !== null) {
            message += ` ${condition}`;
        }

        message += ` ${type}.`;

        return message;
    },

    //              Discard
    generateWeaponDiscardText(type, condition) {
        let message = 'You discard a';

        if (condition !== null) {
            message += ` ${condition}`;
        }

        message += ` ${type}.`;

        return message;
    },

    //          Armour
    generateLootArmourText() {
        return 'You find some armour';
    },

    //              Pickup
    generateProtectionUseText(type, condition) {
        return `You equip a ${condition} ${type}.`;
    },

    //              Discard
    generateProtectionDiscardText(type, condition) {
        return `You discard a ${condition} ${type}.`;
    },

    //          Potion
    generateLootPotionText() {
        return 'You find a potion';
    },

    //              Use
    generateUsePotionText(potionType, potionSize, potionDuration) {
        return `You drink a ${potionSize} ${potionDuration} duration ${potionType} potion.`;
    },

    //              Heal
    generatePotionHealingText(regainedHealth) {
        return `You regained ${regainedHealth} health points.`;
    },

    //          Combat
    generateAdventurerAttackText(enemyType, adventurerRoll, adventurerDamage, adventurerAttackValue, enemyRoll, enemyProtection, enemyAvoidValue, wounds, enemyHealth) {
        let message = `You attack the ${enemyType} with ${adventurerAttackValue} `;
        if (wounds != null) {
            if (enemyHealth > 0) {
                message += `they takes ${wounds} wounds leaving them with ${enemyHealth} health`;
            } else {
                message += `they takes ${wounds} wounds and die`;
            }
        } else {
            message += `they avoid you`;
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
    //          Attack
    generateEnemyAttackText(enemyType, enemyRoll, enemyDamage, enemyAttackValue, adventurerRoll, adventurerProtection, adventurerAvoidValue, wounds, adventurerHealth) {
        let message = `The ${enemyType} attacks you with ${enemyAttackValue} `;
        if (wounds != null) {
            if (adventurerHealth > 0) {
                message += `they inflict ${wounds} wounds leaving you with ${adventurerHealth} health`;
            } else {
                message += `they inflict ${wounds} wounds and you die`;
            }
        } else {
            message += `they avoid you`;
        }

        return message;
    },

    //          Death
    generateEnemyDeathText(enemyType) {
        return `You killed the ${enemyType}.`;
    }
}; 