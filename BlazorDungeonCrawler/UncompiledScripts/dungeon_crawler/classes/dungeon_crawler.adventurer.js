class Adventurer {
    constructor(health, damage, protection) {
        this.HealthBase = health;
        this.HealthInitial = health;
        this.AuraPotion = 0;
        this.AuraPotionDuration = 0;

        this.DamageBase = damage;
        this.DamagePotion = 0;
        this.DamagePotionDuration = 0;

        this.ProtectionBase = protection;
        this.ShieldPotion = 0;
        this.ShieldPotionDuration = 0;

        this.Weapon = 0;

        this.ArmourHelmet = 0;
        this.ArmourBreastplate = 0;
        this.ArmourVambrace = 0;
        this.ArmourGauntlet = 0;
        this.ArmourGreave = 0;
        this.ArmourBoots = 0;

        this.IsAlive = true;
    }

    //Health
    getHealth() {
        return this.HealthBase + this.AuraPotion;
    }

    //if health has been lost then use the potion point to heal (up to inital rolled value) and add remaining points as Aura
    setAuraPotion(potionValue) {
        let regainedHealth = 0;

        //check if damage has been taken
        if (this.HealthBase < this.HealthInitial) {
            let damageTaken = this.HealthInitial - this.HealthBase;

            if (damageTaken >= potionValue) {
                //if damage taken is more (than the potion value) add potion value to the current health
                this.HealthBase += potionValue;
                regainedHealth = potionValue;
            } else {
                //if damage taken is less (than the potion value) heal the damage and use the remaining points as Aura
                this.AuraPotion += potionValue - damageTaken;
                this.HealthBase = this.HealthInitial;
                regainedHealth = damageTaken;
            }
        } else {
            this.AuraPotion += potionValue;
        }

        return regainedHealth;
    }

    setAuraPotionDuration(value) {
        return this.AuraPotionDuration += value;
    }

    decrementAuraPotionDuration() {
        if (this.AuraPotionDuration > 0) {
            this.AuraPotionDuration -= 1;

            if (this.AuraPotionDuration <= 0) {
                this.AuraPotionDuration = 0;
                this.AuraPotion = 0;
            }

            return true;
        }

        return false;
    }

    getHealthDescription() {
        let message = `${this.getHealth()}`;

        if (this.AuraPotion > 0) {
            message += ` (${this.HealthBase} + ${this.AuraPotion}) ${this.AuraPotionDuration}`;
        }

        return message;
    }

    //Damage
    getDamage() {
        return this.DamageBase + this.Weapon + this.DamagePotion;
    }

    //  Weapon
    getWeapon() {
        return this.Weapon;
    }

    setWeapon(value) {
        this.Weapon = value;
    }

    //  Potion
    setDamagePotion(value) {
        return this.DamagePotion += value;
    }

    setDamagePotionDuration(value) {
        return this.DamagePotionDuration += value;
    }

    decrementDamagePotionDuration() {
        if (this.DamagePotionDuration > 0) {
            this.DamagePotionDuration -= 1;

            if (this.DamagePotionDuration <= 0) {
                this.DamagePotionDuration = 0;
                this.DamagePotion = 0;
            }

            return true;
        }

        return false;
    }

    getDamageDescription() {
        let message = `${this.getDamage()}`;

        if (this.Weapon > 0) {
            message += ` (${this.Weapon})`;
        }

        if (this.DamagePotion > 0) {
            message += ` (${this.DamageBase} + ${this.DamagePotion}) ${this.DamagePotionDuration}`;
        }

        return message;
    }

    //Protection
    getProtection() {
        return this.ProtectionBase + this.ShieldPotion + this.ArmourHelmet + this.ArmourBreastplate + this.ArmourVambrace + this.ArmourGauntlet + this.ArmourGreave + this.ArmourBoots;
    }

    //Armour
    //  Helmet
    getArmourHelmet() {
        return this.ArmourHelmet;
    }

    setArmourHelmet(value) {
        return this.ArmourHelmet = value;
    }

    //  Breastplate
    getArmourBreastplate() {
        return this.ArmourBreastplate;
    }

    setArmourBreastplate(value) {
        return this.ArmourBreastplate = value;
    }

    //  Vambrace
    getArmourVambrace() {
        return this.ArmourVambrace;
    }

    setArmourVambrace(value) {
        return this.ArmourVambrace = value;
    }

    //  Gauntlet
    getArmourGauntlet() {
        return this.ArmourGauntlet;
    }

    setArmourGauntlet(value) {
        return this.ArmourGauntlet = value;
    }

    //  Greave
    getArmourGreave() {
        return this.ArmourGreave;
    }

    setArmourGreave(value) {
        return this.ArmourGreave = value;
    }

    //  Boots
    getArmourBoots() {
        return this.ArmourBoots;
    }

    setArmourBoots(value) {
        return this.ArmourBoots = value;
    }

    //Shield potion
    setShieldPotion(value) {
        return this.ShieldPotion += value;
    }

    setShieldPotionDuration(value) {
        return this.ShieldPotionDuration += value;
    }

    decrementShieldPotionDuration() {
        if (this.ShieldPotionDuration > 0) {
            this.ShieldPotionDuration -= 1;

            if (this.ShieldPotionDuration <= 0) {
                this.ShieldPotionDuration = 0;
                this.ShieldPotion = 0;
            }

            return true;
        }

        return false;
    }

    getProtectionDescription() {
        let message = `${this.getProtection()}`;

        let messageArmourAddition = [];
        if (this.ArmourHelmet > 0) {
            messageArmourAddition.push(this.ArmourHelmet);
        };

        if (this.ArmourBreastplate > 0) {
            messageArmourAddition.push(this.ArmourBreastplate);
        };

        if (this.ArmourVambrace > 0) {
            messageArmourAddition.push(this.ArmourVambrace);
        };

        if (this.ArmourGauntlet > 0) {
            messageArmourAddition.push(this.ArmourGauntlet);
        };

        if (this.ArmourGreave > 0) {
            messageArmourAddition.push(this.ArmourGreave);
        };

        if (this.ArmourBoots > 0) {
            messageArmourAddition.push(this.ArmourBoots);
        };

        if (messageArmourAddition.length > 0) {
            message += ` (${messageArmourAddition.join(' + ')})`;
        }

        if (this.ShieldPotion > 0) {
            message += ` (${this.ProtectionBase} + ${this.ShieldPotion}) ${this.ShieldPotionDuration}`;
        }

        return message;
    }

    //Mortis
    isAlive() {
        return this.IsAlive;
    }

    reciveWounds(dammagePoints) {
        let remainingDammagePoints = 0;
        let shieldPotionDammage = 0;
        let auraPotionDammage = 0;
        let adventurerDammage = 0;

        //take dammage to shield potion
        if (this.ShieldPotion > 0) {
            if (dammagePoints < this.ShieldPotion) {
                shieldPotionDammage = dammagePoints;
                remainingDammagePoints = 0;

                //shield potion took all damage points
                this.ShieldPotion -= dammagePoints;
            } else {

                //shield potion took some damage points
                shieldPotionDammage = this.ShieldPotion;
                remainingDammagePoints = dammagePoints - this.ShieldPotion;

                this.ShieldPotion = 0;
            }

            dammagePoints = remainingDammagePoints;
        }

        //take dammage to aura potion
        if (dammagePoints > 0 && this.AuraPotion > 0) {
            if (dammagePoints < this.AuraPotion) {
                auraPotionDammage = dammagePoints;
                remainingDammagePoints = 0;

                //aura potion took all damage points
                this.AuraPotion -= dammagePoints;
            } else {

                //aura potion took some damage points
                auraPotionDammage = this.AuraPotion;
                remainingDammagePoints = dammagePoints - this.AuraPotion;

                this.AuraPotion = 0;
            }

            dammagePoints = remainingDammagePoints;
        }

        //take dammage to Adventurer
        if (dammagePoints > 0) {
            adventurerDammage = dammagePoints;

            let updatedHealth = this.HealthBase - dammagePoints;

            if (updatedHealth > 0) {
                this.HealthBase = updatedHealth;
            } else {
                this.HealthBase = 0;
                this.IsAlive = false;
            }
        }

        // return { 'shieldPotionDammage': shieldPotionDammage, 'auraPotionDammage': auraPotionDammage, 'adventurerDammage': adventurerDammage }

        return adventurerDammage;
    }
}; 