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
        this._index = index;
        this._message = message;
        this._safeDice = safeDice;
        this._dangerDice = dangerDice;
    }
}; 