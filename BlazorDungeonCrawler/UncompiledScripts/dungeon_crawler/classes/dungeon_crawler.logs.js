//  logs *
//  -   log entry
//      -   log action
//      -   log action
//  -   log entry
//      -   log action
//      -   log action
//      -   log action

class Logs {
    constructor( ) {
        this._logs = [];
    }

    addEntry(LogEntry) {
        this._logs.push(LogEntry);
        this.setLog(LogEntry.getTile())
    }

    setLog(message) {
        $('#log').prepend(`<div class="entry">${message}</div>`);
    }
}; 