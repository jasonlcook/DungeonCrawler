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
        return this.DamageBase + this.DamagePotion;
    }

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

        if (this.DamagePotion > 0) {
            message += ` (${this.DamageBase} + ${this.DamagePotion}) ${this.DamagePotionDuration}`;
        }

        return message;
    }

    //Protection
    getProtection() {
        return this.ProtectionBase + this.ShieldPotion;
    }

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

        // return {            'shieldPotionDammage': shieldPotionDammage, 'auraPotionDammage': auraPotionDammage, 'adventurerDammage': adventurerDammage         }

        return adventurerDammage;
    }
}; 