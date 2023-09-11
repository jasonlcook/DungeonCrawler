//  logs
//  -   log entry
//      -   log action *
//      -   log action *
//  -   log entry
//      -   log action *
//      -   log action *
//      -   log action *

class LogAction {
    constructor(index, message, safeDice, dangerDice) {
        this._id = dungeon_crawler.core.generateGuid();
        this._index = index;
        this._message = message;
        this._safeDice = safeDice;
        this._dangerDice = dangerDice;
        this._logAction;
    }

    getId() {
        return this._id;
    }

    getIndex() {
        return this._index;
    }

    getMessage() {
        return this._message;
    }

    getSafeDice() {
        return this._safeDice;
    }

    getDangerDice() {
        return this._dangerDice;
    }

    addChildLogAction(logAction) {
        this._logAction = logAction;
    }
}; 