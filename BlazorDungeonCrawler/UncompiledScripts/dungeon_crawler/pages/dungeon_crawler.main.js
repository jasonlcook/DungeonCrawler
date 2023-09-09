dungeon_crawler.main = {
    startup() {
        //generate adventurer
        dungeon_crawler.main.generateAdventurer();
        dungeon_crawler.main.setAdventurerDetails();

        dungeon_crawler.main.generateLevel(1);
    },

    generateLevel(value) {
        let currentVisitedLevel = null;
        let visitedLevel, visitedLevels = dungeon_crawler.core.globals.levels;
        for (var i = 0; i < visitedLevels.length; i++) {
            visitedLevel = visitedLevels[i];
            if (visitedLevel.level == value) {
                currentVisitedLevel = visitedLevel;
            }
        }

        let level;
        if (currentVisitedLevel == null) {
            level = new Level();
            level.loadLevel(value);

            dungeon_crawler.core.globals.levels.push(level);
            dungeon_crawler.core.globals.currentLevel = level;

            //set tiles
            dungeon_crawler.main.setTiles(level.stageCols, level.stageRows);

            //set spawn location
            dungeon_crawler.core.globals.currentLevel.setSpawn(level.level);
        } else {
            dungeon_crawler.core.globals.currentLevel = currentVisitedLevel;
            level = currentVisitedLevel;
        }

        dungeon_crawler.main.setLevelDetails();

        dungeon_crawler.core.globals.currentLevel.tiles.setSelectables();

        dungeon_crawler.main.setStage();

        //
        if (dungeon_crawler.core.globals.eventBindings != null) {
            dungeon_crawler.core.globals.eventBindings.unbindEvents();
            dungeon_crawler.core.globals.eventBindings.clearBoundEvents();
        }

        dungeon_crawler.main.bindEvents();
    },

    bindEvents() {
        if (typeof dungeon_crawler.core.globals.eventBindings == 'undefined' || dungeon_crawler.core.globals.eventBindings == null) {
            dungeon_crawler.core.globals.eventBindings = new EventBindings();
        }

        let dispacter, type, handler, name;

        dispacter = $('#stage .hexagon-tile span');
        type = 'click';
        handler = dungeon_crawler.main.tileClick;
        name = 'tile_click';

        dungeon_crawler.core.globals.eventBindings.addEventBinding(dispacter, type, handler, name);

        dungeon_crawler.core.globals.eventBindings.bindEvents();
    },

    tileClick(event) {
        if (event !== null && typeof event.target !== 'undefined' || event.target !== null) {
            let id = $(event.target.parentElement).attr('data-identity');
            let selectedTile = dungeon_crawler.core.globals.currentLevel.tiles.getById(id);

            if (selectedTile.Selectable) {
                dungeon_crawler.main.movement(selectedTile);

                if (dungeon_crawler.core.globals.InCombat) {
                    dungeon_crawler.main.combat();
                }
            }
        }
    },

    movement(selectedTile) {
        //Potion effect
        //  Both attempt to decorment and retun true if it was needed
        if (dungeon_crawler.core.globals.adventurer.decrementAuraPotionDuration()) {
            dungeon_crawler.main.updateAdventurerHealth();
        }

        if (dungeon_crawler.core.globals.adventurer.decrementDamagePotionDuration()) {
            dungeon_crawler.main.updateAdventurerDamage();
        }

        if (dungeon_crawler.core.globals.adventurer.decrementShieldPotionDuration()) {
            dungeon_crawler.main.updateAdventurerProtection();
        }

        dungeon_crawler.core.globals.InCombat = false;

        dungeon_crawler.main.resetDiceValues();

        //deselect previous tile
        let previousIndex = dungeon_crawler.core.globals.currentLevel.tiles.currentIndex;
        let previousTile = dungeon_crawler.core.globals.currentLevel.tiles.get(previousIndex);
        previousTile.Current = false;

        dungeon_crawler.core.globals.currentLevel.tiles.currentIndex = selectedTile.Index;
        selectedTile.Current = true;
        selectedTile.Hidden = false;

        let selectedTileType = selectedTile.Type;

        if (selectedTileType == dungeon_crawler.core.globals.tileTypes['unknown']) {
            //new tile
            let nextTileType = dungeon_crawler.core.globals.currentLevel.tiles.getNextTileType();

            switch (nextTileType) {
                case dungeon_crawler.core.globals.tileTypes['macguffin']:
                    dungeon_crawler.core.globals.macguffinFound = true;
                    dungeon_crawler.main.macGuffinText();
                    break;
                case dungeon_crawler.core.globals.tileTypes['chest']:

                    //todo: pause game on chest
                    nextTileType = dungeon_crawler.main.selectLoot()

                    switch (nextTileType) {
                        case dungeon_crawler.core.globals.tileTypes['potion']:
                            //todo: pause game on potion

                            let potionType = dungeon_crawler.main.selectPotionType();
                            let potionSize = dungeon_crawler.main.selectPotionSize();
                            let potionDuration = dungeon_crawler.main.selectPotionDuration();

                            dungeon_crawler.main.usePotion(potionType, potionSize, potionDuration);
                            dungeon_crawler.main.usePotionText(potionType, potionSize, potionDuration);
                            break;
                        case dungeon_crawler.core.globals.tileTypes['protection']:
                            //todo: pause game on protection

                            let armourType = dungeon_crawler.main.selectArmourType();
                            let armourCondition = dungeon_crawler.main.selectArmourCondition();

                            let armourValue = dungeon_crawler.main.getArmourValue(armourType, armourCondition);

                            let currentArmourValue, keepArmour = false;

                            switch (armourType) {
                                case dungeon_crawler.core.globals.armourType['helmet']:
                                    currentArmourValue = dungeon_crawler.core.globals.adventurer.getArmourHelmet();

                                    if (armourValue > currentArmourValue) {
                                        dungeon_crawler.core.globals.adventurer.setArmourHelmet(armourValue);
                                        keepArmour = true;
                                    }

                                    break;
                                case dungeon_crawler.core.globals.armourType['breastplate']:
                                    currentArmourValue = dungeon_crawler.core.globals.adventurer.getArmourBreastplate();

                                    if (armourValue > currentArmourValue) {
                                        dungeon_crawler.core.globals.adventurer.setArmourBreastplate(armourValue);
                                        keepArmour = true;
                                    }

                                    break;
                                case dungeon_crawler.core.globals.armourType['vambrace']:
                                    currentArmourValue = dungeon_crawler.core.globals.adventurer.getArmourVambrace();

                                    if (armourValue > currentArmourValue) {
                                        dungeon_crawler.core.globals.adventurer.setArmourVambrace(armourValue);
                                        keepArmour = true;
                                    }

                                    break;
                                case dungeon_crawler.core.globals.armourType['gauntlet']:
                                    currentArmourValue = dungeon_crawler.core.globals.adventurer.getArmourGauntlet();

                                    if (armourValue > currentArmourValue) {
                                        dungeon_crawler.core.globals.adventurer.setArmourGauntlet(armourValue);
                                        keepArmour = true;
                                    }

                                    break;
                                case dungeon_crawler.core.globals.armourType['greave']:
                                    currentArmourValue = dungeon_crawler.core.globals.adventurer.getArmourGreave();

                                    if (armourValue > currentArmourValue) {
                                        dungeon_crawler.core.globals.adventurer.setArmourGreave(armourValue);
                                        keepArmour = true;
                                    }

                                    break;
                                case dungeon_crawler.core.globals.armourType['boots']:
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
                                dungeon_crawler.main.setProtectionUseText(armourType, armourCondition, armourValue);
                                dungeon_crawler.main.updateAdventurerProtection();
                            } else {
                                dungeon_crawler.main.setProtectionDiscardText(armourType, armourCondition, armourValue);
                            }

                            break;
                        case dungeon_crawler.core.globals.tileTypes['weapon']:
                            let weponType = dungeon_crawler.main.selectWeponType();                            
                            let weponCondition = null;
                            if (weponType !== dungeon_crawler.core.globals.weaponType['rock'] && weponType !== dungeon_crawler.core.globals.weaponType['club']) {
                                 weponCondition = dungeon_crawler.main.selectWeponCondition();
                            }                            

                            let weaponValue = dungeon_crawler.main.getWeaponValue(weponType, weponCondition);
                            let currentWeaponValue = dungeon_crawler.core.globals.adventurer.getWeapon();

                            if (weaponValue > currentWeaponValue) {
                                dungeon_crawler.core.globals.adventurer.setWeapon(weaponValue);
                                dungeon_crawler.main.setWeaponUseText(weponType, weponCondition, weaponValue);

                                dungeon_crawler.main.updateAdventurerDamage();
                            } else {
                                dungeon_crawler.main.setWeaponDiscardText(weponType, weponCondition, weaponValue);
                            }

                            break;
                    }

                    break;
            }

            selectedTile.Type = nextTileType;
        } else {
            //reentering a tile
            if (selectedTileType == dungeon_crawler.core.globals.tileTypes['entrance']) {
                if (!dungeon_crawler.core.globals.macguffinFound) {
                    dungeon_crawler.main.exitWithoutMacGuffinText();
                } else {
                    dungeon_crawler.main.endGamge();
                    dungeon_crawler.main.exitWithMacGuffinText();
                    return;
                }
            } else if (selectedTileType == dungeon_crawler.core.globals.tileTypes['stairs_ascending']) {
                let previouslevel = dungeon_crawler.core.globals.currentLevel.level - 1;

                dungeon_crawler.main.generateLevel(previouslevel);
                dungeon_crawler.main.stairsUpText(previouslevel);
            } else if (selectedTileType == dungeon_crawler.core.globals.tileTypes['empty'] || selectedTileType == dungeon_crawler.core.globals.tileTypes['fight']) {
                //if tile has already been placed roll for monster encounter
                let repeatTile = dungeon_crawler.core.globals.currentLevel.tiles.getRepeatTileType();

                if (repeatTile != null) {
                    selectedTile.Type = repeatTile;
                }
            }
        }

        if (selectedTile.Type == dungeon_crawler.core.globals.tileTypes['stairs_descending']) {
            dungeon_crawler.core.globals.currentLevel.stairsDeployed = true;

            let nextlevel = dungeon_crawler.core.globals.currentLevel.level + 1;

            dungeon_crawler.main.generateLevel(nextlevel);
            dungeon_crawler.main.stairsDownText(nextlevel);
        }

        dungeon_crawler.core.globals.currentLevel.tiles.setSelectables();
        dungeon_crawler.main.setStage();

        dungeon_crawler.core.globals.eventBindings.unbindEvents();
        dungeon_crawler.core.globals.eventBindings.clearBoundEvents();

        dungeon_crawler.main.bindEvents();
    },

    combat() {
        let monsterDifficulty = dungeon_crawler.main.selectMonsterDifficulty();

        let enemy = dungeon_crawler.core.globals.currentLevel.getEnemy();

        //todo: use monster difficulty role to generate monsters
        let currentEnemy = new Enemy();
        currentEnemy.generateEnemy(enemy.type, enemy.healthDice, enemy.damageDice, enemy.protectionDice);

        dungeon_crawler.main.resetDiceValues();
        let adventurerScore = dungeon_crawler.main.roleSafeDie();
        let monsterScore = dungeon_crawler.main.roleDangerDie();

        //If Adventurer wins the roll they starts combat
        let adventurerInitiatesCombat = false;
        if (adventurerScore > monsterScore) {
            adventurerInitiatesCombat = true;
        }

        let enemyType = currentEnemy.getType();

        dungeon_crawler.main.monsterEncounterText(adventurerInitiatesCombat, enemyType, currentEnemy.getHealth(), currentEnemy.getDamage(), currentEnemy.getProtection());

        let adventurerDamage = dungeon_crawler.core.globals.adventurer.getDamage();
        let adventurerProtection = dungeon_crawler.core.globals.adventurer.getProtection();

        let enemyDamage = currentEnemy.getDamage();
        let enemyProtection = currentEnemy.getProtection();

        let adventurerRoll, enemyRoll, attackValue, avoidValue, wounds;
        do {
            //Adventurer fight
            dungeon_crawler.main.resetDiceValues();
            wounds = null;
            if (adventurerInitiatesCombat) {
                adventurerRoll = dungeon_crawler.main.roleSafeDie();
                attackValue = adventurerRoll + adventurerDamage;

                enemyRoll = dungeon_crawler.main.roleDangerDie();
                avoidValue = enemyRoll + enemyProtection;

                if (attackValue > avoidValue) {
                    wounds = attackValue - avoidValue;
                    currentEnemy.reciveWounds(wounds);
                }

                dungeon_crawler.main.adventurerAttackText(enemyType, adventurerRoll, adventurerDamage, attackValue, enemyRoll, enemyProtection, avoidValue, wounds);
            }

            //Monster fight
            dungeon_crawler.main.resetDiceValues();
            wounds = null;
            if (currentEnemy.isAlive()) {
                enemyRoll = dungeon_crawler.main.roleDangerDie();
                attackValue = enemyRoll + enemyDamage;

                adventurerRoll = dungeon_crawler.main.roleSafeDie();
                avoidValue = adventurerRoll + adventurerProtection;

                if (attackValue > avoidValue) {
                    wounds = attackValue - avoidValue;

                    //Damage will be dealt to Shield potion (if avalible), then Aura potion (if avalible) and finaly the Adventurer.  
                    //The function will return the number of wounds taken by the Adventurer.
                    let adventurerWounds = dungeon_crawler.core.globals.adventurer.reciveWounds(wounds);
                    wounds = adventurerWounds;

                    dungeon_crawler.main.updateAdventurerHealth();

                    //as the Shield potion may have taken dammage it too is updated
                    dungeon_crawler.main.updateAdventurerProtection();
                }

                dungeon_crawler.main.enemyAttackText(enemyType, enemyRoll, enemyDamage, attackValue, adventurerRoll, adventurerProtection, avoidValue, wounds);
            }

            adventurerInitiatesCombat = true;

        } while (dungeon_crawler.core.globals.adventurer.isAlive() && currentEnemy.isAlive());

        if (dungeon_crawler.core.globals.adventurer.isAlive()) {
            dungeon_crawler.main.enemyDeathText(enemyType);
        } else {
            dungeon_crawler.main.adventurerDeathText(enemyType);
            dungeon_crawler.main.endGamge();
        }
    },

    endGamge() {
        dungeon_crawler.core.globals.currentLevel.tiles.unsetSelectables();
        dungeon_crawler.main.setStage();

        dungeon_crawler.core.globals.eventBindings.unbindEvents();
        dungeon_crawler.core.globals.eventBindings.clearBoundEvents();
    },

    //Monster difficulty selection
    selectMonsterDifficulty() {
        return dungeon_crawler.main.roleDangerDie();
    },

    //Loot
    //  1 - 2:  Potion
    //  3 - 4:  Weapon
    //  5 - 6:  Protection
    selectLoot() {
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
            case 2:
                return dungeon_crawler.core.globals.tileTypes['potion'];
                break;
            case 3:
            case 4:
                return dungeon_crawler.core.globals.tileTypes['weapon'];
                break;
            case 5:
            case 6:
                return dungeon_crawler.core.globals.tileTypes['protection'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected loot table role "${value}"`);
        return dungeon_crawler.core.globals.tileTypes['unknown'];
    },

    //  Potion
    //  1 - 2:  Sheild (Protection)
    //  3 - 4:  Damage
    //  5 - 6:  Aura (Health)
    selectPotionType() {
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
            case 2:
                return dungeon_crawler.core.globals.potionType['sheild'];
                break;
            case 3:
            case 4:
                return dungeon_crawler.core.globals.potionType['damage'];
                break;
            case 5:
            case 6:
                return dungeon_crawler.core.globals.potionType['aura'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected potion table role "${value}"`);
        return dungeon_crawler.core.globals.potionType['unknown'];
    },

    selectPotionSize() {
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
            case 2:
                return dungeon_crawler.core.globals.potionSize['vial'];
                break;
            case 3:
            case 4:
                return dungeon_crawler.core.globals.potionSize['flask'];
                break;
            case 5:
            case 6:
                return dungeon_crawler.core.globals.potionSize['bottle'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected potion size role "${value}"`);
        return dungeon_crawler.core.globals.potionSize['unknown'];
    },

    selectPotionDuration() {
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
            case 2:
                return dungeon_crawler.core.globals.potionDuration['short'];
                break;
            case 3:
            case 4:
                return dungeon_crawler.core.globals.potionDuration['medium'];
                break;
            case 5:
            case 6:
                return dungeon_crawler.core.globals.potionDuration['long'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected potion duration role "${value}"`);
        return dungeon_crawler.core.globals.potionDuration['unknown'];
    },

    usePotion(potionType, potionSize, potionDuration) {
        let sizeValue = 0;
        switch (potionSize) {
            case dungeon_crawler.core.globals.potionSize['vial']:
                sizeValue = 6;
                break;
            case dungeon_crawler.core.globals.potionSize['flask']:
                sizeValue = 12;
                break;
            case dungeon_crawler.core.globals.potionSize['bottle']:
                sizeValue = 18;
                break;
            default:
                dungeon_crawler.core.outputError(`Unexpected potion size "${potionSize}"`);
                sizeValue = 0;
                break;
        }

        let durationValue = 0;
        switch (potionDuration) {
            case dungeon_crawler.core.globals.potionDuration['short']:
                durationValue = 10;
                break;
            case dungeon_crawler.core.globals.potionDuration['medium']:
                durationValue = 20;
                break;
            case dungeon_crawler.core.globals.potionDuration['long']:
                durationValue = 30;
                break;
            default:
                dungeon_crawler.core.outputError(`Unexpected potion duration "${potionDuration}"`);
                durationValue = 0;
                break;
        }

        switch (potionType) {
            case dungeon_crawler.core.globals.potionType['aura']:
                let regainedHealth = dungeon_crawler.core.globals.adventurer.setAuraPotion(sizeValue);
                if (regainedHealth > 0) {
                    dungeon_crawler.main.usePotionHealingText(regainedHealth);
                }

                dungeon_crawler.core.globals.adventurer.setAuraPotionDuration(durationValue);
                dungeon_crawler.main.updateAdventurerHealth();
                break;
            case dungeon_crawler.core.globals.potionType['damage']:
                dungeon_crawler.core.globals.adventurer.setDamagePotion(sizeValue);
                dungeon_crawler.core.globals.adventurer.setDamagePotionDuration(durationValue);
                dungeon_crawler.main.updateAdventurerDamage();
                break;
            case dungeon_crawler.core.globals.potionType['sheild']:
                dungeon_crawler.core.globals.adventurer.setShieldPotion(sizeValue);
                dungeon_crawler.core.globals.adventurer.setShieldPotionDuration(durationValue);
                dungeon_crawler.main.updateAdventurerProtection();
                break;
            default:
                dungeon_crawler.core.outputError(`Unexpected potion type "${potionType}"`);
                durationValue = 0;
                break;
        }
    },

    selectArmourType() {
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
                return dungeon_crawler.core.globals.armourType['boots'];
                break;
            case 2:
                return dungeon_crawler.core.globals.armourType['greave'];
                break;
            case 3:
                return dungeon_crawler.core.globals.armourType['vambrace'];
                break;
            case 4:
                return dungeon_crawler.core.globals.armourType['gauntlet'];
                break;
            case 5:
                return dungeon_crawler.core.globals.armourType['helmet'];
                break;
            case 6:
                return dungeon_crawler.core.globals.armourType['breastplate'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected armour type role "${value}"`);
        return dungeon_crawler.core.globals.armourType['unknown'];
    },

    //todo: Add magical Conditions
    selectArmourCondition() {
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
            case 2:
                return dungeon_crawler.core.globals.armourCondition['rusty'];
                break;
            case 3:
            case 4:
            case 5:
                return dungeon_crawler.core.globals.armourCondition['tarnished'];
                break;
            case 6:
                return dungeon_crawler.core.globals.armourCondition['shiny'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected armour condition role "${value}"`);
        return dungeon_crawler.core.globals.armourCondition['unknown'];
    },

    getArmourValue(armourType, armourCondition) {
        let armourTypeValue = 0;
        switch (armourType) {
            case dungeon_crawler.core.globals.armourType['helmet']:
                armourTypeValue = 4;
                break;
            case dungeon_crawler.core.globals.armourType['breastplate']:
                armourTypeValue = 4;
                break;
            case dungeon_crawler.core.globals.armourType['vambrace']:
                armourTypeValue = 2;
                break;
            case dungeon_crawler.core.globals.armourType['gauntlet']:
                armourTypeValue = 3;
                break;
            case dungeon_crawler.core.globals.armourType['greave']:
                armourTypeValue = 1;
                break;
            case dungeon_crawler.core.globals.armourType['boots']:
                armourTypeValue = 1;
                break;
            default:
                dungeon_crawler.core.outputError(`Unexpected armour type "${armourType}"`);
                armourTypeValue = 0;
                break;
        }

        let armourConditionvalue = 0;
        switch (armourCondition) {
            case dungeon_crawler.core.globals.armourCondition['rusty']:
                armourConditionvalue = 2;
                break;
            case dungeon_crawler.core.globals.armourCondition['tarnished']:
                armourConditionvalue = 3;
                break;
            case dungeon_crawler.core.globals.armourCondition['shiny']:
                armourConditionvalue = 4;
                break;
            default:
                dungeon_crawler.core.outputError(`Unexpected armour condition "${armourCondition}"`);
                armourConditionvalue = 0;
                break;
        }

        return armourTypeValue * armourConditionvalue;
    },

    selectWeponType() {
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
                return dungeon_crawler.core.globals.weaponType['rock'];
                break;
            case 2:
                return dungeon_crawler.core.globals.weaponType['club'];
                break;
            case 3:
                return dungeon_crawler.core.globals.weaponType['dagger'];
                break;
            case 4:
                return dungeon_crawler.core.globals.weaponType['mace'];
                break;
            case 5:
                return dungeon_crawler.core.globals.weaponType['axe'];
                break;
            case 6:
                return dungeon_crawler.core.globals.weaponType['sword'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected wepon type role "${value}"`);
        return dungeon_crawler.core.globals.armourType['unknown'];
    },

    selectWeponCondition() {
        let value = dungeon_crawler.main.roleSafeDie();

        switch (value) {
            case 1:
                return dungeon_crawler.core.globals.weaponCondition['broken'];
                break;
            case 2:
                return dungeon_crawler.core.globals.weaponCondition['rusty'];
                break;
            case 3:
                return dungeon_crawler.core.globals.weaponCondition['chipped'];
                break;
            case 4:
                return dungeon_crawler.core.globals.weaponCondition['sharp'];
                break;
            case 5:
                return dungeon_crawler.core.globals.weaponCondition['enchanted'];
                break;
            case 6:
                return dungeon_crawler.core.globals.weaponCondition['flaming'];
                break;
        }

        dungeon_crawler.core.outputError(`Unexpected wepon condition role "${value}"`);
        return dungeon_crawler.core.globals.armourCondition['unknown'];
    },

    getWeaponValue(weponType, weponCondition) {
        let weponTypeValue = 0;
        switch (weponType) {
            case dungeon_crawler.core.globals.weaponType['rock']:
                weponTypeValue = 1;
                break;
            case dungeon_crawler.core.globals.weaponType['club']:
                weponTypeValue = 2;
                break;
            case dungeon_crawler.core.globals.weaponType['dagger']:
                weponTypeValue = 4;
                break;
            case dungeon_crawler.core.globals.weaponType['mace']:
                weponTypeValue = 6;
                break;
            case dungeon_crawler.core.globals.weaponType['axe']:
                weponTypeValue = 8;
                break;
            case dungeon_crawler.core.globals.weaponType['sword']:
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
                case dungeon_crawler.core.globals.weaponCondition['broken']:
                    weponConditionValue = 1;
                    break;
                case dungeon_crawler.core.globals.weaponCondition['rusty']:
                    weponConditionValue = 2;
                    break;
                case dungeon_crawler.core.globals.weaponCondition['chipped']:
                    weponConditionValue = 4;
                    break;
                case dungeon_crawler.core.globals.weaponCondition['sharp']:
                    weponConditionValue = 6;
                    break;
                case dungeon_crawler.core.globals.weaponCondition['enchanted']:
                    weponConditionValue = 8;
                    break;
                case dungeon_crawler.core.globals.weaponCondition['flaming']:
                    weponConditionValue = 10;
                    break;
                default:
                    dungeon_crawler.core.outputError(`Unexpected wepon condition "${weponCondition}"`);
                    weponConditionValue = 0;
                    break;
            }
        }        

        return weponTypeValue * weponConditionValue;
    },

    //Dice
    roleSafeDie() {
        let value = dungeon_crawler.main.roleDSix();
        dungeon_crawler.main.setSafeDieValue(value);
        return value;
    },

    roleDangerDie() {
        let value = dungeon_crawler.main.roleDSix();
        dungeon_crawler.main.setDangerDieValue(value);
        return value;
    },

    roleDSix() {
        return dungeon_crawler.main.roleDie(6);
    },

    roleDTwenty() {
        return dungeon_crawler.main.roleDie(20);
    },

    roleDie(max) {
        return Math.floor(Math.random() * (max)) + 1;
    },

    setTiles(stageCols, stageRows) {
        let hexagonHeight = dungeon_crawler.core.globals.hexHeight;
        let hexagonWidth = dungeon_crawler.core.globals.hexWidth;

        //set stage dimentions
        //  height
        let stageHeight = stageCols * hexagonHeight;
        dungeon_crawler.core.globals.stageHeight = stageHeight;

        //  width
        let hexWidthQuaters = hexagonWidth / 4;
        let stageWidth = stageRows * (hexWidthQuaters * 3) + hexWidthQuaters;
        dungeon_crawler.core.globals.stageWidth = stageWidth;

        //set board
        let hexagonLeft = 0, hexagonTop = 0, hexRow = -1, hexColumn = 0;

        //  due to the orientation of our board we miss one hex for every other grid row
        let tileCount = (stageCols * stageRows) - Math.ceil((stageRows + 1) / 2);

        hexagonTop -= hexagonHeight / 2;
        for (var i = 0; i < tileCount; i++) {
            hexagonTop += hexagonHeight;

            if (hexagonTop >= stageHeight - (hexagonWidth / 2)) {
                //move tile along one place
                hexagonLeft += (hexagonWidth / 4) * 3;

                //reset top
                if ((hexColumn % 2) == 1) {
                    //long
                    hexagonTop = hexagonHeight - (hexagonHeight / 2);
                } else {
                    //short
                    hexagonTop = 0;
                }

                hexRow = 0;

                //add column
                hexColumn += 1;
            } else {
                hexRow += 1;
            }

            dungeon_crawler.core.globals.currentLevel.tiles.add(new Tile(i, dungeon_crawler.core.globals.tileTypes['unknown'], hexRow, hexColumn, hexagonLeft, hexagonTop));
        }
    },

    setStage() {
        $('#stage').html('').css({ 'height': `${dungeon_crawler.core.globals.stageHeight}px`, 'width': `${dungeon_crawler.core.globals.stageWidth}px` });

        let tile, tileTypeClass, tileSelectableClass, tileText, tiles = dungeon_crawler.core.globals.currentLevel.tiles;

        for (var i = 0; i < tiles.length; i++) {
            tileTypeClass = 'hexagon-tile-hidden';
            tileSelectableClass = '';

            tile = tiles.get(i);

            tileText = `${tile.Row} - ${tile.Column}`;

            if (!tile.Hidden) {
                if (typeof tile.Hidden == 'undefined' || tile.Hidden == null) {
                    tileTypeClass = 'hexagon-tile-unknown';
                } else {
                    switch (tile.Type) {
                        //entrance
                        case dungeon_crawler.core.globals.tileTypes['entrance']:
                            tileTypeClass = 'hexagon-tile-entrance';
                            break;
                        //exit
                        case dungeon_crawler.core.globals.tileTypes['exit']:
                            tileTypeClass = 'hexagon-tile-exit';
                            break;
                        case dungeon_crawler.core.globals.tileTypes['stairs_ascending']:
                            tileTypeClass = 'hexagon-tile-stairs-ascending';
                            break;
                        case dungeon_crawler.core.globals.tileTypes['stairs_descending']:
                            tileTypeClass = 'hexagon-tile-stairs-descending';
                            break;
                        case dungeon_crawler.core.globals.tileTypes['fight']:
                            tileTypeClass = 'hexagon-tile-fight';
                            break;
                        case dungeon_crawler.core.globals.tileTypes['chest']:
                            tileTypeClass = 'hexagon-tile-chest';
                            break;
                        case dungeon_crawler.core.globals.tileTypes['weapon']:
                            tileTypeClass = 'hexagon-tile-weapon';
                            break;
                        case dungeon_crawler.core.globals.tileTypes['protection']:
                            tileTypeClass = 'hexagon-tile-protection';
                            break;
                        case dungeon_crawler.core.globals.tileTypes['empty']:
                            tileTypeClass = 'hexagon-tile-empty';
                            break;
                        case dungeon_crawler.core.globals.tileTypes['potion']:
                            tileTypeClass = 'hexagon-tile-potion';
                            break;
                        case dungeon_crawler.core.globals.tileTypes['macguffin']:
                            tileTypeClass = 'hexagon-tile-macguffin';
                            break;
                        default:
                        case dungeon_crawler.core.globals.tileTypes['unknown']:
                            tileTypeClass = 'hexagon-tile-unknown';
                            break;
                    }
                }
            }

            if (tile.Selectable) {
                tileSelectableClass = 'hexagon-tile-selectable';
            }

            $('#stage').append(`<div data-identity="${tile.Id}" class="hexagon-tile ${tileTypeClass} ${tileSelectableClass}" style="left: ${tile.X}px; top: ${tile.Y}px"><span>${tileText}</span></div>`);
        }
    },

    //Adventurer
    generateAdventurer() {
        let healthValue = dungeon_crawler.main.roleSafeDie() + dungeon_crawler.main.roleSafeDie();
        let damageValue = dungeon_crawler.main.roleSafeDie();
        let protectionValue = dungeon_crawler.main.roleSafeDie();

        dungeon_crawler.main.startingAdventurerText(healthValue, damageValue, protectionValue);

        dungeon_crawler.core.globals.adventurer = new Adventurer(healthValue, damageValue, protectionValue);
    },

    setLevelDetails() {
        $('#current-level').html(dungeon_crawler.core.globals.currentLevel.level);
    },

    setAdventurerDetails() {
        dungeon_crawler.main.updateAdventurerHealth();
        dungeon_crawler.main.updateAdventurerDamage();
        dungeon_crawler.main.updateAdventurerProtection();
    },

    updateAdventurerHealth() {
        $('#current-health').html(dungeon_crawler.core.globals.adventurer.getHealthDescription());
    },

    updateAdventurerDamage() {
        $('#current-damage').html(dungeon_crawler.core.globals.adventurer.getDamageDescription());
    },

    updateAdventurerProtection() {
        $('#current-protection').html(dungeon_crawler.core.globals.adventurer.getProtectionDescription());
    },

    resetDiceValues() {
        dungeon_crawler.main.resetSafeDieValue();
        dungeon_crawler.main.resetDangerDieValue();
    },

    resetSafeDieValue() {
        $('#current-dice-safe').html('');
    },

    setSafeDieValue(value) {
        let dieHTML = dungeon_crawler.main.getDieHTML(value);
        $('#current-dice-safe').append(dieHTML);
    },

    resetDangerDieValue() {
        $('#current-dice-danger').html('');
    },

    setDangerDieValue(value) {
        let dieHTML = dungeon_crawler.main.getDieHTML(value);
        $('#current-dice-danger').append(dieHTML);
    },

    getDieHTML(value) {
        let diceClass = "empty";

        switch (value) {
            case 1:
                diceClass = "one";
                break;
            case 2:
                diceClass = "two";
                break;
            case 3:
                diceClass = "three";
                break;
            case 4:
                diceClass = "four";
                break;
            case 5:
                diceClass = "five";
                break;
            case 6:
                diceClass = "six";
                break;
        }

        return `<div class="die ${diceClass}"></div>`;
    },

    //Log
    //  Story
    //      Adventurer
    startingAdventurerText(health, damage, protection) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateStartingAdventurerText(health, damage, protection));
    },

    //          Weapon
    setWeaponUseText(type, condition, weaponValue) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateWeaponValuenUseText(type, condition, weaponValue));
    },

    setWeaponDiscardText(type, condition, weaponValue) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateWeaponDiscardText(type, condition, weaponValue));
    },

    //          Armour
    setProtectionUseText(type, condition, armourValue) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateProtectionUseText(type, condition, armourValue));
    },

    setProtectionDiscardText(type, condition, armourValue) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateProtectionDiscardText(type, condition, armourValue));
    },

    //          Potion
    usePotionText(potionType, potionSize, potionDuration) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateUsePotionText(potionType, potionSize, potionDuration));
    },

    //              Cure
    usePotionHealingText(regainedHealth) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generatePotionHealingText(regainedHealth));
    },

    //      Stairs down
    stairsDownText(level) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateStairsDownText(level));
    },

    //      Stairs up
    stairsUpText(level) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateStairsUpText(level));
    },

    //      MacGuffin
    macGuffinText() {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateMacGuffinText());
    },

    exitWithoutMacGuffinText() {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateExitWithoutMacGuffinText());
    },

    exitWithMacGuffinText() {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateExitWithMacGuffinText());
    },

    //  Combat
    monsterEncounterText(adventurerInitiatesCombat, enemyType, health, damage, protection) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateMonsterEncounterText(adventurerInitiatesCombat, enemyType, health, damage, protection));
    },

    adventurerAttackText(enemyType, adventurerRoll, adventurerDamage, adventurerAttackValue, enemyRoll, enemyProtection, enemyAvoidValue, wounds) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateAdventurerAttackText(enemyType, adventurerRoll, adventurerDamage, adventurerAttackValue, enemyRoll, enemyProtection, enemyAvoidValue, wounds));
    },

    adventurerDeathText(enemyType) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateAdventurerDeathText(enemyType));
    },

    enemyAttackText(enemyType, enemyRoll, enemyDamage, attackValue, adventurerRoll, adventurerProtection, avoidValue, wounds) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateEnemyAttackText(enemyType, enemyRoll, enemyDamage, attackValue, adventurerRoll, adventurerProtection, avoidValue, wounds));
    },

    enemyDeathText(enemyType) {
        dungeon_crawler.main.setLog(dungeon_crawler.log.generateEnemyDeathText(enemyType));
    },

    setLog(message) {
        $('#log').prepend(`<div class="entry">${message}</div>`);
    }
};

window['dungeon_crawler_main_startup'] = dungeon_crawler.main.startup;