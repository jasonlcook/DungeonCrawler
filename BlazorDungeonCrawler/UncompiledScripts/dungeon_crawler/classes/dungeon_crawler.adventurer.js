class Adventurer {
    constructor(health, strength, armour) {
        this.Health = health;
        this.Strength = strength;
        this.Armour = armour;

        this.IsAlive = true;
    }

    getHealth() {
        return this.Health;
    }

    getStrength() {
        return this.Strength;
    }

    getArmour() {
        return this.Armour;
    }

    isAlive() {
        return this.IsAlive;
    }

    reciveWounds(value) {
        let currentHealth = this.Health - value;
        if (currentHealth > 0) {
            this.Health = currentHealth;
        } else {
            this.Health = 0;
            this.IsAlive = false;
        }
    }
}; 