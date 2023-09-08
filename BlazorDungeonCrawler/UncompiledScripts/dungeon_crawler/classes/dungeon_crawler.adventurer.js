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

    getHealthDescription() {
        let message = `${this.getHealth()}`;

        if (this.AuraPotion > 0) {
            message += ` (${this.HealthBase} + ${this.AuraPotion})`;
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

    getStrengthDescription() {
        let message = `${this.getStrength()}`;

        if (this.StrengthPotion > 0) {
            message += ` (${this.StrengthBase} + ${this.StrengthPotion})`;
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

    getArmourDescription() {
        let message = `${this.getArmour()}`;

        if (this.ArmourPotion > 0) {
            message += ` (${this.ArmourBase} + ${this.ArmourPotion})`;
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