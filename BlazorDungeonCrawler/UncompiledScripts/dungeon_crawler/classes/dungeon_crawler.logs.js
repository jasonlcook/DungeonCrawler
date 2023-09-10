//  logs *
//  -   log entry
//      -   log action
//      -   log action
//  -   log entry
//      -   log action
//      -   log action
//      -   log action

class Logs {
    constructor() {
        this._logs = [];
    }

    addEntry(LogEntry) {
        this._logs.push(LogEntry);
        this.setLog(LogEntry.getId(), LogEntry.getTile())
    }

    setLog(id, message) {
        $('#log').prepend(`<div data-id="${id}" class="entry">${message}</div>`);
    }
}; 