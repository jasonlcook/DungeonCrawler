//todo: rename enemy to monster
class Enemy {
    constructor() {
        this.Type;

        this.Health;
        this.Strength;
        this.Protection;

        this.IsAlive;
    }

    generateEnemy(type, healthDice, strengthDice, protectionDice) {
        this.Type = type;

        this.Health = this.secretRoll(healthDice);
        this.Strength = this.secretRoll(strengthDice);
        this.Protection = this.secretRoll(protectionDice);

        this.IsAlive = true;
    }

    //this is a secret (DM) roll so no need to show the user
    secretRoll(diceCount) {
        let maxValue = diceCount * 6;

        //the enemy values are maximum.
        let value = Math.floor(Math.random() * (maxValue)) + 1;

        //if the roll is below 2 / 3rds then it will be uplifted
        let minVlue = Math.floor((maxValue / 3) * 2);
        if (value < minVlue) {
            value = minVlue;
        }

        return value;
    }

    getType() {
        return this.Type;
    }

    getHealth() {
        return this.Health;
    }

    getStrength() {
        return this.Strength;
    }

    getProtection() {
        return this.Protection;
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