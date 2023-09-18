//todo: rename enemy to monster
class Enemy {
    constructor() {
        this._type;

        this._health;
        this._damage;
        this._protection;

        this._isAlive;
    }

    generateEnemy(type, healthDice, damageDice, protectionDice) {
        this._type = type;

        this._health = this.secretRoll(healthDice);
        this._damage = this.secretRoll(damageDice);
        this._protection = this.secretRoll(protectionDice);

        this._isAlive = true;
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
        return this._type;
    }

    getHealth() {
        return this._health;
    }

    getDamage() {
        return this._damage;
    }

    getProtection() {
        return this._protection;
    }

    isAlive() {
        return this._isAlive;
    }

    reciveWounds(value) {
        let currentHealth = this._health - value;
        if (currentHealth > 0) {
            this._health = currentHealth;
        } else {
            this._health = 0;
            this._isAlive = false;
        }
    }
}; 