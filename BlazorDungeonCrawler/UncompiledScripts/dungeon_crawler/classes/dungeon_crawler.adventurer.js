class Adventurer {
    constructor(health, strength, armour) {
        this.HealthBase = health;
        this.HealthInitial = health;
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