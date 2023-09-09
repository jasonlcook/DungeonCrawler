﻿dungeon_crawler.weapon = {
    weaponType: dungeon_crawler.core.createEnum(['unknown', 'rock', 'club', 'dagger', 'mace', 'axe', 'sword']),
    weaponCondition: dungeon_crawler.core.createEnum(['unknown', 'broken', 'rusty', 'chipped', 'sharp', 'enchanted', 'flaming']),

    getWeapon() {
        let weponType = dungeon_crawler.weapon.selectWeaponType();
        let weponCondition = null;
        if (weponType !== dungeon_crawler.weapon.weaponType['rock'] && weponType !== dungeon_crawler.weapon.weaponType['club']) {
            weponCondition = dungeon_crawler.weapon.selectWeaponCondition();
        }

        let weaponValue = dungeon_crawler.weapon.getWeaponValue(weponType, weponCondition);
        let currentWeaponValue = dungeon_crawler.core.globals.adventurer.getWeapon();

        if (weaponValue > currentWeaponValue) {
            dungeon_crawler.core.globals.adventurer.setWeapon(weaponValue);
            dungeon_crawler.main.setWeaponUseText(weponType, weponCondition, weaponValue);

            dungeon_crawler.main.updateAdventurerDamage();
        } else {
            dungeon_crawler.main.setWeaponDiscardText(weponType, weponCondition, weaponValue);
        }
    },

    //Dice role
    //  Type
    //      1:      Rock
    //      2:      Club
    //      3:      Dagger
    //      4:      Mace
    //      5:      Axe
    //      6:      Sword
    selectWeaponType() {
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
                return dungeon_crawler.weapon.weaponType['rock'];
                break;
            case 2:
                return dungeon_crawler.weapon.weaponType['club'];
                break;
            case 3:
                return dungeon_crawler.weapon.weaponType['dagger'];
                break;
            case 4:
                return dungeon_crawler.weapon.weaponType['mace'];
                break;
            case 5:
                return dungeon_crawler.weapon.weaponType['axe'];
                break;
            case 6:
                return dungeon_crawler.weapon.weaponType['sword'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected wepon type role "${value}"`);
        return dungeon_crawler.adventurer.armour.armourType['unknown'];
    },

    //  Condition
    //      1:      Broken
    //      2:      Rusty
    //      3:      Chipped
    //      4:      Sharp
    //      5:      Enchanted
    //      6:      Flaming
    selectWeaponCondition() {
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
                return dungeon_crawler.weapon.weaponCondition['broken'];
                break;
            case 2:
                return dungeon_crawler.weapon.weaponCondition['rusty'];
                break;
            case 3:
                return dungeon_crawler.weapon.weaponCondition['chipped'];
                break;
            case 4:
                return dungeon_crawler.weapon.weaponCondition['sharp'];
                break;
            case 5:
                return dungeon_crawler.weapon.weaponCondition['enchanted'];
                break;
            case 6:
                return dungeon_crawler.weapon.weaponCondition['flaming'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected wepon condition role "${value}"`);
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
            case dungeon_crawler.weapon.weaponType['dagger']:
                weponTypeValue = 4;
                break;
            case dungeon_crawler.weapon.weaponType['mace']:
                weponTypeValue = 6;
                break;
            case dungeon_crawler.weapon.weaponType['axe']:
                weponTypeValue = 8;
                break;
            case dungeon_crawler.weapon.weaponType['sword']:
                weponTypeValue = 10;
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
                    weponConditionValue = 4;
                    break;
                case dungeon_crawler.weapon.weaponCondition['sharp']:
                    weponConditionValue = 6;
                    break;
                case dungeon_crawler.weapon.weaponCondition['enchanted']:
                    weponConditionValue = 8;
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