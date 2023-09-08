class Adventurer {
    constructor(health, strength, armour) {
        this.HealthBase = health;
        this.AuraPotion = 0;
        this.AuraPotionDuration = 0;

        //todo: change Strength to Damage
        this.StrengthBase = strength;
        this.StrengthPotion = 0;
        this.StrengthPotionDuration = 0;

        //todo: change ArmourBase to ProtectionBase
        this.ArmourBase = armour;
        this.ArmourPotion = 0;
        this.ArmourPotionDuration = 0;

        this.IsAlive = true;
    }

    //Health
    getHealth() {
        return this.HealthBase + this.AuraPotion;
    }

    setAuraPotion(value) {
        return this.AuraPotion += value;
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

    //Strength
    getStrength() {
        return this.StrengthBase + this.StrengthPotion;
    }

    setStrengthPotion(value) {
        return this.StrengthPotion += value;
    }

    setStrengthPotionDuration(value) {
        return this.StrengthPotionDuration += value;
    }

    decrementStrengthPotionDuration() {
        if (this.StrengthPotionDuration > 0) {
            this.StrengthPotionDuration -= 1;

            if (this.StrengthPotionDuration <= 0) {
                this.StrengthPotionDuration = 0;
                this.StrengthPotion = 0;
            }

            return true;
        }

        return false;
    }

    getStrengthDescription() {
        let message = `${this.getStrength()}`;

        if (this.StrengthPotion > 0) {
            message += ` (${this.StrengthBase} + ${this.StrengthPotion}) ${this.StrengthPotionDuration}`;
        }

        return message;
    }

    //Protection
    getArmour() {
        return this.ArmourBase + this.ArmourPotion;
    }

    setArmourPotion(value) {
        return this.ArmourPotion += value;
    }

    setArmourPotionDuration(value) {
        return this.ArmourPotionDuration += value;
    }

    decrementArmourPotionDuration() {
        if (this.ArmourPotionDuration > 0) {
            this.ArmourPotionDuration -= 1;

            if (this.ArmourPotionDuration <= 0) {
                this.ArmourPotionDuration = 0;
                this.ArmourPotion = 0;
            }

            return true;
        }

        return false;
    }

    getArmourDescription() {
        let message = `${this.getArmour()}`;

        if (this.ArmourPotion > 0) {
            message += ` (${this.ArmourBase} + ${this.ArmourPotion}) ${this.ArmourPotionDuration}`;
        }

        return message;
    }

    //Mortis
    isAlive() {
        return this.IsAlive;
    }

    //todo: add Aura and Sheld deplenishment first 
    reciveWounds(value) {
        let updatedHealth = this.HealthBase - value;
        if (updatedHealth > 0) {
            this.set = updatedHealth;
        } else {
            this.HealthBase = 0;
            this.IsAlive = false;
        }
    }
}; 