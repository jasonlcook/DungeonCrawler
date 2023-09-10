class Adventurer {
    constructor( ) {
        this._healthBase = 0;
        this._healthInitial = 0;
        this._auraPotion = 0;
        this._auraPotionDuration = 0;

        this._damageBase = 0;
        this._damagePotion = 0;
        this._damagePotionDuration = 0;

        this._protectionBase = 0;
        this._shieldPotion = 0;
        this._shieldPotionDuration = 0;

        this._weapon = 0;

        this._armourHelmet = 0;
        this._armourBreastplate = 0;
        this._armourVambrace = 0;
        this._armourGauntlet = 0;
        this._armourGreave = 0;
        this._armourBoots = 0;

        this._isAlive = true;
    }

    //Health
    rollInitialHealth() {
        let health = dungeon_crawler.main.roleSafeDie() + dungeon_crawler.main.roleSafeDie();
        this._healthBase = health;
        this._healthInitial = health;
    }

    getHealth() {
        return this._healthBase + this._auraPotion;
    }

    getHealthText() {
        switch (this._healthBase) {
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

        dungeon_crawler.core.outputError(`Generate health text error with value ${this._healthBase}`);
    }

    //Protection
    rollInitialProtection() {
        let protection = dungeon_crawler.main.roleSafeDie();
        this._protectionBase = protection;
    }

    getProtectionText() {
        switch (this._protectionBase) {
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

        dungeon_crawler.core.outputError(`Generate protection text error with value ${this._protectionBase}`);
    }

    //if health has been lost then use the potion point to heal (up to inital rolled value) and add remaining points as Aura
    setAuraPotion(potionValue) {
        let regainedHealth = 0;

        //check if damage has been taken
        if (this._healthBase < this._healthInitial) {
            let damageTaken = this._healthInitial - this._healthBase;

            if (damageTaken >= potionValue) {
                //if damage taken is more (than the potion value) add potion value to the current health
                this._healthBase += potionValue;
                regainedHealth = potionValue;
            } else {
                //if damage taken is less (than the potion value) heal the damage and use the remaining points as Aura
                this._auraPotion += potionValue - damageTaken;
                this._healthBase = this._healthInitial;
                regainedHealth = damageTaken;
            }
        } else {
            this._auraPotion += potionValue;
        }

        return regainedHealth;
    }

    setAuraPotionDuration(value) {
        return this._auraPotionDuration += value;
    }

    decrementAuraPotionDuration() {
        if (this._auraPotionDuration > 0) {
            this._auraPotionDuration -= 1;

            if (this._auraPotionDuration <= 0) {
                this._auraPotionDuration = 0;
                this._auraPotion = 0;
            }

            return true;
        }

        return false;
    }

    getHealthDescription() {
        let message = `${this.getHealth()}`;

        if (this._auraPotion > 0) {
            message += ` (${this._healthBase} + ${this._auraPotion}) ${this._auraPotionDuration}`;
        }

        return message;
    }

    //Damage
    rollInitialDamage() {
        let damage = dungeon_crawler.main.roleSafeDie();
        this._damageBase = damage;
    }

    getDamageText() {
        switch (this._damageBase) {
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

        dungeon_crawler.core.outputError(`Generate damage text error with value ${this._damageBase}`);
    }

    getDamage() {
        return this._damageBase + this._weapon + this._damagePotion;
    }

    //  Weapon
    getWeapon() {
        return this._weapon;
    }

    setWeapon(value) {
        this._weapon = value;
    }

    //  Potion
    setDamagePotion(value) {
        return this._damagePotion += value;
    }

    setDamagePotionDuration(value) {
        return this._damagePotionDuration += value;
    }

    decrementDamagePotionDuration() {
        if (this._damagePotionDuration > 0) {
            this._damagePotionDuration -= 1;

            if (this._damagePotionDuration <= 0) {
                this._damagePotionDuration = 0;
                this._damagePotion = 0;
            }

            return true;
        }

        return false;
    }

    getDamageDescription() {
        let message = `${this.getDamage()}`;

        if (this._weapon > 0) {
            message += ` (${this._weapon})`;
        }

        if (this._damagePotion > 0) {
            message += ` (${this._damageBase} + ${this._damagePotion}) ${this._damagePotionDuration}`;
        }

        return message;
    }

    //Protection
    getProtection() {
        return this._protectionBase + this._shieldPotion + this._armourHelmet + this._armourBreastplate + this._armourVambrace + this._armourGauntlet + this._armourGreave + this._armourBoots;
    }

    //Armour
    //  Helmet
    getArmourHelmet() {
        return this._armourHelmet;
    }

    setArmourHelmet(value) {
        return this._armourHelmet = value;
    }

    //  Breastplate
    getArmourBreastplate() {
        return this._armourBreastplate;
    }

    setArmourBreastplate(value) {
        return this._armourBreastplate = value;
    }

    //  Vambrace
    getArmourVambrace() {
        return this._armourVambrace;
    }

    setArmourVambrace(value) {
        return this._armourVambrace = value;
    }

    //  Gauntlet
    getArmourGauntlet() {
        return this._armourGauntlet;
    }

    setArmourGauntlet(value) {
        return this._armourGauntlet = value;
    }

    //  Greave
    getArmourGreave() {
        return this._armourGreave;
    }

    setArmourGreave(value) {
        return this._armourGreave = value;
    }

    //  Boots
    getArmourBoots() {
        return this._armourBoots;
    }

    setArmourBoots(value) {
        return this._armourBoots = value;
    }

    //Shield potion
    setShieldPotion(value) {
        return this._shieldPotion += value;
    }

    setShieldPotionDuration(value) {
        return this._shieldPotionDuration += value;
    }

    decrementShieldPotionDuration() {
        if (this._shieldPotionDuration > 0) {
            this._shieldPotionDuration -= 1;

            if (this._shieldPotionDuration <= 0) {
                this._shieldPotionDuration = 0;
                this._shieldPotion = 0;
            }

            return true;
        }

        return false;
    }

    getProtectionDescription() {
        let message = `${this.getProtection()}`;

        let messageArmourAddition = [];
        if (this._armourHelmet > 0) {
            messageArmourAddition.push(this._armourHelmet);
        };

        if (this._armourBreastplate > 0) {
            messageArmourAddition.push(this._armourBreastplate);
        };

        if (this._armourVambrace > 0) {
            messageArmourAddition.push(this._armourVambrace);
        };

        if (this._armourGauntlet > 0) {
            messageArmourAddition.push(this._armourGauntlet);
        };

        if (this._armourGreave > 0) {
            messageArmourAddition.push(this._armourGreave);
        };

        if (this._armourBoots > 0) {
            messageArmourAddition.push(this._armourBoots);
        };

        if (messageArmourAddition.length > 0) {
            message += ` (${messageArmourAddition.join(' + ')})`;
        }

        if (this._shieldPotion > 0) {
            message += ` (${this._protectionBase} + ${this._shieldPotion}) ${this._shieldPotionDuration}`;
        }

        return message;
    }

    //Mortis
    isAlive() {
        return this._isAlive;
    }

    reciveWounds(dammagePoints) {
        let remainingDammagePoints = 0;
        let shieldPotionDammage = 0;
        let auraPotionDammage = 0;
        let adventurerDammage = 0;

        //take dammage to shield potion
        if (this._shieldPotion > 0) {
            if (dammagePoints < this._shieldPotion) {
                shieldPotionDammage = dammagePoints;
                remainingDammagePoints = 0;

                //shield potion took all damage points
                this._shieldPotion -= dammagePoints;
            } else {

                //shield potion took some damage points
                shieldPotionDammage = this._shieldPotion;
                remainingDammagePoints = dammagePoints - this._shieldPotion;

                this._shieldPotion = 0;
            }

            dammagePoints = remainingDammagePoints;
        }

        //take dammage to aura potion
        if (dammagePoints > 0 && this._auraPotion > 0) {
            if (dammagePoints < this._auraPotion) {
                auraPotionDammage = dammagePoints;
                remainingDammagePoints = 0;

                //aura potion took all damage points
                this._auraPotion -= dammagePoints;
            } else {

                //aura potion took some damage points
                auraPotionDammage = this._auraPotion;
                remainingDammagePoints = dammagePoints - this._auraPotion;

                this._auraPotion = 0;
            }

            dammagePoints = remainingDammagePoints;
        }

        //take dammage to Adventurer
        if (dammagePoints > 0) {
            adventurerDammage = dammagePoints;

            let updatedHealth = this._healthBase - dammagePoints;

            if (updatedHealth > 0) {
                this._healthBase = updatedHealth;
            } else {
                this._healthBase = 0;
                this._isAlive = false;
            }
        }

        // return { 'shieldPotionDammage': shieldPotionDammage, 'auraPotionDammage': auraPotionDammage, 'adventurerDammage': adventurerDammage }

        return adventurerDammage;
    }
}; 