//  logs
//  -   log entry *
//      -   log action
//      -   log action
//  -   log entry *
//      -   log action
//      -   log action
//      -   log action

class LogEntry {
    constructor(title) {
        this._title = title;
        this._logActions = [];
    }

    getTile() {
        return this._title;
    }

    addLogEntry(actions) {
        if (actions !== null) {
            this._logActions.push(actions);
        }
    }
}; 